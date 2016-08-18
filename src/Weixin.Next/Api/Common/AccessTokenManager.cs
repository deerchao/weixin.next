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
        /// <returns></returns>
        Task<AccessTokenInfo> GetTokenInfo();

        /// <summary>
        /// 刷新 access_token 及过期时间
        /// </summary>
        /// <param name="oldToken">已过期的旧 access_token</param>
        /// <returns></returns>
        Task<AccessTokenInfo> RefreshTokenInfo(string oldToken = null);
    }

    public class AccessTokenInfo
    {
        /// <summary>
        /// access_token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 过期时间, unix 时间戳. 注意: 是绝对时间, 不是微信返回的 expires_in
        /// </summary>
        public long ExpireTime { get; set; }
    }

    public static class AccessTokenManagerExtensions
    {
        /// <summary>
        /// 获取 access_token
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static async Task<string> GetToken(this IAccessTokenManager manager)
        {
            var info = await manager.GetTokenInfo().ConfigureAwait(false);
            return info.Token;
        }

        /// <summary>
        /// 刷新 access_token
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="oldToken">已过期的旧 access_token</param>
        /// <returns></returns>
        public static async Task<string> RefreshToken(this IAccessTokenManager manager, string oldToken = null)
        {
            var info = await manager.RefreshTokenInfo(oldToken).ConfigureAwait(false);
            return info.Token;
        }
    }

    public abstract class AccessTokenManagerBase : IAccessTokenManager
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private string _token;
        private long _expireTime;

        public async Task<AccessTokenInfo> GetTokenInfo()
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
                        _token = result.Token;
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
                            var result = await Refresh(_token).ConfigureAwait(false);
                            _token = result.Token;
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

            return new AccessTokenInfo
            {
                Token = _token,
                ExpireTime = _expireTime,
            };
        }

        public async Task<AccessTokenInfo> RefreshTokenInfo(string oldToken = null)
        {
            if (oldToken == null || oldToken == Volatile.Read(ref _token))
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);

                try
                {
                    if (oldToken == null || oldToken == Volatile.Read(ref _token))
                    {
                        var result = await Refresh(oldToken).ConfigureAwait(false);
                        _token = result.Token;
                        Volatile.Write(ref _expireTime, result.ExpireTime);

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

        /// <summary>
        /// 获取 access_token 及过期时间, 不强制刷新
        /// </summary>
        /// <returns></returns>
        protected abstract Task<AccessTokenInfo> Get();
        /// <summary>
        /// 获取 access_token 及过期时间, 强制刷新
        /// </summary>
        /// <returns></returns>
        protected abstract Task<AccessTokenInfo> Refresh(string oldToken);
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

        protected override async Task<AccessTokenInfo> Get()
        {
            //由于不是与自己的全局缓存服务器通信, 我们只能直接从微信服务器刷新
            var result = await Token.Get(_appId, _appSecret, Config);
            return new AccessTokenInfo
            {
                Token = result.access_token,
                //提前 10 秒到期
                ExpireTime = DateTime.UtcNow.AddSeconds(result.expires_in - 10).Ticks,
            };
        }

        protected override Task<AccessTokenInfo> Refresh(string oldToken)
        {
            return Get();
        }
    }
}