using System;

namespace Weixin.Next
{
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