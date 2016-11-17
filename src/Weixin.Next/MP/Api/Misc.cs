using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信其它接口
    /// </summary>
    public static class Misc
    {
        /// <summary>
        /// 获取微信服务器IP地址
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetCallbackIpResult> GetCallbackIp(ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetCallbackIpResult>("https://api.weixin.qq.com/cgi-bin/getcallbackip?$acac$", config);
        }

        public class GetCallbackIpResult : IApiResult
        {
            /// <summary>
            /// 微信服务器IP地址列表
            /// </summary>
            public string[] ip_list { get; set; }
        }

        #region 获取公众号的自动回复规则
        /// <summary>
        /// 获取公众号的自动回复规则
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetCurrentAutoReplyInfoResult> GetCurrentAutoReplyInfo(ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetCurrentAutoReplyInfoResult>("https://api.weixin.qq.com/cgi-bin/get_current_autoreply_info?$acac$", config);
        }

        public class GetCurrentAutoReplyInfoResult : IApiResult
        {
            /// <summary>
            /// 关注后自动回复是否开启，0代表未开启，1代表开启
            /// </summary>
            public int is_add_friend_reply_open { get; set; }
            /// <summary>
            /// 消息自动回复是否开启，0代表未开启，1代表开启
            /// </summary>
            public int is_autoreply_open { get; set; }
            /// <summary>
            /// 关注后自动回复的信息
            /// </summary>
            public Autoreply_Info add_friend_autoreply_info { get; set; }
            /// <summary>
            /// 消息自动回复的信息
            /// </summary>
            public Autoreply_Info message_default_autoreply_info { get; set; }
            /// <summary>
            /// 关键词自动回复的信息
            /// </summary>
            public Keyword_Autoreply_Info keyword_autoreply_info { get; set; }
        }

        public class Autoreply_Info
        {
            /// <summary>
            /// 自动回复的类型。关注后自动回复和消息自动回复的类型仅支持文本（text）、图片（img）、语音（voice）、视频（video），关键词自动回复则还多了图文消息（news）
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 对于文本类型，content是文本内容，对于图文、图片、语音、视频类型，content是mediaID
            /// </summary>
            public string content { get; set; }
        }

        public class Keyword_Autoreply_Info
        {
            public Keyword_AutoReply_Rule[] list { get; set; }
        }

        public class Keyword_AutoReply_Rule
        {
            /// <summary>
            /// 规则名称
            /// </summary>
            public string rule_name { get; set; }
            /// <summary>
            /// 创建时间, 时间戳
            /// </summary>
            public long create_time { get; set; }
            /// <summary>
            /// 回复模式，reply_all代表全部回复，random_one代表随机回复其中一条
            /// </summary>
            public string reply_mode { get; set; }
            /// <summary>
            /// 匹配的关键词列表
            /// </summary>
            public Keyword_List_Info[] keyword_list_info { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Reply_List_Info[] reply_list_info { get; set; }
        }

        public class Keyword_List_Info : Autoreply_Info
        {
            /// <summary>
            /// 匹配模式，contain代表消息中含有该关键词即可，equal表示消息内容必须和关键词严格相同
            /// </summary>
            public string match_mode { get; set; }
        }

        public class Reply_List_Info : Autoreply_Info
        {
            /// <summary>
            /// 图文消息的信息, 仅在 type 为 news 时才会有
            /// </summary>
            public News_Info news_info { get; set; }
        }

        public class News_Info
        {
            public News_Item[] list { get; set; }
        }

        public class News_Item
        {
            /// <summary>
            /// 图文消息的标题
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 作者
            /// </summary>
            public string author { get; set; }
            /// <summary>
            /// 摘要
            /// </summary>
            public string digest { get; set; }
            /// <summary>
            /// 是否显示封面，0为不显示，1为显示
            /// </summary>
            public int show_cover { get; set; }
            /// <summary>
            /// 封面图片的URL
            /// </summary>
            public string cover_url { get; set; }
            /// <summary>
            /// 正文的URL
            /// </summary>
            public string content_url { get; set; }
            /// <summary>
            /// 原文的URL，若置空则无查看原文入口
            /// </summary>
            public string source_url { get; set; }
        }
        #endregion
    }
}
