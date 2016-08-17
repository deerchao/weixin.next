using System;
using System.Threading;
using System.Threading.Tasks;

namespace Weixin.Next.Api
{
    public interface IJsapiTicketManager
    {
        /// <summary>
        /// 获取 jsapi_ticket 及过期时间
        /// </summary>
        /// <param name="forceRefresh">是否在未到过期时间时强制刷新</param>
        /// <returns></returns>
        Task<JsapiTicketInfo> GetTicketInfo(bool forceRefresh = false);
    }

    public class JsapiTicketInfo
    {
        /// <summary>
        /// jsapi_ticket
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// 过期时间, unix 时间戳
        /// </summary>
        public long ExpireTime { get; set; }
    }

    public static class JsapiTicketManagerExtensions
    {
        /// <summary>
        /// 获取 jsapi_ticket
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="forceRefresh">是否在未到过期时间时强制刷新</param>
        /// <returns></returns>
        public static async Task<string> GetTicket(this IJsapiTicketManager manager, bool forceRefresh = false)
        {
            var info = await manager.GetTicketInfo(forceRefresh).ConfigureAwait(false);
            return info.Ticket;
        }
    }

    public abstract class JsapiTicketManagerBase : IJsapiTicketManager
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private string _ticket;
        private long _expireTime;

        public async Task<JsapiTicketInfo> GetTicketInfo(bool forceRefresh = false)
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
                        var result = await Refresh().ConfigureAwait(false);
                        _ticket = result.Ticket;
                        expireTime = result.ExpireTime;
                        Interlocked.Exchange(ref _expireTime, expireTime);

                        return result;
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return new JsapiTicketInfo
            {
                Ticket = _ticket,
                ExpireTime = _expireTime,
            };
        }

        protected abstract Task<JsapiTicketInfo> Refresh();
    }

    /// <summary>
    /// 用于缓存并过期时刷新 jsapi_ticket
    /// </summary>
    public class JsapiTicketManager : JsapiTicketManagerBase
    {
        private readonly ApiConfig _config;

        public JsapiTicketManager(ApiConfig config)
        {
            _config = config;
        }

        protected override async Task<JsapiTicketInfo> Refresh()
        {
            var result = await Ticket.GetJsapi(_config);
            return new JsapiTicketInfo
            {
                Ticket = result.ticket,
                //提前 10 秒到期
                ExpireTime = DateTime.UtcNow.AddSeconds(result.expires_in - 10).Ticks,
            };
        }
    }

}