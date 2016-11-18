using System;
using System.Threading;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    public interface IJsapiTicketManager
    {
        /// <summary>
        /// 获取 jsapi_ticket 及过期时间
        /// </summary>
        /// <returns></returns>
        Task<JsapiTicketInfo> GetTicketInfo();

        /// <summary>
        /// 刷新 jsapi_ticket 及过期时间
        /// </summary>
        /// <param name="oldTicket">已过期的 jsapi_ticket</param>
        /// <returns></returns>
        Task<JsapiTicketInfo> RefreshTicketInfo(string oldTicket = null);

    }

    public class JsapiTicketInfo
    {
        /// <summary>
        /// jsapi_ticket
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// 过期时间, unix 时间戳. 注意: 是绝对时间, 不是微信返回的 expires_in
        /// </summary>
        public long ExpireTime { get; set; }
    }

    public static class JsapiTicketManagerExtensions
    {
        /// <summary>
        /// 获取 jsapi_ticket
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static async Task<string> GetTicket(this IJsapiTicketManager manager)
        {
            var info = await manager.GetTicketInfo().ConfigureAwait(false);
            return info.Ticket;
        }

        /// <summary>
        /// 刷新 jsapi_ticket
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="oldTicket">过期的 jsapi_ticket</param>
        /// <returns></returns>
        public static async Task<string> RefreshTicket(this IJsapiTicketManager manager, string oldTicket = null)
        {
            var info = await manager.RefreshTicketInfo(oldTicket).ConfigureAwait(false);
            return info.Ticket;
        }
    }

    public abstract class JsapiTicketManagerBase : IJsapiTicketManager
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private string _ticket;
        private long _expireTime;

        public async Task<JsapiTicketInfo> GetTicketInfo()
        {
            var expireTime = Volatile.Read(ref _expireTime);
            if (expireTime == 0)
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);

                try
                {
                    expireTime = Volatile.Read(ref _expireTime);
                    if (expireTime == 0)
                    {
                        var result = await Get().ConfigureAwait(false);
                        _ticket = result.Ticket;
                        Volatile.Write(ref _expireTime, result.ExpireTime);
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            else
            {
                var now = DateTime.UtcNow.Ticks;
                if (expireTime <= now)
                {
                    await _semaphore.WaitAsync().ConfigureAwait(false);

                    try
                    {
                        expireTime = Volatile.Read(ref _expireTime);
                        now = DateTime.UtcNow.Ticks;
                        if (expireTime <= now)
                        {
                            var result = await Refresh(_ticket).ConfigureAwait(false);
                            _ticket = result.Ticket;
                            Volatile.Write(ref _expireTime, result.ExpireTime);

                            return result;
                        }
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
            }

            return new JsapiTicketInfo
            {
                Ticket = _ticket,
                ExpireTime = _expireTime,
            };
        }

        public async Task<JsapiTicketInfo> RefreshTicketInfo(string oldTicket = null)
        {
            if (oldTicket == null || oldTicket == Volatile.Read(ref _ticket))
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);

                try
                {
                    if (oldTicket == null || oldTicket == Volatile.Read(ref _ticket))
                    {
                        var result = await Refresh(oldTicket).ConfigureAwait(false);
                        _ticket = result.Ticket;
                        Volatile.Write(ref _expireTime, result.ExpireTime);

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

        /// <summary>
        /// 获取 jsapi_ticket 及过期时间, 强制刷新
        /// </summary>
        /// <returns></returns>
        protected abstract Task<JsapiTicketInfo> Refresh(string oldTicket);
        /// <summary>
        /// 获取 jsapi_ticket 及过期时间, 不强制刷新
        /// </summary>
        /// <returns></returns>
        protected abstract Task<JsapiTicketInfo> Get();
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

        protected override Task<JsapiTicketInfo> Refresh(string oldTicket)
        {
            return Get();
        }

        protected override async Task<JsapiTicketInfo> Get()
        {
            //由于不是与自己的全局缓存服务器通信, 我们只能直接从微信服务器刷新
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