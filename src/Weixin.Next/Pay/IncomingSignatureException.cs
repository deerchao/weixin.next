using System;

namespace Weixin.Next.Pay
{
    public class IncomingSignatureException : Exception
    {
        public IncomingSignatureException()
            : base("接收到的数据中签名不正确")
        {
        }
    }
}
