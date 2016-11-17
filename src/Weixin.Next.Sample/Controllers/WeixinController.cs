using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Weixin.Next.MP.Api;
using Weixin.Next.MP.Messaging;

namespace Weixin.Next.Sample.Controllers
{
    public class WeixinController : Controller
    {
        /// <summary>
        /// 用于处理微信公众号设置服务器 URL 时的验证消息
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(string signature, string timestamp, string nonce, string echostr)
        {
            var token = ConfigurationManager.AppSettings["weixin.token"];
            if (Signature.Check(token, timestamp, nonce, signature))
            {
                return Content(echostr);
            }

            return View("Get");
        }

        /// <summary>
        /// 用于处理微信公众号发送的实质性消息
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public async Task<ActionResult> Post(PostUrlParameters p)
        {
            try
            {
                var response = await WeixinConfig.MessageCenter.ProcessMessage(p, Request.InputStream).ConfigureAwait(false);
                return Content(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"消息处理异常: {ex.Message}, {ex.StackTrace}");
                return Content("");
            }
        }


        /// <summary>
        /// 将用户重定向到微信页面, 以获取用户的 openid
        /// </summary>
        public ActionResult AuthBase()
        {
            return Auth(OAuth2.ScopeBase, "base");
        }

        /// <summary>
        /// 将用户重定向到微信页面, 以获取用户的昵称,性别,所在地等信息(需要用户关注或者授权)
        /// </summary>
        public ActionResult AuthUserInfo()
        {
            return Auth(OAuth2.ScopeUserInfo, "userinfo");
        }

        private ActionResult Auth(string scope, string state)
        {
            var appId = ConfigurationManager.AppSettings["weixin.appId"];

            //相对网址变为绝对网址
            var callbackUrl = new Uri(Request.Url, Url.Action("AuthCallback")).ToString();

            var redirectUrl = OAuth2.GetAuthorizeUrl(appId, callbackUrl, scope, state);
            return Redirect(redirectUrl);
        }

        /// <summary>
        /// 获取用户信息后被微信重定向回来的入口
        /// </summary>
        /// <param name="code">授权 code</param>
        /// <param name="state">之前重定向过去时网址中的 state 参数</param>
        /// <returns></returns>
        public async Task<ActionResult> AuthCallback(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content($"您拒绝了授权.");
            }

            var appId = ConfigurationManager.AppSettings["weixin.appId"];
            var appSecret = ConfigurationManager.AppSettings["weixin.appSecret"];

            OAuth2.AccessTokenResult tokenResult;
            try
            {
                tokenResult = await OAuth2.GetAccessToken(appId, appSecret, code).ConfigureAwait(false);
            }
            catch (ApiException ex)
            {
                return Content($"使用 code({code}) 换取 access_token 失败: {ex.Code} {ex.Message}");
            }

            //只有使用了 OAuth2.ScopeUserInfo (snsapi_userinfo) 为 scope 才能获取用户信息, 否则到此为止
            if (state != "userinfo")
                return Content("基本用户信息: " + JsonConvert.SerializeObject(tokenResult));

            try
            {
                var userResult = await OAuth2.GetUserInfo(tokenResult.openid, tokenResult.access_token);
                return Content("完整用户信息: " + JsonConvert.SerializeObject(userResult));
            }
            catch (ApiException ex)
            {
                return Content($"使用 access_token({tokenResult.access_token}) 换取用户({tokenResult.openid})信息失败: {ex.Code} {ex.Message}");
            }
        }

        /// <summary>
        /// 展示调用微信 JS-SDK
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> JsScan()
        {
            //请先登录微信公众平台进入“公众号设置”的“功能设置”里填写“JS接口安全域名”

            var appId = ConfigurationManager.AppSettings["weixin.appId"];
            var config = await JsConfig.Generate(WeixinConfig.JsapiTicketManager, appId, Request.Url.ToString(),
                new[]
                {
                    JsApi.scanQRCode
                },
                true).ConfigureAwait(false);
            ViewBag.wxconfig = JsonConvert.ToString(config);

            return View();
        }

        /// <summary>
        /// <para>展示 access_token 中控服务器的实现方式. 其他网站/程序需要使用到微信 access_token 的地方可以请求此 action 获取.</para>
        /// <para>如果是旧 access_token 过期, 最好提供已过期的值, 以避免短时间内多次刷新造成的问题</para>
        /// <para>警告: 请在实际使用时先验证客户端是否经过授权, 否则将会造成严重的安全问题.</para>
        /// </summary>
        /// <param name="refresh">是否旧的 access_token 已经过期, 需要刷新</param>
        /// <param name="oldToken">已过期的旧 access_token, 如果未知是否过期不必提供</param>
        /// <returns></returns>
        public async Task<JsonResult> AccessToken(bool refresh = false, string oldToken = null)
        {
            var task = refresh
                ? WeixinConfig.AccessTokenManager.RefreshTokenInfo(oldToken)
                : WeixinConfig.AccessTokenManager.GetTokenInfo();

            return Json(await task.ConfigureAwait(false));
        }
    }
}