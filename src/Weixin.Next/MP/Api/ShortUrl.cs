using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信短网址
    /// </summary>
    public static class ShortUrl
    {
        /// <summary>
        /// 将一条长链接转成短链接
        /// </summary>
        /// <param name="long_url">长链接</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<CreateResult> Create(string long_url, ApiConfig config = null)
        {
            return ApiHelper.PostResult<CreateResult>("https://api.weixin.qq.com/cgi-bin/shorturl?$acac$", new
            {
                action = "long2short",
                long_url,
            }, config);
        }

        public class CreateResult : IApiResult
        {
            /// <summary>
            /// 短链接
            /// </summary>
            public string short_url { get; set; }
        }
    }
}
