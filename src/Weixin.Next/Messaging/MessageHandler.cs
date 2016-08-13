using System;
using System.Threading.Tasks;
using Weixin.Next.Messaging.Requests;
using Weixin.Next.Messaging.Responses;

namespace Weixin.Next.Messaging
{
    public abstract class MessageHandler : IMessageHandler
    {
        private static readonly Task<ResponseMessage> _empty = Task.FromResult(ResponseMessage.Empty);
        private static readonly Task<ResponseMessage> _success = Task.FromResult(ResponseMessage.Success);

        public Task<ResponseMessage> Handle(RequestMessage message)
        {
            switch (message.MsgType)
            {
                case RequestMessageType.text:
                case RequestMessageType.image:
                case RequestMessageType.voice:
                case RequestMessageType.video:
                case RequestMessageType.shortvideo:
                case RequestMessageType.location:
                case RequestMessageType.link:
                    return HandleNormalMessage((NormalRequestMessage)message);
                case RequestMessageType.@event:
                    return HandleEventMessage((EventMessage)message);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        ///  <para>特殊的返回消息: 空字符串</para>
        /// <para>微信服务器不会对此作任何处理，并且不会发起重试</para>
        /// </summary>
        /// <returns></returns>
        protected Task<ResponseMessage> Empty()
        {
            return _empty;
        }

        /// <summary>
        ///  <para>特殊的返回消息: 字符串"success"</para>
        /// <para>微信服务器不会对此作任何处理，并且不会发起重试</para>
        /// </summary>
        /// <returns></returns>
        protected Task<ResponseMessage> Success()
        {
            return _success;
        }


        protected virtual Task<ResponseMessage> HandleNormalMessage(NormalRequestMessage message)
        {
            switch (message.MsgType)
            {
                case RequestMessageType.text:
                    return HandleTextMessage((TextRequestMessage)message);
                case RequestMessageType.image:
                    return HandleImageMessage((ImageRequestMessage)message);
                case RequestMessageType.voice:
                    return HandleVoiceMessage((VoiceRequestMessage)message);
                case RequestMessageType.video:
                    return HandleVideoMessage((VideoRequestMessage)message);
                case RequestMessageType.shortvideo:
                    return HandleShortVideoMessage((ShortVideoRequestMessage)message);
                case RequestMessageType.location:
                    return HandleLocationMessage((LocationRequestMessage)message);
                case RequestMessageType.link:
                    return HandleLinkMessage((LinkRequestMessage)message);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual Task<ResponseMessage> HandleTextMessage(TextRequestMessage message)
        {
            return DefaultResponse();
        }

        protected virtual Task<ResponseMessage> HandleImageMessage(ImageRequestMessage message)
        {
            return DefaultResponse();
        }

        protected virtual Task<ResponseMessage> HandleVoiceMessage(VoiceRequestMessage message)
        {
            return DefaultResponse();
        }

        protected virtual Task<ResponseMessage> HandleVideoMessage(VideoRequestMessage message)
        {
            return DefaultResponse();
        }

        protected virtual Task<ResponseMessage> HandleShortVideoMessage(ShortVideoRequestMessage message)
        {
            return DefaultResponse();
        }

        protected virtual Task<ResponseMessage> HandleLocationMessage(LocationRequestMessage message)
        {
            return DefaultResponse();
        }

        protected virtual Task<ResponseMessage> HandleLinkMessage(LinkRequestMessage message)
        {
            return DefaultResponse();
        }


        protected virtual Task<ResponseMessage> HandleEventMessage(EventMessage message)
        {
            switch (message.Event)
            {
                case EventMessageType.subscribe:
                    return HandleSubscribeEvent((SubscribeEventMessage)message);
                case EventMessageType.unsubscribe:
                    return HandleUnsubscribeEvent((UnsubscribeEventMessage)message);
                case EventMessageType.scan:
                    return HandleScanEvent((ScanEventMessage)message);
                case EventMessageType.location:
                    return HandleLocationEvent((LocationEventMessage)message);
                case EventMessageType.click:
                    return HandleClickEvent((ClickMenuMessage)message);
                case EventMessageType.view:
                    return HandleViewEvent((ViewMenuMessage)message);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual Task<ResponseMessage> HandleSubscribeEvent(SubscribeEventMessage message)
        {
            return DefaultResponse();
        }

        protected virtual Task<ResponseMessage> HandleUnsubscribeEvent(UnsubscribeEventMessage message)
        {
            return DefaultResponse();
        }

        protected virtual Task<ResponseMessage> HandleScanEvent(ScanEventMessage message)
        {
            return DefaultResponse();
        }

        protected virtual Task<ResponseMessage> HandleLocationEvent(LocationEventMessage message)
        {
            return DefaultResponse();
        }

        protected virtual Task<ResponseMessage> HandleClickEvent(ClickMenuMessage message)
        {
            return DefaultResponse();
        }

        protected virtual Task<ResponseMessage> HandleViewEvent(ViewMenuMessage message)
        {
            return DefaultResponse();
        }


        protected virtual Task<ResponseMessage> DefaultResponse()
        {
            return Empty();
        }
    }
}