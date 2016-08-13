using System;
using System.Linq;
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
                default:
                    result = MenuMessage.Parse(root, @event);
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
    #endregion

    #region Menu

    public abstract class MenuMessage : KeyedEventMessage
    {
        public static MenuMessage Parse(XElement root, EventMessageType @event)
        {
            MenuMessage result = null;
            switch (@event)
            {

                case EventMessageType.click:
                    return Click(root);
                case EventMessageType.view:
                    return View(root);
                case EventMessageType.scancode_push:
                    return ScanCodePush(root);
                case EventMessageType.scancode_waitmsg:
                    return ScanCodeWaitMsg(root);
                case EventMessageType.pic_sysphoto:
                    return PicSysPhoto(root);
                case EventMessageType.pic_photo_or_album:
                    return PicPhotoOrAlbum(root);
                case EventMessageType.pic_weixin:
                    return PicWeixin(root);
                case EventMessageType.location_select:
                    return LocationSelect(root);
            }

            return result;
        }

        private static MenuMessage Click(XElement root)
        {
            return new ClickMenuMessage();
        }

        private static MenuMessage View(XElement root)
        {
            return new ViewMenuMessage();
        }

        private static MenuMessage ScanCodePush(XElement root)
        {
            var sci = root.Element("ScanCodeInfo");
            return new ScanCodePushMenuMessage
            {
                ScanCodeInfo = new ScanCodeInfo
                {
                    ScanType = sci.Element("ScanType").Value,
                    ScanResult = sci.Element("ScanResult").Value,
                }
            };
        }

        private static MenuMessage ScanCodeWaitMsg(XElement root)
        {
            var sci = root.Element("ScanCodeInfo");
            return new ScanCodeWaitMsgMenuMessage
            {
                ScanCodeInfo = new ScanCodeInfo
                {
                    ScanType = sci.Element("ScanType").Value,
                    ScanResult = sci.Element("ScanResult").Value,
                }
            };

        }

        private static MenuMessage PicSysPhoto(XElement root)
        {
            var spi = root.Element("SendPicsInfo");
            return new PicSysPhotoMenuMessage
            {
                SendPicsInfo = new SendPicsInfo
                {
                    Count = int.Parse(spi.Element("Count").Value),
                    PicList = spi.Elements("item").Select(x => new PicItem
                    {
                        PicMd5Sum = x.Value,
                    }).ToArray(),
                }
            };
        }

        private static MenuMessage PicPhotoOrAlbum(XElement root)
        {
            var spi = root.Element("SendPicsInfo");
            return new PicPhotoOrAlbumMenuMessage
            {
                SendPicsInfo = new SendPicsInfo
                {
                    Count = int.Parse(spi.Element("Count").Value),
                    PicList = spi.Elements("item").Select(x => new PicItem
                    {
                        PicMd5Sum = x.Value,
                    }).ToArray(),
                }
            };
        }

        private static MenuMessage PicWeixin(XElement root)
        {
            var spi = root.Element("SendPicsInfo");
            return new PicWeixinMenuMessage
            {
                SendPicsInfo = new SendPicsInfo
                {
                    Count = int.Parse(spi.Element("Count").Value),
                    PicList = spi.Elements("item").Select(x => new PicItem
                    {
                        PicMd5Sum = x.Value,
                    }).ToArray(),
                }
            };
        }

        private static MenuMessage LocationSelect(XElement root)
        {
            var sli = root.Element("SendLocationInfo");
            return new LocationSelectMenuMessage
            {
                SendLocationInfo = new SendLocationInfo
                {
                    Location_X = double.Parse(sli.Element("Location_X").Value),
                    Location_Y = double.Parse(sli.Element("Location_Y").Value),
                    Scale = int.Parse(sli.Element("Scale").Value),
                    Label = sli.Element("Label").Value,
                    Poiname = sli.Element("Poiname").Value,
                }
            };
        }
    }

    /// <summary>
    /// click 点击菜单拉取消息
    /// <para>
    /// EventKey: 与自定义菜单接口中KEY值对应
    /// </para>
    /// </summary>
    public class ClickMenuMessage : MenuMessage
    {
    }

    /// <summary>
    /// view 点击菜单拉取消息
    /// <para>
    /// EventKey: 设置的跳转URL
    /// </para>
    /// </summary>
    public class ViewMenuMessage : MenuMessage
    {
    }

    #region scan

    public class ScanCodeInfo
    {
        /// <summary>
        /// 扫描类型，一般是qrcode
        /// </summary>
        public string ScanType { get; set; }

        /// <summary>
        /// 扫描结果，即二维码对应的字符串信息
        /// </summary>
        public string ScanResult { get; set; }
    }

    public abstract class ScanCodeMenuMessage : MenuMessage
    {
        /// <summary>
        /// 扫描信息
        /// </summary>
        public ScanCodeInfo ScanCodeInfo { get; set; }
    }

    /// <summary>
    /// scancode_push 扫码推事件的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class ScanCodePushMenuMessage : ScanCodeMenuMessage
    {
    }

    /// <summary>
    /// scancode_waitmsg 扫码推事件且弹出“消息接收中”提示框的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class ScanCodeWaitMsgMenuMessage : ScanCodeMenuMessage
    {
    }

    #endregion

    #region pic

    public class PicItem
    {
        /// <summary>
        /// 图片的MD5值，开发者若需要，可用于验证接收到图片
        /// </summary>
        public string PicMd5Sum { get; set; }
    }

    public class SendPicsInfo
    {
        /// <summary>
        /// 发送的图片数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 图片列表
        /// </summary>
        public PicItem[] PicList { get; set; }
    }

    public abstract class PicMenuMessage : MenuMessage
    {
        /// <summary>
        /// 发送的图片信息
        /// </summary>
        public SendPicsInfo SendPicsInfo { get; set; }
    }

    /// <summary>
    /// pic_sysphoto 弹出系统拍照发图的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class PicSysPhotoMenuMessage : PicMenuMessage
    {
    }

    /// <summary>
    /// pic_photo_or_album 弹出拍照或者相册发图的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class PicPhotoOrAlbumMenuMessage : PicMenuMessage
    {
    }

    /// <summary>
    /// pic_weixin 弹出微信相册发图器的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class PicWeixinMenuMessage : PicMenuMessage
    {
    }

    #endregion

    public class SendLocationInfo
    {
        /// <summary>
        /// X坐标信息
        /// </summary>
        public double Location_X { get; set; }

        /// <summary>
        /// Y坐标信息
        /// </summary>
        public double Location_Y { get; set; }

        /// <summary>
        /// 精度，可理解为精度或者比例尺、越精细的话 scale越高
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 地理位置的字符串信息
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 朋友圈POI的名字，可能为空
        /// </summary>
        public string Poiname { get; set; }
    }

    /// <summary>
    /// location_select 弹出地理位置选择器的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class LocationSelectMenuMessage : MenuMessage
    {
        /// <summary>
        /// 发送的位置信息
        /// </summary>
        public SendLocationInfo SendLocationInfo { get; set; }
    }
    #endregion
}
