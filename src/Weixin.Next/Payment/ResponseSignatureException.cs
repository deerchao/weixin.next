using System;

namespace Weixin.Next.Payment
{
    public class ResponseSignatureException : Exception
    {
        public ResponseSignatureException()
            : base("返回结果中的签名不正确")
        {
        }
    }
}
