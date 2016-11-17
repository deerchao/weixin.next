using System;
using System.Xml.Linq;

namespace Weixin.Next.MP.Messaging.Requests
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信发送过来的消息的类别
    /// </summary>
    public enum RequestMessageType
    {
        /// <summary>
        /// 未知类型的消息
        /// </summary>
        unknown,
        /// <summary>
        /// 文本消息
        /// </summary>
        text,
        /// <summary>
        /// 图片消息
        /// </summary>
        image,
        /// <summary>
        /// 语音消息
        /// </summary>
        voice,
        /// <summary>
        /// 视频消息
        /// </summary>
        video,
        /// <summary>
        /// 小视频消息
        /// </summary>
        shortvideo,
        /// <summary>
        /// 地理位置消息
        /// </summary>
        location,
        /// <summary>
        /// 链接消息
        /// </summary>
        link,
        /// <summary>
        /// 事件消息
        /// </summary>
        @event,
    }
    // ReSharper restore InconsistentNaming

    /// <summary>
    /// 微信发送过来的消息
    /// </summary>
    public abstract class RequestMessage
    {
        protected readonly XElement _xml;

        protected RequestMessage(XElement xml)
        {
            _xml = xml;
        }

        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string ToUserName { get { return _xml.Element("ToUserName").Value; } }
        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get { return _xml.Element("FromUserName").Value; } }
        /// <summary>
        /// 消息创建时间 （时间戳）
        /// </summary>
        public long CreateTime { get { return long.Parse(_xml.Element("CreateTime").Value); } }
        /// <summary>
        /// 消息类型
        /// </summary>
        public RequestMessageType MsgType { get; private set; }

        /// <summary>
        /// 去除重复消息的依据
        /// </summary>
        /// <returns></returns>
        public virtual string GetDuplicationKey()
        {
            return _xml.Element("FromUserName").Value +
                   _xml.Element("CreateTime").Value;
        }

        #region Parse
        // ReSharper disable PossibleNullReferenceException
        /// <summary>
        /// 根据 XML 字符串生成 RequestMessage 对象
        /// </summary>
        /// <param name="xmlString">解密后的 xml 字符串</param>
        /// <returns></returns>
        public static RequestMessage Parse(string xmlString)
        {
            var xml = XDocument.Parse(xmlString).Root;

            RequestMessageType msgType;
            if (!Enum.TryParse(xml.Element("MsgType").Value.ToLowerInvariant(), out msgType))
                msgType = RequestMessageType.unknown;

            var result = msgType == RequestMessageType.unknown
                ? new UnknownRequestMessage(xml)
                : msgType == RequestMessageType.@event
                ? (RequestMessage)EventMessage.Parse(xml)
                : NormalRequestMessage.Parse(xml, msgType);

            result.MsgType = msgType;

            return result;
        }
        // ReSharper restore PossibleNullReferenceException
        #endregion
    }

    public abstract class NormalRequestMessage : RequestMessage
    {
        protected NormalRequestMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public long MsgId { get { return long.Parse(_xml.Element("MsgId").Value); } }

        public override string GetDuplicationKey()
        {
            return _xml.Element("MsgId").Value;
        }

        #region Parse
        // ReSharper disable PossibleNullReferenceException
        public static NormalRequestMessage Parse(XElement xml, RequestMessageType type)
        {
            NormalRequestMessage result = null;
            switch (type)
            {
                case RequestMessageType.text:
                    result = new TextRequestMessage(xml);
                    break;
                case RequestMessageType.image:
                    result = new ImageRequestMessage(xml);
                    break;
                case RequestMessageType.voice:
                    result = new VoiceRequestMessage(xml);
                    break;
                case RequestMessageType.video:
                    result = new VideoRequestMessage(xml);
                    break;
                case RequestMessageType.shortvideo:
                    result = new ShortVideoRequestMessage(xml);
                    break;
                case RequestMessageType.location:
                    result = new LocationRequestMessage(xml);
                    break;
                case RequestMessageType.link:
                    result = new LinkRequestMessage(xml);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }
        // ReSharper restore PossibleNullReferenceException
        #endregion
    }

    /// <summary>
    /// text 文本消息
    /// </summary>
    public class TextRequestMessage : NormalRequestMessage
    {
        public TextRequestMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get { return _xml.Element("Content").Value; } }
    }

    /// <summary>
    /// image 图片消息
    /// </summary>
    public class ImageRequestMessage : NormalRequestMessage
    {
        public ImageRequestMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 图片链接（由系统生成）
        /// </summary>
        public string PicUrl { get { return _xml.Element("PicUrl").Value; } }

        /// <summary>
        /// 媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get { return _xml.Element("MediaId").Value; } }
    }

    /// <summary>
    /// voice 语音消息
    /// </summary>
    public class VoiceRequestMessage : NormalRequestMessage
    {
        public VoiceRequestMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get { return _xml.Element("MediaId").Value; } }
        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>
        public string Format { get { return _xml.Element("Format").Value; } }
        /// <summary>
        /// 开通语音识别后才有, 语音识别结果
        /// </summary>
        public string Recognition { get { return _xml.Element("Recognition")?.Value; } }
    }

    /// <summary>
    /// video 视频消息
    /// </summary>
    public class VideoRequestMessage : NormalRequestMessage
    {
        public VideoRequestMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get { return _xml.Element("MediaId").Value; } }
        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数
        /// </summary>
        public string ThumbMediaId { get { return _xml.Element("ThumbMediaId").Value; } }
    }

    /// <summary>
    /// shortvideo 小视频消息
    /// </summary>
    public class ShortVideoRequestMessage : NormalRequestMessage
    {
        public ShortVideoRequestMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get { return _xml.Element("MediaId").Value; } }
        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数
        /// </summary>
        public string ThumbMediaId { get { return _xml.Element("ThumbMediaId").Value; } }
    }

    /// <summary>
    /// location 地理位置消息
    /// </summary>
    public class LocationRequestMessage : NormalRequestMessage
    {
        public LocationRequestMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public double Location_X { get { return double.Parse(_xml.Element("Location_X").Value); } }
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Location_Y { get { return double.Parse(_xml.Element("Location_Y").Value); } }
        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public int Scale { get { return int.Parse(_xml.Element("Scale").Value); } }
        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get { return _xml.Element("Label").Value; } }
    }

    /// <summary>
    /// link 链接消息
    /// </summary>
    public class LinkRequestMessage : NormalRequestMessage
    {
        public LinkRequestMessage(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get { return _xml.Element("Title").Value; } }
        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get { return _xml.Element("Description").Value; } }
        /// <summary>
        /// 消息链接
        /// </summary>
        public string Url { get { return _xml.Element("Url").Value; } }
    }
}
