using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Weixin.Next.Utilities;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    public class JsConfig
    {
        // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印
        public bool debug { get; set; }
        // 必填，公众号的唯一标识
        public string appId { get; set; }
        // 必填，生成签名的时间戳
        public long timestamp { get; set; }
        // 必填，生成签名的随机串
        public string nonceStr { get; set; }
        // 必填，签名
        public string signature { get; set; }
        // 必填，需要使用的JS接口列表
        public string[] jsApiList { get; set; }

        /// <summary>
        /// 生成 jsapi 中的 wx.config 方法需要的参数
        /// </summary>
        /// <param name="ticketManager">jsapi-ticket 的管理者</param>
        /// <param name="appId">微信应用唯一标识</param>
        /// <param name="url">需要调用 wx.config 页面的完整网址</param>
        /// <param name="jsApiList">需要的接口名称列表</param>
        /// <param name="debug">是否开启调度模式</param>
        /// <returns></returns>
        public static async Task<JsConfig> Generate(IJsapiTicketManager ticketManager, string appId, string url, string[] jsApiList, bool debug = false)
        {
            var hashIndex = url.IndexOf('#');
            if (hashIndex > 0)
                url = url.Substring(0, hashIndex);

            var result = new JsConfig
            {
                debug = debug,
                appId = appId,
                timestamp = DateTime.UtcNow.ToWeixinTimestamp(),
                jsApiList = jsApiList,
                nonceStr = Guid.NewGuid().ToString("n"),
            };

            var ticket = await ticketManager.GetTicket().ConfigureAwait(false);

            var string1 = $"jsapi_ticket={ticket}&noncestr={result.nonceStr}&timestamp={result.timestamp:D}&url={url}";

            using (var sha1 = SHA1.Create())
            {
                var signatureData = sha1.ComputeHash(Encoding.UTF8.GetBytes(string1));
                result.signature = string.Concat(signatureData.Select(x => x.ToString("x2")));
            }

            return result;
        }
    }
}