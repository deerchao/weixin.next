using System.Threading.Tasks;
using Weixin.Next.Messaging.Requests;
using Weixin.Next.Messaging.Responses;

namespace Weixin.Next.Messaging
{
    /// <summary>
    /// 负责处理单个请求, 每个非重复的请求创建一个
    /// </summary>
    public interface IMessageHandler
    {
        Task<IResponseMessage> Handle(RequestMessage request);
    }
}
