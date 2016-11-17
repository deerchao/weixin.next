using System;
using System.Linq;
using System.Xml.Linq;

namespace Weixin.Next.MP.Messaging.Requests
{
    public abstract class MenuMessage : KeyedEventMessage
    {
        public MenuMessage(XElement xml) : base(xml)
        {
        }

        public long MenuId { get { return long.Parse(_xml.Element("MenuId").Value); } }

        public static MenuMessage Parse(XElement xml, EventMessageType @event)
        {
            switch (@event)
            {

                case EventMessageType.click:
                    return new ClickMenuMessage(xml);
                case EventMessageType.view:
                    return new ViewMenuMessage(xml);
                case EventMessageType.scancode_push:
                    return new ScanCodePushMenuMessage(xml);
                case EventMessageType.scancode_waitmsg:
                    return new ScanCodeWaitMsgMenuMessage(xml);
                case EventMessageType.pic_sysphoto:
                    return new PicSysPhotoMenuMessage(xml);
                case EventMessageType.pic_photo_or_album:
                    return new PicPhotoOrAlbumMenuMessage(xml);
                case EventMessageType.pic_weixin:
                    return new PicWeixinMenuMessage(xml);
                case EventMessageType.location_select:
                    return new LocationSelectMenuMessage(xml);
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
        public ClickMenuMessage(XElement xml) : base(xml)
        {
        }
    }

    /// <summary>
    /// view 点击菜单拉取消息
    /// <para>
    /// EventKey: 设置的跳转URL
    /// </para>
    /// </summary>
    public class ViewMenuMessage : MenuMessage
    {
        public ViewMenuMessage(XElement xml) : base(xml)
        {
        }
    }

    #region scan

    public class ScanCodeInfo
    {
        private readonly XElement _element;

        public ScanCodeInfo(XElement element)
        {
            _element = element;
        }

        /// <summary>
        /// 扫描类型，一般是qrcode
        /// </summary>
        public string ScanType { get { return _element.Element("ScanType").Value; } }

        /// <summary>
        /// 扫描结果，即二维码对应的字符串信息
        /// </summary>
        public string ScanResult { get { return _element.Element("ScanResult").Value; } }
    }

    public abstract class ScanCodeMenuMessage : MenuMessage
    {
        public ScanCodeMenuMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 扫描信息
        /// </summary>
        public ScanCodeInfo ScanCodeInfo { get { return new ScanCodeInfo(_xml.Element("ScanCodeInfo")); } }
    }

    /// <summary>
    /// scancode_push 扫码推事件的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class ScanCodePushMenuMessage : ScanCodeMenuMessage
    {
        public ScanCodePushMenuMessage(XElement xml) : base(xml)
        {
        }
    }

    /// <summary>
    /// scancode_waitmsg 扫码推事件且弹出“消息接收中”提示框的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class ScanCodeWaitMsgMenuMessage : ScanCodeMenuMessage
    {
        public ScanCodeWaitMsgMenuMessage(XElement xml) : base(xml)
        {
        }
    }

    #endregion

    #region pic

    public class PicItem
    {
        private readonly XElement _element;

        public PicItem(XElement element)
        {
            _element = element;
        }

        /// <summary>
        /// 图片的MD5值，开发者若需要，可用于验证接收到图片
        /// </summary>
        public string PicMd5Sum { get { return _element.Value; } }
    }

    public class SendPicsInfo
    {
        private readonly XElement _element;

        public SendPicsInfo(XElement element)
        {
            _element = element;
        }

        /// <summary>
        /// 发送的图片数量
        /// </summary>
        public int Count { get { return int.Parse(_element.Element("Count").Value); } }

        /// <summary>
        /// 图片列表
        /// </summary>
        public PicItem[] PicList
        {
            get
            {
                return _element.Element("PicList")
                    .Elements("item")
                    .Select(x => new PicItem(x))
                    .ToArray();
            }
        }
    }

    public abstract class PicMenuMessage : MenuMessage
    {
        public PicMenuMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 发送的图片信息
        /// </summary>
        public SendPicsInfo SendPicsInfo { get { return new SendPicsInfo(_xml.Element("SendPicsInfo")); } }
    }

    /// <summary>
    /// pic_sysphoto 弹出系统拍照发图的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class PicSysPhotoMenuMessage : PicMenuMessage
    {
        public PicSysPhotoMenuMessage(XElement xml) : base(xml)
        {
        }
    }

    /// <summary>
    /// pic_photo_or_album 弹出拍照或者相册发图的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class PicPhotoOrAlbumMenuMessage : PicMenuMessage
    {
        public PicPhotoOrAlbumMenuMessage(XElement xml) : base(xml)
        {
        }
    }

    /// <summary>
    /// pic_weixin 弹出微信相册发图器的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class PicWeixinMenuMessage : PicMenuMessage
    {
        public PicWeixinMenuMessage(XElement xml) : base(xml)
        {
        }
    }

    #endregion

    public class SendLocationInfo
    {
        private readonly XElement _element;

        public SendLocationInfo(XElement element)
        {
            _element = element;
        }

        /// <summary>
        /// X坐标信息
        /// </summary>
        public double Location_X { get { return double.Parse(_element.Element("Location_X").Value); } }

        /// <summary>
        /// Y坐标信息
        /// </summary>
        public double Location_Y { get { return double.Parse(_element.Element("Location_Y").Value); } }

        /// <summary>
        /// 精度，可理解为精度或者比例尺、越精细的话 scale越高
        /// </summary>
        public int Scale { get { return int.Parse(_element.Element("Scale").Value); } }

        /// <summary>
        /// 地理位置的字符串信息
        /// </summary>
        public string Label { get { return _element.Element("Label").Value; } }

        /// <summary>
        /// 朋友圈POI的名字，可能为空
        /// </summary>
        public string Poiname { get { return _element.Element("Poiname").Value; } }
    }

    /// <summary>
    /// location_select 弹出地理位置选择器的事件推送
    /// <para>
    /// EventKey: 事件KEY值，由开发者在创建菜单时设定
    /// </para>
    /// </summary>
    public class LocationSelectMenuMessage : MenuMessage
    {
        public LocationSelectMenuMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 发送的位置信息
        /// </summary>
        public SendLocationInfo SendLocationInfo { get { return new SendLocationInfo(_xml.Element("SendLocationInfo")); } }
    }
}