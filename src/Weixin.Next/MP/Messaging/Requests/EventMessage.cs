using System;
using System.Xml.Linq;

namespace Weixin.Next.MP.Messaging.Requests
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信发送过来的事件消息的类别
    /// </summary>
    public enum EventMessageType
    {
        /// <summary>
        /// 未知类型的事件
        /// </summary>
        unknown,

        /// <summary>
        /// 关注事件
        /// </summary>
        subscribe,
        /// <summary>
        /// 取消关注事件
        /// </summary>
        unsubscribe,
        /// <summary>
        /// 扫描带参数二维码事件
        /// </summary>
        scan,
        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        location,

        /// <summary>
        /// 菜单点击事件
        /// </summary>
        click,
        /// <summary>
        /// 菜单浏览时间
        /// </summary>
        view,
        /// <summary>
        /// 扫码推事件的事件推送
        /// </summary>
        scancode_push,
        /// <summary>
        /// 扫码推事件且弹出“消息接收中”提示框的事件推送
        /// </summary>
        scancode_waitmsg,
        /// <summary>
        /// 弹出系统拍照发图的事件推送
        /// </summary>
        pic_sysphoto,
        /// <summary>
        /// 弹出拍照或者相册发图的事件推送
        /// </summary>
        pic_photo_or_album,
        /// <summary>
        /// 弹出微信相册发图器的事件推送
        /// </summary>
        pic_weixin,
        /// <summary>
        /// 弹出地理位置选择器的事件推送
        /// </summary>
        location_select,

        /// <summary>
        /// 资质认证成功
        /// </summary>
        qualification_verify_success,
        /// <summary>
        /// 资质认证失败
        /// </summary>
        qualification_verify_fail,
        /// <summary>
        /// 名称认证成功
        /// </summary>
        naming_verify_success,
        /// <summary>
        /// 名称认证失败
        /// </summary>
        naming_verify_fail,
        /// <summary>
        /// 年审通知
        /// </summary>
        annual_renew,
        /// <summary>
        /// 认证过期失效通知
        /// </summary>
        verify_expired,

        /// <summary>
        /// 模板消息发送完成
        /// </summary>
        templatesendjobfinish,
        /// <summary>
        /// 群发任务完成
        /// </summary>
        masssendjobfinish
    }
    // ReSharper restore InconsistentNaming

    public abstract class EventMessage : RequestMessage
    {
        protected EventMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        public EventMessageType Event { get; private set; }

        #region Parse
        // ReSharper disable PossibleNullReferenceException
        /// <summary>
        /// 根据 XML 元素生成 EventMessage 对象
        /// </summary>
        /// <param name="xml">代表微信消息中的 xml</param>
        /// <returns></returns>
        public static EventMessage Parse(XElement xml)
        {
            EventMessageType @event;
            if (!Enum.TryParse(xml.Element("Event").Value.ToLowerInvariant(), out @event))
                @event = EventMessageType.unknown;

            EventMessage result;
            switch (@event)
            {
                case EventMessageType.unknown:
                    result = new UnknownEventMessage(xml);
                    break;
                case EventMessageType.subscribe:
                    result = new SubscribeEventMessage(xml);
                    break;
                case EventMessageType.unsubscribe:
                    result = new UnsubscribeEventMessage(xml);
                    break;
                case EventMessageType.scan:
                    result = new ScanEventMessage(xml);
                    break;
                case EventMessageType.location:
                    result = new LocationEventMessage(xml);
                    break;
                case EventMessageType.qualification_verify_success:
                    result = new QualificationVerifySuccessEvent(xml);
                    break;
                case EventMessageType.qualification_verify_fail:
                    result = new QualificationVerifyFailEvent(xml);
                    break;
                case EventMessageType.naming_verify_success:
                    result = new NamingVerifySuccessEvent(xml);
                    break;
                case EventMessageType.naming_verify_fail:
                    result = new NamingVerifyFailEvent(xml);
                    break;
                case EventMessageType.annual_renew:
                    result = new AnnualRenewEvent(xml);
                    break;
                case EventMessageType.verify_expired:
                    result = new VerifyExpiredEvent(xml);
                    break;
                case EventMessageType.templatesendjobfinish:
                    result = new TemplateSendJobFinishEventMessage(xml);
                    break;
                case EventMessageType.masssendjobfinish:
                    result = new MassSendJobFinishEventMessage(xml);
                    break;
                default:
                    result = MenuMessage.Parse(xml, @event);
                    break;
            }

            result.Event = @event;

            return result;
        }
        // ReSharper restore PossibleNullReferenceException
        #endregion
    }

    public abstract class KeyedEventMessage : EventMessage
    {
        protected KeyedEventMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 在不同子类中, 代表不同的意义
        /// </summary>
        public string EventKey { get { return _xml.Element("EventKey")?.Value; } }
    }

    public abstract class TicketedEventMessage : KeyedEventMessage
    {
        protected TicketedEventMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 在不同子类中, 代表不同的意义
        /// </summary>
        public string Ticket { get { return _xml.Element("Ticket")?.Value; } }
    }

    #region Concrete
    /// <summary>
    /// subscribe 订阅
    /// <para>
    /// EventKey: 只有通过扫描带参数二维码关注时才有此字段，qrscene_为前缀，后面为二维码的参数值
    /// <br/>
    /// Ticket: 只有通过扫描带参数二维码关注时才有此字段，二维码的ticket，可用来换取二维码图片
    /// </para>
    /// </summary>
    public class SubscribeEventMessage : TicketedEventMessage
    {
        public SubscribeEventMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 获取扫描的二维码参数值
        /// </summary>
        /// <returns></returns>
        public string GetQrSceneValue()
        {
            return EventKey?.Substring("qrscene_".Length);
        }
    }

    /// <summary>
    /// unsubscribe 取消订阅
    /// </summary>
    public class UnsubscribeEventMessage : EventMessage
    {
        public UnsubscribeEventMessage(XElement xml) : base(xml)
        {
        }
    }

    /// <summary>
    /// scan 扫描带参数二维码
    /// <para>
    /// EventKey: 一个32位无符号整数，即创建二维码时的二维码scene_id
    /// <br/>
    /// Ticket: 二维码的ticket，可用来换取二维码图片
    /// </para>
    /// </summary>
    public class ScanEventMessage : TicketedEventMessage
    {
        public ScanEventMessage(XElement xml) : base(xml)
        {
        }
    }

    /// <summary>
    /// location 上报地理位置
    /// </summary>
    public class LocationEventMessage : EventMessage
    {
        public LocationEventMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public double Latitude { get { return double.Parse(_xml.Element("Latitude").Value); } }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Longitude { get { return double.Parse(_xml.Element("Longitude").Value); } }

        /// <summary>
        /// 地理位置精度
        /// </summary>
        public double Precision { get { return double.Parse(_xml.Element("Precision").Value); } }
    }

    public class TemplateSendJobFinishEventMessage : EventMessage
    {
        public TemplateSendJobFinishEventMessage(XElement xml) : base(xml)
        {
        }

        public long MsgID { get { return long.Parse(_xml.Element("MsgID").Value); } }

        public string Status { get { return _xml.Element("Status").Value; } }

        public bool IsSuccess()
        {
            return Status == "success";
        }
    }

    public class MassSendJobFinishEventMessage : EventMessage
    {
        public MassSendJobFinishEventMessage(XElement xml) : base(xml)
        {
        }

        public long MsgID { get { return long.Parse(_xml.Element("MsgID").Value); } }

        public string Status { get { return _xml.Element("Status").Value; } }

        public int TotalCount { get { return int.Parse(_xml.Element("TotalCount").Value); } }
        public int FilterCount { get { return int.Parse(_xml.Element("FilterCount").Value); } }
        public int SentCount { get { return int.Parse(_xml.Element("SentCount").Value); } }
        public int ErrorCount { get { return int.Parse(_xml.Element("ErrorCount").Value); } }

        public bool IsSuccess()
        {
            return Status == "send success";
        }
    }
    #endregion
}
