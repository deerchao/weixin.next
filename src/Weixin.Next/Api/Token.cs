using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Weixin.Next.Api
{
    public class Token
    {
        /// <summary>
        /// 获取 access_token, 注意获取后之前获取的将会失效
        /// </summary>
        /// <param name="appId">微信应用ID</param>
        /// <param name="appSecret">微信应用密码</param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static async Task<TokenResult> Retrieve(string appId, string appSecret, HttpClient httpClient = null)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={Uri.EscapeDataString(appId)}&secret={Uri.EscapeDataString(appSecret)}";
            var http = httpClient ?? new HttpClient();
            var text = await http.GetStringAsync(url).ConfigureAwait(false);
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