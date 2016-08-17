using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Weixin.Next.Api
{
    /// <summary>
    /// 用于缓存并过期时刷新 jsapi_ticket
    /// </summary>
    public class JsapiTicketManager
    {
        private readonly string _appId;
        private readonly ApiConfig _config;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private string _ticket;
        private long _expireTime;

        public JsapiTicketManager(string appId, ApiConfig config)
        {
            _appId = appId;
            _config = config;
        }

        /// <summary>
        /// 获取 jsapi_ticket
        /// </summary>
        /// <param name="forceRefresh">是否在未到过期时间时强制刷新</param>
        /// <returns></returns>
        public async Task<string> GetTicket(bool forceRefresh = false)
        {
            var expireTime = Interlocked.Read(ref _expireTime);
            var now = DateTime.UtcNow.Ticks;
            if (forceRefresh || expireTime <= now)
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);

                try
                {
                    expireTime = Volatile.Read(ref _expireTime);
                    now = DateTime.UtcNow.Ticks;
                    if (forceRefresh || expireTime <= now)
                    {
                        var result = await Ticket.GetJsapi(_config).ConfigureAwait(false);
                        _ticket = result.ticket;
                        //提前 10 秒到期
                        expireTime = now + TimeSpan.FromSeconds(result.expires_in - 10).Ticks;
                        Interlocked.Exchange(ref _expireTime, expireTime);
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return _ticket;
        }

        /// <summary>
        /// 生成 jsapi 中的 wx.config 方法需要的参数
        /// </summary>
        /// <param name="url">需要调用 wx.config 页面的完整网址</param>
        /// <param name="jsApiList">需要的接口名称列表</param>
        /// <param name="debug">只否开启调度模式</param>
        /// <returns></returns>
        public async Task<JsConfig> GenerateJsConfig(string url, string[] jsApiList, bool debug = false)
        {
            var hashIndex = url.IndexOf('#');
            if (hashIndex > 0)
                url = url.Substring(0, hashIndex);

            var result = new JsConfig
            {
                debug = debug,
                appId = _appId,
                timestamp = DateTime.UtcNow.ToUnixTimestamp(),
                jsApiList = jsApiList,
                nonceStr = Guid.NewGuid().ToString("n"),
            };

            var ticket = await GetTicket().ConfigureAwait(false);

            var string1 = $"jsapi_ticket={ticket}&noncestr={result.nonceStr}&timestamp={result.timestamp:D}&url={url}";

            using (var sha1 = SHA1.Create())
            {
                var signatureData = sha1.ComputeHash(Encoding.UTF8.GetBytes(string1));
                result.signature = string.Concat(signatureData.Select(x => x.ToString("x2")));
            }

            return result;
        }
    }

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
    }
}