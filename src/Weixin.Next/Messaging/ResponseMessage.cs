namespace Weixin.Next.Messaging
{
    public class ResponseMessage
    {
        /// <summary>
        /// <para>特殊的返回消息, 代表空字符串</para>
        /// <para>直接回复空串，微信服务器不会对此作任何处理，并且不会发起重试</para>
        /// </summary>
        public static readonly ResponseMessage Empty = new ResponseMessage();
    }
}