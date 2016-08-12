using System.Threading.Tasks;

namespace Weixin.Next.Messaging
{
    public interface IMessageHandler
    {
        Task<ResponseMessage> Handle(RequestMessage message);
    }
}
