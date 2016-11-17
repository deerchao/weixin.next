using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信消息发送
    /// </summary>
    public static class Message
    {
        #region 发送客服消息
        /// <summary>
        /// 客服接口-发消息-文本
        /// </summary>
        /// <param name="touser">接收者的 openid</param>
        /// <param name="content">文本内容</param>
        /// <param name="kf_account">客服账号, 提供了将会以此客服的身份进行发送</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task SendCustomText(string touser, string content, string kf_account = null, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/message/custom/send?$acac$", new { touser, msgtype = "text", text = new { content }, customservice = new { kf_account } }, config);
        }

        /// <summary>
        /// 客服接口-发消息-图片
        /// </summary>
        /// <param name="touser">接收者的 openid</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="kf_account">客服账号, 提供了将会以此客服的身份进行发送</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task SendCustomImage(string touser, string media_id, string kf_account = null, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/message/custom/send?$acac$", new { touser, msgtype = "image", image = new { media_id }, customservice = new { kf_account } }, config);
        }

        /// <summary>
        /// 客服接口-发消息-语音
        /// </summary>
        /// <param name="touser">接收者的 openid</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="kf_account">客服账号, 提供了将会以此客服的身份进行发送</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task SendCustomVoice(string touser, string media_id, string kf_account = null, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/message/custom/send?$acac$", new { touser, msgtype = "voice", voice = new { media_id }, customservice = new { kf_account } }, config);
        }

        /// <summary>
        /// 客服接口-发消息-视频
        /// </summary>
        /// <param name="touser">接收者的 openid</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="thumb_media_id">缩略图媒体ID</param>
        /// <param name="title">标题</param>
        /// <param name="description">说明</param>
        /// <param name="kf_account">客服账号, 提供了将会以此客服的身份进行发送</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task SendCustomVideo(string touser, string media_id, string thumb_media_id, string title, string description, string kf_account = null, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/message/custom/send?$acac$", new { touser, msgtype = "video", video = new { media_id, thumb_media_id, title, description }, customservice = new { kf_account } }, config);
        }

        /// <summary>
        /// 客服接口-发消息-音乐
        /// </summary>
        /// <param name="touser">接收者的 openid</param>
        /// <param name="musicurl">音乐链接</param>
        /// <param name="hqmusicurl">高品质音乐链接，wifi环境优先使用该链接播放音乐</param>
        /// <param name="thumb_media_id">缩略图媒体ID</param>
        /// <param name="title">标题</param>
        /// <param name="description">说明</param>
        /// <param name="kf_account">客服账号, 提供了将会以此客服的身份进行发送</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task SendCustomMusic(string touser, string musicurl, string hqmusicurl, string thumb_media_id, string title, string description, string kf_account = null, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/message/custom/send?$acac$", new { touser, msgtype = "music", music = new { title, description, musicurl, hqmusicurl, thumb_media_id }, customservice = new { kf_account } }, config);
        }

        /// <summary>
        /// 客服接口-发消息-外链图文
        /// </summary>
        /// <param name="touser">接收者的 openid</param>
        /// <param name="articles">文章列表, 8 条以内</param>
        /// <param name="kf_account">客服账号, 提供了将会以此客服的身份进行发送</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task SendCustomNews(string touser, NewsArticle[] articles, string kf_account = null, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/message/custom/send?$acac$", new { touser, msgtype = "news", news = new { articles }, customservice = new { kf_account } }, config);
        }

        public class NewsArticle
        {
            /// <summary>
            /// 文章标题
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 文章说明
            /// </summary>
            public string description { get; set; }
            /// <summary>
            /// 文章网址
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// 图片网址
            /// </summary>
            public string picurl { get; set; }
        }

        /// <summary>
        /// 客服接口-发消息-图文
        /// </summary>
        /// <param name="touser">接收者的 openid</param>
        /// <param name="media_id">素材媒体id</param>
        /// <param name="kf_account">客服账号, 提供了将会以此客服的身份进行发送</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task SendCustomMpNews(string touser, string media_id, string kf_account = null, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/message/custom/send?$acac$", new { touser, msgtype = "mpnews", mpnews = new { media_id }, customservice = new { kf_account } }, config);
        }

        /// <summary>
        /// 客服接口-发消息-卡券 (客服消息接口投放卡券仅支持非自定义Code码和导入code模式的卡券的卡券)
        /// </summary>
        /// <param name="touser">接收者的 openid</param>
        /// <param name="card_id">卡券id</param>
        /// <param name="kf_account">客服账号, 提供了将会以此客服的身份进行发送</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task SendCustomWxCard(string touser, string card_id, string kf_account = null, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/message/custom/send?$acac$", new { touser, msgtype = "wxcard", wxcard = new { card_id }, customservice = new { kf_account } }, config);
        }
        #endregion

        #region 按标签群发消息
        /// <summary>
        /// 根据标签进行群发-文本
        /// </summary>
        /// <param name="tag_id">接收者的标签id, 如果为 null, 则代表所有人(is_to_all=true)</param>
        /// <param name="content">文本内容</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendResult> SendMassAllText(int? tag_id, string content, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/sendall?$acac$", new { filter = new { is_to_all = tag_id == null, tag_id }, msgtype = "text", text = new { content } }, config);
        }

        /// <summary>
        /// 根据标签进行群发-图片
        /// </summary>
        /// <param name="tag_id">接收者的标签id, 如果为 null, 则代表所有人(is_to_all=true)</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendResult> SendMassAllImage(int? tag_id, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/sendall?$acac$", new { filter = new { is_to_all = tag_id == null, tag_id }, msgtype = "image", image = new { media_id } }, config);
        }

        /// <summary>
        /// 根据标签进行群发-语音
        /// </summary>
        /// <param name="tag_id">接收者的标签id, 如果为 null, 则代表所有人(is_to_all=true)</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendResult> SendMassAllVoice(int? tag_id, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/sendall?$acac$", new { filter = new { is_to_all = tag_id == null, tag_id }, msgtype = "voice", voice = new { media_id } }, config);
        }

        /// <summary>
        /// 根据标签进行群发-视频
        /// </summary>
        /// <param name="tag_id">接收者的标签id, 如果为 null, 则代表所有人(is_to_all=true)</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendResult> SendMassAllMpVideo(int? tag_id, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/sendall?$acac$", new { filter = new { is_to_all = tag_id == null, tag_id }, msgtype = "mpvideo", mpvideo = new { media_id } }, config);
        }

        /// <summary>
        /// 根据标签进行群发-图文
        /// </summary>
        /// <param name="tag_id">接收者的标签id, 如果为 null, 则代表所有人(is_to_all=true)</param>
        /// <param name="media_id">素材媒体id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendNewsResult> SendMassAllMpNews(int? tag_id, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendNewsResult>("https://api.weixin.qq.com/cgi-bin/message/mass/sendall?$acac$", new { filter = new { is_to_all = tag_id == null, tag_id }, msgtype = "mpnews", mpnews = new { media_id } }, config);
        }

        /// <summary>
        /// 根据标签进行群发-卡券 (客服消息接口投放卡券仅支持非自定义Code码和导入code模式的卡券的卡券)
        /// </summary>
        /// <param name="tag_id">接收者的标签id, 如果为 null, 则代表所有人(is_to_all=true)</param>
        /// <param name="card_id">卡券id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendResult> SendMassAllWxCard(int? tag_id, string card_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/sendall?$acac$", new { filter = new { is_to_all = tag_id == null, tag_id }, msgtype = "wxcard", wxcard = new { card_id } }, config);
        }
        #endregion

        public class MassSendResult : IApiResult
        {
            /// <summary>
            /// 消息发送任务的ID
            /// </summary>
            public long msg_id { get; set; }
        }

        public class MassSendNewsResult : MassSendResult
        {
            /// <summary>
            /// 消息的数据ID，可以用于在图文分析数据接口中，获取到对应的图文消息的数据，是图文分析数据接口中的msgid字段中的前半部分
            /// </summary>
            public long msg_data_id { get; set; }
        }

        #region 根据OpenID列表群发
        /// <summary>
        /// 根据OpenID列表群发-文本
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="content">文本内容</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendResult> SendMassText(string[] touser, string content, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/send?$acac$", new { touser, msgtype = "text", text = new { content } }, config);
        }

        /// <summary>
        /// 根据OpenID列表群发-图片
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendResult> SendMassImage(string[] touser, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/send?$acac$", new { touser, msgtype = "image", image = new { media_id } }, config);
        }

        /// <summary>
        /// 根据OpenID列表群发-语音
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendResult> SendMassVoice(string[] touser, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/send?$acac$", new { touser, msgtype = "voice", voice = new { media_id } }, config);
        }

        /// <summary>
        /// 根据OpenID列表群发-视频
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendResult> SendMassMpVideo(string[] touser, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/send?$acac$", new { touser, msgtype = "mpvideo", mpvideo = new { media_id } }, config);
        }

        /// <summary>
        /// 根据OpenID列表群发-图文
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="media_id">素材媒体id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendNewsResult> SendMassMpNews(string[] touser, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendNewsResult>("https://api.weixin.qq.com/cgi-bin/message/mass/send?$acac$", new { touser, msgtype = "mpnews", mpnews = new { media_id } }, config);
        }

        /// <summary>
        /// 根据OpenID列表群发-卡券 (客服消息接口投放卡券仅支持非自定义Code码和导入code模式的卡券的卡券)
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="card_id">卡券id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassSendResult> SendMassWxCard(string[] touser, string card_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/send?$acac$", new { touser, msgtype = "wxcard", wxcard = new { card_id } }, config);
        }
        #endregion

        /// <summary>
        /// 删除群发
        /// </summary>
        /// <param name="msg_id">消息id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task DeleteMassSend(long msg_id, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/message/mass/delete?$acac$", new { msg_id }, config);
        }

        #region 预览群发消息

        /// <summary>
        /// 预览群发消息-文本
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="towxname">接收者的微信ID, 如与 openid 冲突, 会使用微信ID</param>
        /// <param name="content">文本内容</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<PreviewSendResult> SendPreviewText(string touser, string towxname, string content, ApiConfig config = null)
        {
            return ApiHelper.PostResult<PreviewSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/preview?$acac$", new { touser, towxname, msgtype = "text", text = new { content } }, config);
        }

        /// <summary>
        /// 预览群发消息-图片
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="towxname">接收者的微信ID, 如与 openid 冲突, 会使用微信ID</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<PreviewSendResult> SendPreviewImage(string touser, string towxname, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<PreviewSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/preview?$acac$", new { touser, towxname, msgtype = "image", image = new { media_id } }, config);
        }

        /// <summary>
        /// 预览群发消息-语音
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="towxname">接收者的微信ID, 如与 openid 冲突, 会使用微信ID</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<PreviewSendResult> SendPreviewVoice(string touser, string towxname, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<PreviewSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/preview?$acac$", new { touser, towxname, msgtype = "voice", voice = new { media_id } }, config);
        }

        /// <summary>
        /// 预览群发消息-视频
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="towxname">接收者的微信ID, 如与 openid 冲突, 会使用微信ID</param>
        /// <param name="media_id">素材的媒体ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<PreviewSendResult> SendPreviewMpVideo(string touser, string towxname, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<PreviewSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/preview?$acac$", new { touser, towxname, msgtype = "mpvideo", mpvideo = new { media_id } }, config);
        }

        /// <summary>
        /// 预览群发消息-图文
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="towxname">接收者的微信ID, 如与 openid 冲突, 会使用微信ID</param>
        /// <param name="media_id">素材媒体id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<PreviewSendResult> SendPreviewMpNews(string touser, string towxname, string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<PreviewSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/preview?$acac$", new { touser, towxname, msgtype = "mpnews", mpnews = new { media_id } }, config);
        }

        /// <summary>
        /// 预览群发消息-卡券 (客服消息接口投放卡券仅支持非自定义Code码和导入code模式的卡券的卡券)
        /// </summary>
        /// <param name="touser">接收者的 openid 列表</param>
        /// <param name="towxname">接收者的微信ID, 如与 openid 冲突, 会使用微信ID</param>
        /// <param name="card_id">卡券id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<PreviewSendResult> SendPreviewWxCard(string touser, string towxname, string card_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<PreviewSendResult>("https://api.weixin.qq.com/cgi-bin/message/mass/preview?$acac$", new { touser, towxname, msgtype = "wxcard", wxcard = new { card_id } }, config);
        }

        public class PreviewSendResult : IApiResult
        {
            /// <summary>
            /// 消息ID
            /// </summary>
            public long msg_id { get; set; }
        }
        #endregion

        /// <summary>
        /// 查询群发消息发送状态
        /// </summary>
        /// <param name="msg_id">消息id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<MassGetStatusResult> GetMassSendStatus(long msg_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<MassGetStatusResult>("https://api.weixin.qq.com/cgi-bin/message/mass/get?$acac$", new { msg_id }, config);
        }

        public class MassGetStatusResult : IApiResult
        {
            /// <summary>
            /// 群发消息后返回的消息id
            /// </summary>
            public int msg_id { get; set; }
            /// <summary>
            /// 消息发送后的状态，SEND_SUCCESS 表示发送成功
            /// </summary>
            public string msg_status { get; set; }

            public bool IsSuccess()
            {
                return msg_status == "SEND_SUCCESS";
            }
        }

    }
}
