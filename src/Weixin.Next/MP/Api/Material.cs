using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信素材接口
    /// </summary>
    public static class Material
    {
        #region 添加永久素材
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

        #endregion

        #region 修改永久素材

        /// <summary>
        /// 修改永久图文素材
        /// </summary>
        /// <param name="media_id">要修改的图文消息的id</param>
        /// <param name="index">要更新的文章在图文消息中的位置（多图文消息时，此字段才有意义），第一篇为0</param>
        /// <param name="article">文章信息</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task UpdateNews(string media_id, int index, NewsArticle article, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/material/update_news?$acac$", new { media_id, index, articles = article }, config);
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

        #endregion

        #region 删除永久素材
        /// <summary>
        /// 删除永久素材
        /// </summary>
        /// <param name="media_id">要删除的素材的media_id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task Delete(string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/material/del_material?$acac$", new { media_id }, config);
        }
        #endregion

        #region 获取永久素材

        /// <summary>
        /// 下载永久图片素材
        /// </summary>
        /// <param name="media_id">媒体文件ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<Stream> GetImage(string media_id, ApiConfig config = null)
        {
            using (var s = await ApiHelper.PostStream("https://api.weixin.qq.com/cgi-bin/material/get_material?$acac$", new { media_id }, config).ConfigureAwait(false))
            {
                var ms = new MemoryStream();
                await s.CopyToAsync(ms).ConfigureAwait(false);

                //估计错误消息应该不会大于400字节
                if (ms.Length < 400)
                {
                    var buffer = ms.ToArray();
                    var text = Encoding.UTF8.GetString(buffer);

                    //如果是失败消息, 这里会抛出异常
                    ApiHelper.BuildVoid(text, config);
                }

                ms.Position = 0;
                return ms;
            }
        }

        /// <summary>
        /// 下载永久语音素材
        /// </summary>
        /// <param name="media_id">媒体文件ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<Stream> GetVoice(string media_id, ApiConfig config = null)
        {
            using (var s = await ApiHelper.PostStream("https://api.weixin.qq.com/cgi-bin/material/get_material?$acac$", new { media_id }, config).ConfigureAwait(false))
            {
                var ms = new MemoryStream();
                await s.CopyToAsync(ms).ConfigureAwait(false);

                //估计错误消息应该不会大于400字节
                if (ms.Length < 400)
                {
                    var buffer = ms.ToArray();
                    var text = Encoding.UTF8.GetString(buffer);

                    //如果是失败消息, 这里会抛出异常
                    ApiHelper.BuildVoid(text, config);
                }

                ms.Position = 0;
                return ms;
            }
        }

        /// <summary>
        /// 下载永久视频素材
        /// </summary>
        /// <param name="media_id">媒体文件ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetVideoResult> GetVideo(string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetVideoResult>("https://api.weixin.qq.com/cgi-bin/material/get_material?$acac$", new { media_id }, config);
        }

        public class GetVideoResult : IApiResult
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 说明
            /// </summary>
            public string description { get; set; }
            /// <summary>
            /// 下载视频的网址
            /// </summary>
            public string down_url { get; set; }
        }

        /// <summary>
        /// 下载永久图文素材
        /// </summary>
        /// <param name="media_id">媒体文件ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetNewsResult> GetNews(string media_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetNewsResult>("https://api.weixin.qq.com/cgi-bin/material/get_material?$acac$", new { media_id }, config);
        }

        public class GetNewsResult : IApiResult
        {
            public NewsArticle[] news_item { get; set; }
        }

        #endregion

        #region 获取永久素材列表
        /// <summary>
        /// 获取永久图文消息素材列表(包含公众号在公众平台官网素材管理模块中新建的素材)
        /// </summary>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 返回</param>
        /// <param name="count">返回素材的数量，取值在1到20之间</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<BatchGetNewsResult> BatchGetNews(int offset, int count, ApiConfig config = null)
        {
            return ApiHelper.PostResult<BatchGetNewsResult>("https://api.weixin.qq.com/cgi-bin/material/batchget_material?$acac$", new { type = "news", offset, count }, config);
        }

        /// <summary>
        /// 获取永久图片消息素材列表(包含公众号在公众平台官网素材管理模块中新建的素材)
        /// </summary>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 返回</param>
        /// <param name="count">返回素材的数量，取值在1到20之间</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<BatchGetImageResult> BatchGetImage(int offset, int count, ApiConfig config = null)
        {
            return ApiHelper.PostResult<BatchGetImageResult>("https://api.weixin.qq.com/cgi-bin/material/batchget_material?$acac$", new { type = "image", offset, count }, config);
        }

        /// <summary>
        /// 获取永久视频消息素材列表(包含公众号在公众平台官网素材管理模块中新建的素材)
        /// </summary>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 返回</param>
        /// <param name="count">返回素材的数量，取值在1到20之间</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<BatchGetResult> BatchGetVideo(int offset, int count, ApiConfig config = null)
        {
            return ApiHelper.PostResult<BatchGetResult>("https://api.weixin.qq.com/cgi-bin/material/batchget_material?$acac$", new { type = "video", offset, count }, config);
        }

        /// <summary>
        /// 获取永久语音消息素材列表(包含公众号在公众平台官网素材管理模块中新建的素材)
        /// </summary>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 返回</param>
        /// <param name="count">返回素材的数量，取值在1到20之间</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<BatchGetResult> BatchGetVoice(int offset, int count, ApiConfig config = null)
        {
            return ApiHelper.PostResult<BatchGetResult>("https://api.weixin.qq.com/cgi-bin/material/batchget_material?$acac$", new { type = "voice", offset, count }, config);
        }


        public class BatchGetResult : IApiResult
        {
            /// <summary>
            /// 该类型的素材的总数
            /// </summary>
            public int total_count { get; set; }
            /// <summary>
            /// 本次调用获取的素材的数量
            /// </summary>
            public int item_count { get; set; }
            /// <summary>
            /// 素材信息
            /// </summary>
            public MediaItem[] item { get; set; }
        }

        public class BatchGetImageResult : IApiResult
        {
            /// <summary>
            /// 该类型的素材的总数
            /// </summary>
            public int total_count { get; set; }
            /// <summary>
            /// 本次调用获取的素材的数量
            /// </summary>
            public int item_count { get; set; }
            /// <summary>
            /// 素材信息
            /// </summary>
            public ImageItem[] item { get; set; }
        }

        public class BatchGetNewsResult : IApiResult
        {
            /// <summary>
            /// 该类型的素材的总数
            /// </summary>
            public int total_count { get; set; }
            /// <summary>
            /// 本次调用获取的素材的数量
            /// </summary>
            public int item_count { get; set; }
            /// <summary>
            /// 素材信息
            /// </summary>
            public NewsItem[] item { get; set; }
        }

        public class MediaItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string media_id { get; set; }
            /// <summary>
            /// 文件名称
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 最后更新时间
            /// </summary>
            public long update_time { get; set; }
        }

        public class ImageItem : MediaItem
        {
            /// <summary>
            /// 图片网址
            /// </summary>
            public string url { get; set; }
        }

        public class NewsItem : NewsArticle
        {
            /// <summary>
            /// 图文页的URL
            /// </summary>
            public string url { get; set; }
        }

        /// <summary>
        /// 获取素材总数
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetCountResult> GetCount(ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetCountResult>("https://api.weixin.qq.com/cgi-bin/material/get_materialcount?$acac$", config);
        }

        public class GetCountResult : IApiResult
        {
            /// <summary>
            /// 语音总数量
            /// </summary>
            public int voice_count { get; set; }
            /// <summary>
            /// 视频总数量
            /// </summary>
            public int video_count { get; set; }
            /// <summary>
            /// 图片总数量
            /// </summary>
            public int image_count { get; set; }
            /// <summary>
            /// 图文总数量
            /// </summary>
            public int news_count { get; set; }
        }
        #endregion
    }
}
