using System;

namespace Weixin.Next.MP.Api
{
    /// <summary>
    /// 代表微信服务返回的错误信息
    /// </summary>
    public class ApiException : Exception
    {
        private readonly int _code;

        public ApiException(int code, string message)
            : base($"{code} - {message}")
        {
            _code = code;
        }

        public int Code
        {
            get { return _code; }
        }
    }
}