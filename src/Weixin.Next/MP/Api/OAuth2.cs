using System;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信网页授权
    /// </summary>
    public static class OAuth2
    {
        /// <summary>
        /// 授权作用域之一, 只能获取用户 openid
        /// </summary>
        public const string ScopeBase = "snsapi_base";
        /// <summary>
        /// 授权作用域之一, 能获取用户 openid, 昵称, 性别, 所在地等
        /// </summary>
        public const string ScopeUserInfo = "snsapi_userinfo";

        /// <summary>
        /// 获取网页授权网址. 将用户导入此网址后, 如果用户同意授权，页面将跳转至 redirect_uri/?code=&lt;code&gt;&amp;state=<paramref name="state"/>
        /// </summary>
        /// <param name="appId">微信应用id, 公众号的唯一标识</param>
        /// <param name="redirect_uri">授权后返回网址</param>
        /// <param name="scope">应用授权作用域，snsapi_base （不弹出授权页面，直接跳转，只能获取用户openid），snsapi_userinfo （弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息）</param>
        /// <param name="state">重定向后会带上state参数，开发者可以填写a-zA-Z0-9的参数值，最多128字节</param>
        /// <returns></returns>
        public static string GetAuthorizeUrl(string appId, string redirect_uri, string scope, string state = null)
        {
            return "https://open.weixin.qq.com/connect/oauth2/authorize?" +
                   $"appid={Uri.EscapeDataString(appId)}&" +
                   $"redirect_uri={Uri.EscapeDataString(redirect_uri)}&" +
                   "response_type=code&" +
                   $"scope={Uri.EscapeDataString(scope)}&" +
                   $"state={Uri.EscapeDataString(state ?? "")}#" +
                   "wechat_redirect";
        }

        /// <summary>
        /// <para>通过授权 code 换取网页授权 access_token</para>
        /// </summary>
        /// <param name="appid">公众号的唯一标识</param>
        /// <param name="secret">公众号的appsecret</param>
        /// <param name="code">填写客户端返回的code参数</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<AccessTokenResult> GetAccessToken(string appid, string secret, string code, ApiConfig config = null)
        {
            var url = "https://api.weixin.qq.com/sns/oauth2/access_token?" +
                      $"appid={Uri.EscapeDataString(appid)}&" +
                      $"secret={Uri.EscapeDataString(secret)}&" +
                      $"code={Uri.EscapeDataString(code)}&" +
                      "grant_type=authorization_code";

            var response = await ApiHelper.GetString(url, config).ConfigureAwait(false);
            return ApiHelper.BuildResult<AccessTokenResult>(response);
        }

        /// <summary>
        /// <para>刷新access_token</para>
        /// </summary>
        /// <param name="appid">公众号的唯一标识</param>
        /// <param name="refresh_token">填写通过access_token获取到的refresh_token参数</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<AccessTokenResult> RefreshAccessToken(string appid, string refresh_token, ApiConfig config = null)
        {
            var url = "https://api.weixin.qq.com/sns/oauth2/refresh_token?" +
                      $"appid={Uri.EscapeDataString(appid)}&" +
                      "grant_type=refresh_token&" +
                      $"refresh_token={Uri.EscapeDataString(refresh_token)}";

            var response = await ApiHelper.GetString(url, config).ConfigureAwait(false);
            return ApiHelper.BuildResult<AccessTokenResult>(response);
        }

        public class AccessTokenResult : IApiResult
        {
            /// <summary>
            /// 网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同
            /// </summary>
            public string access_token { get; set; }
            /// <summary>
            /// access_token接口调用凭证超时时间，单位（秒）
            /// </summary>
            public int expires_in { get; set; }
            /// <summary>
            /// 用户刷新access_token
            /// </summary>
            public string refresh_token { get; set; }
            /// <summary>
            /// 用户唯一标识，请注意，在未关注公众号时，用户访问公众号的网页，也会产生一个用户和公众号唯一的OpenID
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 用户授权的作用域，使用逗号（,）分隔
            /// </summary>
            public string scope { get; set; }
        }

        /// <summary>
        /// 拉取用户信息(需scope为 snsapi_userinfo)
        /// </summary>
        /// <param name="openid">用户的唯一标识</param>
        /// <param name="access_token">网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同</param>
        /// <param name="lang">返回国家地区语言版本，zh_CN 简体，zh_TW 繁体，en 英语</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<UserInfoResult> GetUserInfo(string openid, string access_token, string lang = "zh_CN", ApiConfig config = null)
        {
            var url = "https://api.weixin.qq.com/sns/userinfo?" +
                      $"access_token={Uri.EscapeDataString(access_token)}&" +
                      $"openid={Uri.EscapeDataString(openid)}&" +
                      $"lang={lang}";

            var response = await ApiHelper.GetString(url, config).ConfigureAwait(false);
            return ApiHelper.BuildResult<UserInfoResult>(response);
        }

        public class UserInfoResult : UserBase, IApiResult
        {
            /// <summary>
            /// 用户特权信息，json 数组，如微信沃卡用户为（chinaunicom）
            /// </summary>
            public string[] privilege { get; set; }
        }

        /// <summary>
        /// 检验授权凭证（access_token）是否有效
        /// </summary>
        /// <param name="openid">用户的唯一标识</param>
        /// <param name="access_token">网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<bool> Auth(string openid, string access_token, ApiConfig config = null)
        {
            var url = "https://api.weixin.qq.com/sns/auth?" +
                      $"access_token={Uri.EscapeDataString(access_token)}&" +
                      $"openid={Uri.EscapeDataString(openid)}";

            try
            {
                await ApiHelper.GetVoid(url, config).ConfigureAwait(false);
                return true;
            }
            catch (ApiException)
            {
                return false;
            }
        }
    }

    public class UserBase
    {
        /// <summary>
        /// 用户的标识，对当前公众号唯一
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 用户的昵称
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        public int sex { get; set; }
        /// <summary>
        /// 用户所在城市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 用户所在国家
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 用户所在省份
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
        /// </summary>
        public string headimgurl { get; set; }
        /// <summary>
        /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。
        /// </summary>
        public string unionid { get; set; }
    }
}
