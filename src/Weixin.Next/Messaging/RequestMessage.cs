namespace Weixin.Next.Messaging
{
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
        public string MsgType { get; set; }
        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public long MsgId { get; set; }
    }


    /// <summary>
    /// text 文本消息
    /// </summary>
    public class TextRequestMessage : RequestMessage
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
    }

    /// <summary>
    /// image 图片消息
    /// </summary>
    public class ImageRequestMessage : RequestMessage
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
    public class VoiceRequestMessage : RequestMessage
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
    public class VideoRequestMessage : RequestMessage
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
    public class ShortVideoRequestMessage : RequestMessage
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
    public class LocationRequstMessage : RequestMessage
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
    public class LinkRequestMessage : RequestMessage
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
