using System.Xml.Linq;

namespace Weixin.Next.MP.Messaging.Requests
{
    /// <summary>
    /// 未知类别的请求消息, 可以通过 Xml 属性直接获取消息内容
    /// </summary>
    public class UnknownRequestMessage : RequestMessage
    {
        public UnknownRequestMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 代表请求消息中 xml 元素
        /// </summary>
        public XElement Xml { get { return _xml; } }

        /// <summary>
        /// 消息中的 MsgType 值
        /// </summary>
        public string RawMsgType { get { return _xml.Element("MsgType").Value; } }


        public override string GetDuplicationKey()
        {
            return _xml.Element("MsgId")?.Value ?? base.GetDuplicationKey();
        }
    }
}
