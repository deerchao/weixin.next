using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Weixin.Next.Api;
using Weixin.Next.Messaging;

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
            return Auth(Url.Action("AuthBase"), OAuth2.ScopeBase, "base");
        }

        /// <summary>
        /// 将用户重定向到微信页面, 以获取用户的昵称,性别,所在地等信息(需要用户关注或者授权)
        /// </summary>
        public ActionResult AuthUserInfo()
        {
            return Auth(Url.Action("AuthUserInfo"), OAuth2.ScopeUserInfo, "userinfo");
        }

        private ActionResult Auth(string localUrl, string scope, string state)
        {
            var appId = ConfigurationManager.AppSettings["weixin.appId"];

            var fullUrl = Request.Url.ToString();
            var localIndex = fullUrl.IndexOf(localUrl, StringComparison.OrdinalIgnoreCase);
            var prefix = fullUrl.Substring(0, localIndex);
            var callbackUrl = prefix + Url.Action("AuthCallback");

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
            var appSecret = ConfigurationManager.AppSettings["appSecret"];

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
    }
}