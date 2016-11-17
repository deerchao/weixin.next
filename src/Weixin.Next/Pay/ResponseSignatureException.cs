using System;

namespace Weixin.Next.Pay
{
    public class ResponseSignatureException : Exception
    {
        public ResponseSignatureException()
            : base("返回结果中的签名不正确")
        {
        }
    }
}
