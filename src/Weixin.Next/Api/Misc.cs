using System.Threading.Tasks;

namespace Weixin.Next.Api
{
    // ReSharper disable InconsistentNaming
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
    }
}
