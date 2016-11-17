using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信 jsapi_ticket
    /// </summary>
    public static class Ticket
    {
        /// <summary>
        /// 获取 jsapi_ticket
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<TokenResult> GetJsapi(ApiConfig config = null)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?$acac$&type=jsapi";
            return ApiHelper.GetResult<TokenResult>(url, config);
        }

        public class TokenResult : IApiResult
        {
            /// <summary>
            /// 用于调用微信JS接口的临时票据, 正常情况下，jsapi_ticket的有效期为7200秒
            /// </summary>
            public string ticket { get; set; }

            /// <summary>
            /// 凭证有效时间，单位：秒
            /// </summary>
            public int expires_in { get; set; }
        }
    }
}
