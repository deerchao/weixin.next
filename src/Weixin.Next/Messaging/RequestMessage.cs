using System;
using System.Xml.Linq;

namespace Weixin.Next.Messaging
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信发送过来的消息的类别
    /// </summary>
    public enum RequestMessageType
    {
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
        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 消息创建时间 （时间戳）
        /// </summary>
        public long CreateTime { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public RequestMessageType MsgType { get; set; }

        #region Parse
        // ReSharper disable PossibleNullReferenceException
        /// <summary>
        /// 根据 XML 字符串生成 RequestMessage 对象
        /// </summary>
        /// <param name="xml">解密后的 xml 字符串</param>
        /// <returns></returns>
        public static RequestMessage Parse(string xml)
        {
            var root = XDocument.Parse(xml).Root;

            var msgType = (RequestMessageType)Enum.Parse(typeof(RequestMessageType), root.Element("MsgType").Value);

            var result = msgType == RequestMessageType.@event
                ? (RequestMessage)EventMessage.Parse(root)
                : NormalRequestMessage.Parse(root, msgType);

            if (result != null)
            {
                result.ToUserName = root.Element("ToUserName").Value;
                result.FromUserName = root.Element("FromUserName").Value;
                result.CreateTime = long.Parse(root.Element("CreateTime").Value);
                result.MsgType = msgType;
            }
            return result;
        }
        // ReSharper restore PossibleNullReferenceException
        #endregion
    }

    public abstract class NormalRequestMessage : RequestMessage
    {
        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public long MsgId { get; set; }

        #region Parse
        // ReSharper disable PossibleNullReferenceException
        public static NormalRequestMessage Parse(XElement root, RequestMessageType type)
        {
            NormalRequestMessage result = null;
            switch (type)
            {
                case RequestMessageType.text:
                    result = Text(root);
                    break;
                case RequestMessageType.image:
                    result = Image(root);
                    break;
                case RequestMessageType.voice:
                    result = Voice(root);
                    break;
                case RequestMessageType.video:
                    result = Video(root);
                    break;
                case RequestMessageType.shortvideo:
                    result = ShortVideo(root);
                    break;
                case RequestMessageType.location:
                    result = Location(root);
                    break;
                case RequestMessageType.link:
                    result = Link(root);
                    break;
            }

            if (result != null)
            {
                result.MsgId = long.Parse(root.Element("MsgId").Value);
            }

            return result;
        }

        private static NormalRequestMessage Text(XElement root)
        {
            return new TextRequestMessage
            {
                Content = root.Element("Content").Value,
            };
        }

        private static NormalRequestMessage Image(XElement root)
        {
            return new ImageRequestMessage
            {
                PicUrl = root.Element("PicUrl").Value,
                MediaId = root.Element("MediaId").Value,
            };
        }

        private static NormalRequestMessage Voice(XElement root)
        {
            return new VoiceRequestMessage
            {
                MediaId = root.Element("MediaId").Value,
                Format = root.Element("Format").Value,
                Recognition = root.Element("Recognition")?.Value,
            };
        }

        private static NormalRequestMessage Video(XElement root)
        {
            return new VideoRequestMessage
            {
                MediaId = root.Element("MediaId").Value,
                ThumbMediaId = root.Element("ThumbMediaId").Value,
            };
        }

        private static NormalRequestMessage ShortVideo(XElement root)
        {
            return new ShortVideoRequestMessage
            {
                MediaId = root.Element("MediaId").Value,
                ThumbMediaId = root.Element("ThumbMediaId").Value,
            };
        }

        private static NormalRequestMessage Location(XElement root)
        {
            return new LocationRequestMessage
            {
                Location_X = double.Parse(root.Element("Location_X").Value),
                Location_Y = double.Parse(root.Element("Location_Y").Value),
                Scale = int.Parse(root.Element("Scale").Value),
                Label = root.Element("Label").Value,
            };
        }

        private static NormalRequestMessage Link(XElement root)
        {
            return new LinkRequestMessage
            {
                Title = root.Element("Title").Value,
                Description = root.Element("Description").Value,
                Url = root.Element("Url").Value,
            };
        }
        // ReSharper restore PossibleNullReferenceException
        #endregion
    }

    /// <summary>
    /// text 文本消息
    /// </summary>
    public class TextRequestMessage : NormalRequestMessage
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
    }

    /// <summary>
    /// image 图片消息
    /// </summary>
    public class ImageRequestMessage : NormalRequestMessage
    {
        /// <summary>
        /// 图片链接（由系统生成）
        /// </summary>
        public string PicUrl { get; set; }

        /// <summary>
        /// 媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }
    }

    /// <summary>
    /// voice 语音消息
    /// </summary>
    public class VoiceRequestMessage : NormalRequestMessage
    {
        /// <summary>
        /// 媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 开通语音识别后才有, 语音识别结果
        /// </summary>
        public string Recognition { get; set; }
    }

    /// <summary>
    /// video 视频消息
    /// </summary>
    public class VideoRequestMessage : NormalRequestMessage
    {
        /// <summary>
        /// 媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数
        /// </summary>
        public string ThumbMediaId { get; set; }
    }

    /// <summary>
    /// shortvideo 小视频消息
    /// </summary>
    public class ShortVideoRequestMessage : NormalRequestMessage
    {
        /// <summary>
        /// 媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数
        /// </summary>
        public string ThumbMediaId { get; set; }
    }

    /// <summary>
    /// location 地理位置消息
    /// </summary>
    public class LocationRequestMessage : NormalRequestMessage
    {
        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public double Location_X { get; set; }
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Location_Y { get; set; }
        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public int Scale { get; set; }
        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get; set; }
    }

    /// <summary>
    /// link 链接消息
    /// </summary>
    public class LinkRequestMessage : NormalRequestMessage
    {

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 消息链接
        /// </summary>
        public string Url { get; set; }
    }

}
