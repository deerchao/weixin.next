using Weixin.Next.MP.Messaging;
using Weixin.Next.MP.Messaging.Caches;

namespace Weixin.Next.Sample.Models
{
    public class SampleMessageCenter : MessageCenter
    {
        public SampleMessageCenter(string appId, string token, string encodingAesKey)
            : base(appId, token, encodingAesKey)
        {
        }

        protected override IMessageHandler CreateHandler()
        {
            return new SampleMessageHandler();
        }

        protected override IExecutionDictionary CreateExecutionDictionary()
        {
            //如需自动去除重复消息, 使用 ExecutionDictionary
            return new ExecutionDictionary();
            //如不需自动去除重复消息, 使用 NullExecutionDictionary
            //return new NullExecutionDictionary();
        }

        protected override IResponseCache CreateResponseCache()
        {
            //如需自动去除重复消息, 使用 ResponseCache
            return new ResponseCache();
            //如不需自动去除重复消息, 使用 NullResponseCache
            //return new NullResponseCache();
        }
    }
}