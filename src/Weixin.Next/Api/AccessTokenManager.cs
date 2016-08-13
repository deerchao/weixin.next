using System;
using System.Threading;
using System.Threading.Tasks;

namespace Weixin.Next.Api
{
    public class AccessTokenManager
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private readonly string _appId;
        private readonly string _appSecret;

        private string _token;
        private long _expireTime;

        public AccessTokenManager(string appId, string appSecret)
        {
            _appId = appId;
            _appSecret = appSecret;
        }

        /// <summary>
        /// 获取 access_token
        /// </summary>
        /// <param name="forceRefresh">是否在未到过期时间时强制刷新</param>
        /// <returns></returns>
        public async Task<string> GetToken(bool forceRefresh = false)
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
                        var result = await Token.Retrieve(_appId, _appSecret).ConfigureAwait(false);
                        _token = result.access_token;
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

            return _token;
        }
    }
}