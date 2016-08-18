using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tencent;
using Weixin.Next.Messaging.Caches;
using Weixin.Next.Messaging.Requests;
using Weixin.Next.Messaging.Responses;

namespace Weixin.Next.Messaging
{
    /// <summary>
    /// 负责处理所有请求, 应当每个应用(appId) 创建一个
    /// </summary>
    public abstract class MessageCenter
    {
        private readonly WXBizMsgCrypt _cryptor;
        private IExecutionDictionary _executionDictionary;
        private IResponseCache _responseCache;

        public MessageCenter(string appId, string token, string encodingAesKey)
        {
            _cryptor = new WXBizMsgCrypt(token, encodingAesKey, appId);
        }


        public void Initialize()
        {
            _executionDictionary = CreateExecutionDictionary();
            _responseCache = CreateResponseCache();
        }

        public async Task<string> ProcessMessage(PostUrlParameters urlParameters, Stream requestStream)
        {
            var requestMessage = BuildRequest(urlParameters, requestStream);

            string responseText;
            var key = requestMessage.GetDuplicationKey();

            // 如果是正在处理中的重复消息, 则返回等待处理完成返回处理结果
            var responseMessage = await _executionDictionary.Get(key, false).ConfigureAwait(false);
            if (responseMessage != null)
            {
                responseText = SerializeResponse(urlParameters, responseMessage);
                OnResponseGenerated(responseText, ResponseSource.Executing);
                return responseText;
            }

            // 如果是已处理的重复消息, 则直接返回待处理结果
            responseText = await _responseCache.Get(key, false).ConfigureAwait(false);
            if (responseText != null)
            {
                OnResponseGenerated(responseText, ResponseSource.Cache);
                return responseText;
            }

            var handler = CreateHandler();
            var task = handler.Handle(requestMessage);

            // 开始处理后, 保存正在处理的消息
            var done = task.IsCompleted;
            if (!done)
            {
                _executionDictionary.Add(key, task);
            }

            responseMessage = await task.ConfigureAwait(false);
            responseText = SerializeResponse(urlParameters, responseMessage);

            // 处理完成后, 从正在处理转移到处理完成
            await _responseCache.Add(key, responseText).ConfigureAwait(false);
            if (!done)
            {
                _executionDictionary.Remove(key);
            }

            OnResponseGenerated(responseText, ResponseSource.New);
            return responseText;
        }


        private RequestMessage BuildRequest(PostUrlParameters urlParameters, Stream requestStream)
        {
            var inputData = new StreamReader(requestStream, Encoding.UTF8).ReadToEnd();
            var request = "";

            var decryptResult = _cryptor.DecryptMsg(urlParameters.msg_signature, urlParameters.timestamp, urlParameters.nonce, inputData, ref request);
            if (decryptResult != WXBizMsgCrypt.WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK)
                throw new MessageException($"解密失败: {decryptResult}");

            OnRequestRead(request);

            return RequestMessage.Parse(request);
        }

        private string SerializeResponse(PostUrlParameters urlParameters, IResponseMessage responseMessage)
        {
            var response = responseMessage.Serialize();
            if (!responseMessage.EncryptionRequired)
                return response;

            var outputData = "";
            var encryptResult = _cryptor.EncryptMsg(response, urlParameters.timestamp, urlParameters.nonce, ref outputData);
            if (encryptResult != WXBizMsgCrypt.WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK)
                throw new MessageException($"加密失败: {encryptResult}");

            return outputData;
        }

        /// <summary>
        /// 读取并解密完请求消息时执行, 设计用途: 写入日志
        /// </summary>
        /// <param name="requestText">请求体文本</param>
        protected virtual void OnRequestRead(string requestText)
        {
        }

        /// <summary>
        /// 处理完请求后执行, 设计用途: 写入日志
        /// </summary>
        /// <param name="responseText">响应文本</param>
        /// <param name="source">响应文本来源</param>
        protected virtual void OnResponseGenerated(string responseText, ResponseSource source)
        {
        }

        protected abstract IMessageHandler CreateHandler();

        /// <summary>
        /// 使用 <see cref="NullExecutionDictionary"/> 来忽略处理中消息重复排除
        /// </summary>
        protected virtual IExecutionDictionary CreateExecutionDictionary()
        {
            return new ExecutionDictionary();
        }

        /// <summary>
        /// 使用 <see cref="NullResponseCache"/> 来忽略处理完消息重复排除
        /// </summary>
        protected virtual IResponseCache CreateResponseCache()
        {
            return new ResponseCache();
        }
    }

    /// <summary>
    /// 响应来源
    /// </summary>
    public enum ResponseSource
    {
        /// <summary>
        /// 响应来自于对新请求的新处理过程
        /// </summary>
        New,
        /// <summary>
        /// 响应来自尚未执行完毕的重复消息处理过程
        /// </summary>
        Executing,
        /// <summary>
        /// 响应来自已处理完成的重复消息缓存
        /// </summary>
        Cache,
    }
}
