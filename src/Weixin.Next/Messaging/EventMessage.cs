using System;
using System.Xml.Linq;

namespace Weixin.Next.Messaging
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信发送过来的事件消息的类别
    /// </summary>
    public enum EventMessageType
    {
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
    }
    // ReSharper restore InconsistentNaming

    public abstract class EventMessage : RequestMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public EventMessageType Event { get; set; }

        #region Parse
        // ReSharper disable PossibleNullReferenceException
        /// <summary>
        /// 根据 XML 元素生成 EventMessage 对象
        /// </summary>
        /// <param name="root">代表微信消息中的 xml</param>
        /// <returns></returns>
        public static EventMessage Parse(XElement root)
        {
            var @event = (EventMessageType)Enum.Parse(typeof(EventMessageType), root.Element("Event").Value);
            EventMessage result = null;

            switch (@event)
            {
                case EventMessageType.subscribe:
                    result = Subscribe(root);
                    break;
                case EventMessageType.unsubscribe:
                    result = Unsubscribe(root);
                    break;
                case EventMessageType.scan:
                    result = Scan(root);
                    break;
                case EventMessageType.location:
                    result = Location(root);
                    break;
                case EventMessageType.click:
                    result = Click(root);
                    break;
                case EventMessageType.view:
                    result = View(root);
                    break;
            }

            if (result != null)
            {
                result.Event = @event;

                var keyed = result as KeyedEventMessage;
                if (keyed != null)
                {
                    keyed.EventKey = root.Element("EventKey")?.Value;

                    var ticked = result as TicketedEventMessage;
                    if (ticked != null)
                    {
                        ticked.Ticket = root.Element("Ticket")?.Value;
                    }
                }
            }

            return result;
        }

        private static EventMessage Subscribe(XElement root)
        {
            return new SubscribeEventMessage();
        }

        private static EventMessage Unsubscribe(XElement root)
        {
            return new UnsubscribeEventMessage();
        }

        private static EventMessage Scan(XElement root)
        {
            return new ScanEventMessage();
        }

        private static EventMessage Location(XElement root)
        {
            return new LocationEventMessage
            {
                Latitude = double.Parse(root.Element("Latitude").Value),
                Longitude = double.Parse(root.Element("Longitude").Value),
                Precision = double.Parse(root.Element("Precision").Value),
            };
        }

        private static EventMessage Click(XElement root)
        {
            return new ClickEventMessage();
        }

        private static EventMessage View(XElement root)
        {
            return new ViewEventMessage();
        }
        // ReSharper restore PossibleNullReferenceException
        #endregion
    }

    public abstract class KeyedEventMessage : EventMessage
    {
        /// <summary>
        /// 在不同子类中, 代表不同的意义
        /// </summary>
        public string EventKey { get; set; }
    }

    public abstract class TicketedEventMessage : KeyedEventMessage
    {
        /// <summary>
        /// 在不同子类中, 代表不同的意义
        /// </summary>
        public string Ticket { get; set; }
    }


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
    }

    /// <summary>
    /// location 上报地理位置
    /// </summary>
    public class LocationEventMessage : EventMessage
    {
        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 地理位置精度
        /// </summary>
        public double Precision { get; set; }
    }

    /// <summary>
    /// click 点击菜单拉取消息
    /// <para>
    /// EventKey: 与自定义菜单接口中KEY值对应
    /// </para>
    /// </summary>
    public class ClickEventMessage : KeyedEventMessage
    {
    }

    /// <summary>
    /// view 点击菜单拉取消息
    /// <para>
    /// EventKey: 设置的跳转URL
    /// </para>
    /// </summary>
    public class ViewEventMessage : KeyedEventMessage
    {
    }
}
