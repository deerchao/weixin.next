using System;

namespace Weixin.Next.MP.Messaging
{
    public class MessageException : Exception
    {
        public MessageException(string message)
            : base(message)
        {
        }
    }
}