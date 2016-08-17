using System;
using System.Threading;
using System.Threading.Tasks;

namespace Weixin.Next.Api
{
    public interface IAccessTokenManager
    {
        /// <summary>
        /// 获取 access_token 及过期时间
        /// </summary>
        /// <param name="forceRefresh">是否在未到过期时间时强制刷新</param>
        /// <returns></returns>
        Task<AccessTokenInfo> GetTokenInfo(bool forceRefresh = false);
    }

    public class AccessTokenInfo
    {
        /// <summary>
        /// access_token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 过期时间, unix 时间戳
        /// </summary>
        public long ExpireTime { get; set; }
    }

    public static class AccessTokenManagerExtensions
    {
        /// <summary>
        /// 获取 access_token
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="forceRefresh">是否在未到过期时间时强制刷新</param>
        /// <returns></returns>
        public static async Task<string> GetToken(this IAccessTokenManager manager, bool forceRefresh = false)
        {
            var info = await manager.GetTokenInfo(forceRefresh).ConfigureAwait(false);
            return info.Token;
        }
    }

    public abstract class AccessTokenManagerBase : IAccessTokenManager
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private string _token;
        private long _expireTime;

        public async Task<AccessTokenInfo> GetTokenInfo(bool forceRefresh = false)
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
                        _token = result.Token;
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

            return new AccessTokenInfo
            {
                Token = _token,
                ExpireTime = _expireTime,
            };
        }

        protected abstract Task<AccessTokenInfo> Refresh();
    }

    /// <summary>
    /// 用于缓存并过期时刷新 access_token
    /// </summary>
    public class AccessTokenManager : AccessTokenManagerBase
    {
        private readonly string _appId;
        private readonly string _appSecret;


        public AccessTokenManager(string appId, string appSecret)
        {
            _appId = appId;
            _appSecret = appSecret;
        }

        public ApiConfig Config { get; set; }

        protected override async Task<AccessTokenInfo> Refresh()
        {
            var result = await Token.Get(_appId, _appSecret, Config);
            return new AccessTokenInfo
            {
                Token = result.access_token,
                //提前 10 秒到期
                ExpireTime = DateTime.UtcNow.AddSeconds(result.expires_in - 10).Ticks,
            };
        }
    }
}