using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
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

    /// <summary>
    /// 为以不同方式(从微信获取或全局服务器获取)获取 access_token 的类提供基类
    /// </summary>
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

    /// <summary>
    /// 从中控服务器(全局缓存)中获取 access_token
    /// </summary>
    public abstract class CentralAccessTokenManager : AccessTokenManagerBase
    {
        protected override Task<AccessTokenInfo> Get()
        {
            return Request(false, null);
        }

        public IJsonParser JsonParser { get; set; }

        public HttpClient HttpClient { get; set; }

        protected override Task<AccessTokenInfo> Refresh(string oldToken)
        {
            return Request(true, oldToken);
        }

        private async Task<AccessTokenInfo> Request(bool refresh, string oldToken)
        {
            var http = HttpClient ?? new HttpClient();
            var response = await http.SendAsync(GetRequest(refresh, oldToken)).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var jsonValue = JsonParser.Parse(body);
            return JsonParser.Build<AccessTokenInfo>(jsonValue);
        }

        /// <summary>
        /// 示例实现:
        /// <code>return new HttpRequestMessage(HttpMethod.Post, "http://xxx.com/weixin/accesstoken")
        ///{
        ///	Content = new FormUrlEncodedContent(new[]
        ///	{
        ///		new KeyValuePair&lt;string, string&gt;("refresh", refresh.ToString()),
        ///		new KeyValuePair&lt;string, string&gt;("oldToken", oldToken),
        ///	})
        ///};
        ///</code>
        ///注意要配合服务器端的要求, 添加签名或其他身份认证代码, 避免 access_token 被公开.
        /// </summary>
        /// <param name="refresh">是否旧的 access_token 已经过期, 需要刷新</param>
        /// <param name="oldToken">已过期的旧 access_token, 如果未知是否过期则为 null</param>
        /// <returns></returns>
        protected abstract HttpRequestMessage GetRequest(bool refresh, string oldToken);
    }
}