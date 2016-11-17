using System.Xml.Linq;

namespace Weixin.Next.MP.Messaging.Requests
{
    /// <summary>
    /// 未知类别的事件消息, 可以通过 Xml 属性直接获取消息内容
    /// </summary>
    public class UnknownEventMessage : EventMessage
    {
        public UnknownEventMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 代表请求消息中 xml 元素
        /// </summary>
        public XElement Xml { get { return _xml; } }

        /// <summary>
        /// 消息中的 Event 值
        /// </summary>
        public string RawEvent { get { return _xml.Element("Event").Value; } }
    }
}
