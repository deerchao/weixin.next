using System;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信接口 access_token
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 获取 access_token, 注意获取后之前获取的将会失效
        /// </summary>
        /// <param name="appId">微信应用ID</param>
        /// <param name="appSecret">微信应用密码</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<TokenResult> Get(string appId, string appSecret, ApiConfig config = null)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={Uri.EscapeDataString(appId)}&secret={Uri.EscapeDataString(appSecret)}";
            var text = await ApiHelper.GetString(url, config).ConfigureAwait(false);
            return ApiHelper.BuildResult<TokenResult>(text);
        }

        public class TokenResult : IApiResult
        {
            /// <summary>
            /// 获取到的凭证
            /// </summary>
            public string access_token { get; set; }

            /// <summary>
            /// 凭证有效时间，单位：秒
            /// </summary>
            public int expires_in { get; set; }
        }
    }
}