using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信多媒体接口
    /// </summary>
    public static class Media
    {
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="filename">图片文件名</param>
        /// <param name="dataStream">图片数据流</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<UploadImageResult> UploadImage(string filename, Stream dataStream, ApiConfig config = null)
        {
            return ApiHelper.UploadResult<UploadImageResult>("https://api.weixin.qq.com/cgi-bin/media/uploadimg?$acac$", "media", filename, dataStream, null, config);
        }

        public class UploadImageResult : IApiResult
        {
            /// <summary>
            /// 使用图片的网址
            /// </summary>
            public string url { get; set; }
        }

        /// <summary>
        /// 新增临时素材. 临时素材只保留 3 天
        /// </summary>
        /// <param name="type">媒体文件类型，分别有图片（image）、语音（voice）、视频（video）和缩略图（thumb）</param>
        /// <param name="filename">文件名</param>
        /// <param name="dataStream">数据流</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<UploadResult> Upload(string type, string filename, Stream dataStream, ApiConfig config = null)
        {
            return ApiHelper.UploadResult<UploadResult>($"https://api.weixin.qq.com/cgi-bin/media/upload?$acac$&type={Uri.EscapeDataString(type)}", "media", filename, dataStream, null, config);
        }

        public class UploadResult : IApiResult
        {
            /// <summary>
            /// 媒体文件类型，分别有图片（image）、语音（voice）、视频（video）和缩略图（thumb，主要用于视频与音乐格式的缩略图）
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 媒体文件上传后，获取时的唯一标识
            /// </summary>
            public string media_id { get; set; }
            /// <summary>
            /// 媒体文件上传时间戳
            /// </summary>
            public long created_at { get; set; }
        }

        /// <summary>
        /// 下载临时素材
        /// </summary>
        /// <param name="media_id">媒体文件ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<Stream> Get(string media_id, ApiConfig config = null)
        {
            using (var s = await ApiHelper.GetStream($"https://api.weixin.qq.com/cgi-bin/media/get?$acac$&media_id={Uri.EscapeDataString(media_id)}", config).ConfigureAwait(false))
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
    }
}
