namespace Weixin.Next.Messaging
{
    public abstract class EventMessage : RequestMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public string Event { get; set; }
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
