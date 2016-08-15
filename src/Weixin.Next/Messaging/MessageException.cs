using System;

namespace Weixin.Next.Messaging
{
    public class MessageException : Exception
    {
        public MessageException(string message)
            : base(message)
        {
        }
    }
}