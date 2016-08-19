using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Weixin.Next.Api
{
    public static class Material
    {
        /// <summary>
        /// 添加永久图片素材
        /// </summary>
        /// <param name="filename">图片文件名</param>
        /// <param name="dataStream">图片数据流</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<AddImageResult> AddImage(string filename, Stream dataStream, ApiConfig config = null)
        {
            return ApiHelper.UploadResult<AddImageResult>("https://api.weixin.qq.com/cgi-bin/material/add_material?$acac$&type=image", "media", filename, dataStream, null, config);
        }

        /// <summary>
        /// 添加永久语音素材
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="dataStream">数据流</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<AddResult> AddVoice(string filename, Stream dataStream, ApiConfig config = null)
        {
            return ApiHelper.UploadResult<AddResult>("https://api.weixin.qq.com/cgi-bin/material/add_material?$acac$&type=voice", "media", filename, dataStream, null, config);
        }

        /// <summary>
        /// 添加永久缩略图素材
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="dataStream">数据流</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<AddResult> AddThumb(string filename, Stream dataStream, ApiConfig config = null)
        {
            return ApiHelper.UploadResult<AddResult>("https://api.weixin.qq.com/cgi-bin/material/add_material?$acac$&type=thumb", "media", filename, dataStream, null, config);
        }

        /// <summary>
        /// 添加永久视频素材
        /// </summary>
        /// <param name="title">视频素材的标题</param>
        /// <param name="description">视频素材的描述</param>
        /// <param name="filename">文件名</param>
        /// <param name="dataStream">数据流</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<AddResult> AddVideo(string title, string description, string filename, Stream dataStream, ApiConfig config = null)
        {
            var additionalFields = new[]
            {
                new KeyValuePair<string, string>("description", ApiHelper.ToJsonString(new {title, description}, config))
            };
            return ApiHelper.UploadResult<AddResult>("https://api.weixin.qq.com/cgi-bin/material/add_material?$acac$&type=thumb", "media", filename, dataStream, additionalFields, config);
        }

        public class AddResult : IApiResult
        {
            /// <summary>
            /// 新增的永久素材的media_id
            /// </summary>
            public string media_id { get; set; }
        }

        public class AddImageResult : AddResult
        {
            /// <summary>
            /// 新增的图片素材的图片URL
            /// </summary>
            public string url { get; set; }
        }

        /// <summary>
        /// 添加永久图文消息素材
        /// </summary>
        /// <param name="articles"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<AddResult> AddNews(NewsArticle[] articles, ApiConfig config = null)
        {
            return ApiHelper.PostResult<AddResult>("https://api.weixin.qq.com/cgi-bin/material/add_news?$acac$", new { articles }, config);
        }

        public class NewsArticle
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 图文消息的封面图片素材id（必须是永久mediaID）
            /// </summary>
            public string thumb_media_id { get; set; }
            /// <summary>
            /// 作者
            /// </summary>
            public string author { get; set; }
            /// <summary>
            /// 图文消息的摘要，仅有单图文消息才有摘要，多图文此处为空
            /// </summary>
            public string digest { get; set; }
            /// <summary>
            /// 是否显示封面，0为false，即不显示，1为true，即显示
            /// </summary>
            public string show_cover_pic { get; set; }
            /// <summary>
            /// 图文消息的具体内容，支持HTML标签，必须少于2万字符，小于1M，且此处会去除JS
            /// </summary>
            public string content { get; set; }
            /// <summary>
            /// 图文消息的原文地址，即点击“阅读原文”后的URL
            /// </summary>
            public string content_source_url { get; set; }
        }

        //todo 获取素材列表
    }
}
