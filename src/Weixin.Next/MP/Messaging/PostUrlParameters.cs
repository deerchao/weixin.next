namespace Weixin.Next.MP.Messaging
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信服务器 POST XML 消息请求的网址中的参数
    /// </summary>
    public class PostUrlParameters
    {
        /// <summary>
        /// 签名串
        /// </summary>
        public string msg_signature { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 随机串
        /// </summary>
        public string nonce { get; set; }
    }
}
