using System;
using System.Linq;
using System.Xml.Linq;
using Weixin.Next.Utilities;

namespace Weixin.Next.MP.Messaging.Responses
{
    // ReSharper disable InconsistentNaming
    public enum ResponseMessageType
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
        /// 音乐消息
        /// </summary>
        music,
        /// <summary>
        /// 图文消息
        /// </summary>
        news,
        /// <summary>
        /// 转发到客服
        /// </summary>
        transfer_customer_service,
    }
    // ReSharper restore InconsistentNaming

    public abstract class ResponseMessage : IResponseMessage
    {
        protected ResponseMessage()
        {
            CreateTime = DateTime.UtcNow.ToWeixinTimestamp();
        }

        /// <summary>
        /// 接收方帐号（收到的OpenID）
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 消息创建时间 （时间戳）
        /// </summary>
        public long CreateTime { get; set; }
        /// <summary>
        /// 消息类别
        /// </summary>
        public abstract ResponseMessageType MsgType { get; }

        public bool EncryptionRequired
        {
            get { return true; }
        }

        public string Serialize()
        {
            var xml = new XElement("xml",
                new XElement("ToUserName", ToUserName),
                new XElement("FromUserName", FromUserName),
                new XElement("CreateTime", CreateTime),
                new XElement("MsgType", MsgType.ToString("G"))
                );

            var children = SerializeElements();
            if (children != null)
            {
                foreach (var child in children)
                {
                    // 清除没有值的 XElement
                    child.Descendants()
                        .Reverse()
                        .Where(x => !x.HasElements && string.IsNullOrEmpty(x.Value))
                        .Remove();

                    if (!child.HasElements && string.IsNullOrEmpty(child.Value))
                        continue;

                    xml.Add(child);
                }
            }

            return xml.ToString(SaveOptions.DisableFormatting);
        }

        protected virtual XElement[] SerializeElements()
        {
            return null;
        }
    }


    /// <summary>
    /// text 文本消息
    /// </summary>
    public class TextResponseMessage : ResponseMessage
    {
        /// <summary>
        /// 回复的消息内容（换行：在content中能够换行，微信客户端就支持换行显示）
        /// </summary>
        public string Content { get; set; }

        public override ResponseMessageType MsgType
        {
            get { return ResponseMessageType.text; }
        }

        protected override XElement[] SerializeElements()
        {
            return new[]
            {
                new XElement("Content", Content),
            };
        }
    }

    /// <summary>
    /// image 图片消息
    /// </summary>
    public class ImageResponseMessage : ResponseMessage
    {
        /// <summary>
        /// 通过素材管理中的接口上传多媒体文件，得到的id
        /// </summary>
        public string MediaId { get; set; }

        public override ResponseMessageType MsgType
        {
            get { return ResponseMessageType.image; }
        }

        protected override XElement[] SerializeElements()
        {
            return new[]
            {
                new XElement("MediaId", MediaId),
            };
        }
    }

    /// <summary>
    /// voice 语音消息
    /// </summary>
    public class VoiceResponseMessage : ResponseMessage
    {
        /// <summary>
        /// 通过素材管理中的接口上传多媒体文件，得到的id
        /// </summary>
        public string MediaId { get; set; }

        public override ResponseMessageType MsgType
        {
            get { return ResponseMessageType.voice; }
        }

        protected override XElement[] SerializeElements()
        {
            return new[]
            {
                new XElement("MediaId", MediaId),
            };
        }
    }

    /// <summary>
    /// video 视频消息
    /// </summary>
    public class VideoResponseMessage : ResponseMessage
    {
        /// <summary>
        /// 通过素材管理中的接口上传多媒体文件，得到的id
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 可选, 视频消息的标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 可选, 视频消息的描述
        /// </summary>
        public string Description { get; set; }

        public override ResponseMessageType MsgType
        {
            get { return ResponseMessageType.video; }
        }

        protected override XElement[] SerializeElements()
        {
            return new[]
            {
                new XElement("MediaId", MediaId),
                new XElement("Title", Title),
                new XElement("Description", Description),
            };
        }
    }

    /// <summary>
    /// music 音乐消息
    /// </summary>
    public class MusicResponseMessage : ResponseMessage
    {
        public MusicInfo Music { get; set; }

        public class MusicInfo
        {
            /// <summary>
            /// 可选, 音乐标题
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 可选, 音乐描述
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// 音乐链接
            /// </summary>
            public string MusicURL { get; set; }
            /// <summary>
            /// 可选, 高质量音乐链接，WIFI环境优先使用该链接播放音乐
            /// </summary>
            public string HQMusicUrl { get; set; }
            /// <summary>
            /// 缩略图的媒体id，通过素材管理中的接口上传多媒体文件，得到的id
            /// </summary>
            public string ThumbMediaId { get; set; }
        }

        public override ResponseMessageType MsgType
        {
            get { return ResponseMessageType.music; }
        }

        protected override XElement[] SerializeElements()
        {
            return new[]
            {
                new XElement("Music",
                    new XElement("Title", Music.Title),
                    new XElement("Description", Music.Description),
                    new XElement("MusicURL", Music.MusicURL),
                    new XElement("HQMusicUrl", Music.HQMusicUrl),
                    new XElement("ThumbMediaId", Music.ThumbMediaId)),
            };
        }
    }

    /// <summary>
    /// news 图文消息
    /// </summary>
    public class NewsResponseMessage : ResponseMessage
    {
        /// <summary>
        /// 图文消息个数，限制为10条以内
        /// </summary>
        public int ArticleCount { get; set; }
        /// <summary>
        /// 多条图文消息信息，默认第一个item为大图,注意，如果图文数超过10，则将会无响应
        /// </summary>
        public NewsArticle[] Articles { get; set; }

        public class NewsArticle
        {
            /// <summary>
            /// 可选, 图文消息标题
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 可选, 图文消息描述
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// 可选, 图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
            /// </summary>
            public string PicUrl { get; set; }
            /// <summary>
            /// 点击图文消息跳转链接
            /// </summary>
            public string Url { get; set; }
        }

        public override ResponseMessageType MsgType
        {
            get { return ResponseMessageType.news; }
        }

        protected override XElement[] SerializeElements()
        {
            return new[]
            {
                new XElement("ArticleCount", ArticleCount),
                new XElement("Articles", Articles.Select(x => new XElement("item",
                    new XElement("Title", x.Title),
                    new XElement("Description", x.Description),
                    new XElement("PicUrl", x.PicUrl),
                    new XElement("Url", x.Url)))),
            };
        }
    }

    /// <summary>
    /// 消息转发到客服
    /// </summary>
    public class TransferCustomerServiceResponseMessage : ResponseMessage
    {
        /// <summary>
        /// 可选, 消息转发到指定客服
        /// </summary>
        public TransKfInfo TransInfo { get; set; }

        public class TransKfInfo
        {
            /// <summary>
            /// 指定会话接入的客服账号
            /// </summary>
            public string KfAccount { get; set; }
        }

        public override ResponseMessageType MsgType
        {
            get { return ResponseMessageType.transfer_customer_service; }
        }

        protected override XElement[] SerializeElements()
        {
            return new[]
            {
                new XElement("TransInfo",
                    new XElement("KfAccount", TransInfo?.KfAccount)),
            };
        }
    }
}