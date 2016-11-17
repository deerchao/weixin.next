using System.Threading.Tasks;
using Weixin.Next.MP.Messaging.Requests;
using Weixin.Next.MP.Messaging.Responses;

namespace Weixin.Next.MP.Messaging
{
    /// <summary>
    /// 负责处理单个请求, 每个非重复的请求创建一个
    /// </summary>
    public interface IMessageHandler
    {
        Task<IResponseMessage> Handle(RequestMessage request);
    }
}
