using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tencent;
using Weixin.Next.Messaging.Requests;

namespace Weixin.Next.Messaging
{
    /// <summary>
    /// 负责处理所有请求, 应当每个应用(appId) 创建一个
    /// </summary>
    public abstract class MessageCenter
    {
        private readonly WXBizMsgCrypt _cryptor;

        public MessageCenter(string appId, string token, string encodingAesKey)
        {
            _cryptor = new WXBizMsgCrypt(token, encodingAesKey, appId);
        }

        public async Task<string> ProcessMessage(PostUrlParameters urlParameters, Stream requestStream)
        {
            var inputData = await new StreamReader(requestStream, Encoding.UTF8).ReadToEndAsync().ConfigureAwait(false);
            var request = "";

            var decryptResult = _cryptor.DecryptMsg(urlParameters.msg_signature, urlParameters.timestamp, urlParameters.nonce, inputData, ref request);
            if (decryptResult != WXBizMsgCrypt.WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK)
                return $"解密失败: {decryptResult}";

            var requestMessage = RequestMessage.Parse(request);

            //todo: 处理重复请求
            var handler = CreateHandler();
            var responseMessage = await handler.Handle(requestMessage).ConfigureAwait(false);

            var response = responseMessage.Serialize();
            if (!responseMessage.EncryptionRequired)
                return response;

            var outputData = "";
            var encryptResult = _cryptor.EncryptMsg(response, urlParameters.timestamp, urlParameters.nonce, ref outputData);
            if (encryptResult != WXBizMsgCrypt.WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK)
                return $"加密失败: {encryptResult}";
            return outputData;
        }

        protected abstract IMessageHandler CreateHandler();
    }
}
