using System.Threading.Tasks;
using Weixin.Next.MP.Messaging;
using Weixin.Next.MP.Messaging.Requests;
using Weixin.Next.MP.Messaging.Responses;

namespace Weixin.Next.Sample.Models
{
    public class SampleMessageHandler : MessageHandler
    {
        // override 不同的方法来处理不同的消息

        protected override Task<IResponseMessage> HandleSubscribeEvent(SubscribeEventMessage request)
        {
            var scene = request.GetQrSceneValue();
            if (scene == null)
                return Empty();

            return Result(Text("欢迎扫码关注, 二维码场景值为: " + scene));
        }

        protected override Task<IResponseMessage> HandleTextRequest(TextRequestMessage request)
        {
            var content = request.Content;

            return content.Length > 2
                ? Result(Text($"网络太烂, 消息没收全, 再发一次吧! 只收到了 \"{content.Substring(content.Length / 2)}\".."))
                : Result(Text(content));
        }
    }
}