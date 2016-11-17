using System.IO;
using System.Text;
using System.Threading.Tasks;
using Weixin.Next.MP.Messaging.Caches;
using Weixin.Next.MP.Messaging.Crypt;
using Weixin.Next.MP.Messaging.Requests;
using Weixin.Next.MP.Messaging.Responses;

namespace Weixin.Next.MP.Messaging
{
    /// <summary>
    /// 负责处理所有请求, 应当每个应用(appId) 创建一个
    /// </summary>
    public abstract class MessageCenter
    {
        private readonly WXBizMsgCrypt _cryptor;
        private IExecutionDictionary _executionDictionary;
        private IResponseCache _responseCache;

        /// <summary>
        /// 创建需要加密/解密的 MessageCenter
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="token"></param>
        /// <param name="encodingAesKey"></param>
        public MessageCenter(string appId, string token, string encodingAesKey)
        {
            _cryptor = new WXBizMsgCrypt(token, encodingAesKey, appId);
        }

        /// <summary>
        /// 创建无需加密/解密的 MessageCenter
        /// </summary>
        public MessageCenter()
        {
        }

        public void Initialize()
        {
            _executionDictionary = CreateExecutionDictionary();
            _responseCache = CreateResponseCache();
        }

        public async Task<string> ProcessMessage(PostUrlParameters urlParameters, Stream requestStream)
        {
            var requestMessage = BuildRequest(urlParameters, requestStream);

            var key = requestMessage.GetDuplicationKey();

            // 如果是正在处理中的重复消息, 则返回等待处理完成返回处理结果
            var responseText = await GetResponseFromExecution(urlParameters, key).ConfigureAwait(false);
            if (responseText != null)
            {
                OnResponseGenerated(responseText, ResponseSource.Executing);
                return responseText;
            }

            // 如果是已处理的重复消息, 则直接返回待处理结果
            responseText = await GetResponseFromCache(key);
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

            var responseMessage = await task.ConfigureAwait(false);
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

        private async Task<string> GetResponseFromExecution(PostUrlParameters urlParameters, string key)
        {
            var executionTask = _executionDictionary.Get(key, false);
            if (executionTask != null)
            {
                var responseMessage = await executionTask.ConfigureAwait(false);
                return SerializeResponse(urlParameters, responseMessage);
            }

            return null;
        }

        private async Task<string> GetResponseFromCache(string key)
        {
            var cacheTask = _responseCache.Get(key, false);
            if (cacheTask != null)
            {
                var responseText = await cacheTask.ConfigureAwait(false);
                return responseText;
            }
            return null;
        }


        private RequestMessage BuildRequest(PostUrlParameters urlParameters, Stream requestStream)
        {
            var inputData = new StreamReader(requestStream, Encoding.UTF8).ReadToEnd();
            var request = inputData;

            if (_cryptor != null)
            {
                var decryptResult = _cryptor.DecryptMsg(urlParameters.msg_signature, urlParameters.timestamp, urlParameters.nonce, inputData, ref request);
                if (decryptResult != WXBizMsgCrypt.WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK)
                    throw new MessageException($"解密失败: {decryptResult}");
            }

            OnRequestRead(request);

            return RequestMessage.Parse(request);
        }

        private string SerializeResponse(PostUrlParameters urlParameters, IResponseMessage responseMessage)
        {
            var response = responseMessage.Serialize();
            if (!responseMessage.EncryptionRequired)
                return response;

            var outputData = response;

            if (_cryptor != null)
            {
                var encryptResult = _cryptor.EncryptMsg(response, urlParameters.timestamp, urlParameters.nonce, ref outputData);
                if (encryptResult != WXBizMsgCrypt.WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK)
                    throw new MessageException($"加密失败: {encryptResult}");
            }

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
