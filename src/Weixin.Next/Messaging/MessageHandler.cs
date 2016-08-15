using System;
using System.Threading.Tasks;
using Weixin.Next.Messaging.Requests;
using Weixin.Next.Messaging.Responses;

namespace Weixin.Next.Messaging
{
    public abstract class MessageHandler : IMessageHandler
    {
        private static readonly Task<IResponseMessage> _empty = Task.FromResult((IResponseMessage)new RawResponseMessage(""));
        private static readonly Task<IResponseMessage> _success = Task.FromResult((IResponseMessage)new RawResponseMessage("success"));

        public Task<IResponseMessage> Handle(RequestMessage message)
        {
            switch (message.MsgType)
            {
                case RequestMessageType.@event:
                    return HandleEventMessage((EventMessage)message);
                case RequestMessageType.unknown:
                    return HandleUnknownRequest((UnknownRequestMessage)message);
                case RequestMessageType.text:
                case RequestMessageType.image:
                case RequestMessageType.voice:
                case RequestMessageType.video:
                case RequestMessageType.shortvideo:
                case RequestMessageType.location:
                case RequestMessageType.link:
                    return HandleNormalRequest((NormalRequestMessage)message);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Normal Requests
        protected virtual Task<IResponseMessage> HandleNormalRequest(NormalRequestMessage message)
        {
            switch (message.MsgType)
            {
                case RequestMessageType.text:
                    return HandleTextRequest((TextRequestMessage)message);
                case RequestMessageType.image:
                    return HandleImageRequest((ImageRequestMessage)message);
                case RequestMessageType.voice:
                    return HandleVoiceRequest((VoiceRequestMessage)message);
                case RequestMessageType.video:
                    return HandleVideoRequest((VideoRequestMessage)message);
                case RequestMessageType.shortvideo:
                    return HandleShortVideoRequest((ShortVideoRequestMessage)message);
                case RequestMessageType.location:
                    return HandleLocationRequest((LocationRequestMessage)message);
                case RequestMessageType.link:
                    return HandleLinkRequest((LinkRequestMessage)message);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        protected virtual Task<IResponseMessage> HandleTextRequest(TextRequestMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleImageRequest(ImageRequestMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleVoiceRequest(VoiceRequestMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleVideoRequest(VideoRequestMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleShortVideoRequest(ShortVideoRequestMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleLocationRequest(LocationRequestMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleLinkRequest(LinkRequestMessage message)
        {
            return DefaultResponse(message);
        }


        protected virtual Task<IResponseMessage> HandleUnknownRequest(UnknownRequestMessage message)
        {
            return DefaultResponse(message);
        }
        #endregion

        #region Events
        protected virtual Task<IResponseMessage> HandleEventMessage(EventMessage message)
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
                    return HandleClickMenu((ClickMenuMessage)message);
                case EventMessageType.view:
                    return HandleViewMenu((ViewMenuMessage)message);
                case EventMessageType.scancode_push:
                    return HandleScanCodePushMenu((ScanCodePushMenuMessage)message);
                case EventMessageType.scancode_waitmsg:
                    return HandleScanCodeWaitMsgMenu((ScanCodeWaitMsgMenuMessage)message);
                case EventMessageType.pic_sysphoto:
                    return HandlePicSysPhotoMenu((PicSysPhotoMenuMessage)message);
                case EventMessageType.pic_photo_or_album:
                    return HandlePicPhotoOrAlbumMenu((PicPhotoOrAlbumMenuMessage)message);
                case EventMessageType.pic_weixin:
                    return HandlePicWeixinMenu((PicWeixinMenuMessage)message);
                case EventMessageType.location_select:
                    return HandleLocationSelectMenu((LocationSelectMenuMessage)message);

                case EventMessageType.unknown:
                    return HandleUnknownEvent((UnknownEventMessage)message);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual Task<IResponseMessage> HandleSubscribeEvent(SubscribeEventMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleUnsubscribeEvent(UnsubscribeEventMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleScanEvent(ScanEventMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleLocationEvent(LocationEventMessage message)
        {
            return DefaultResponse(message);
        }


        #region Menu Events
        protected virtual Task<IResponseMessage> HandleClickMenu(ClickMenuMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleViewMenu(ViewMenuMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleScanCodePushMenu(ScanCodePushMenuMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleScanCodeWaitMsgMenu(ScanCodeWaitMsgMenuMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandlePicSysPhotoMenu(PicSysPhotoMenuMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandlePicPhotoOrAlbumMenu(PicPhotoOrAlbumMenuMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandlePicWeixinMenu(PicWeixinMenuMessage message)
        {
            return DefaultResponse(message);
        }

        protected virtual Task<IResponseMessage> HandleLocationSelectMenu(LocationSelectMenuMessage message)
        {
            return DefaultResponse(message);
        }
        #endregion

        protected virtual Task<IResponseMessage> HandleUnknownEvent(UnknownEventMessage message)
        {
            return DefaultResponse(message);
        }
        #endregion

        protected virtual Task<IResponseMessage> DefaultResponse(RequestMessage message)
        {
            return Empty();
        }

        #region Helper methods to create responses
        /// <summary>
        ///  <para>特殊的返回消息: 空字符串</para>
        /// <para>微信服务器不会对此作任何处理，并且不会发起重试</para>
        /// </summary>
        /// <returns></returns>
        protected Task<IResponseMessage> Empty()
        {
            return _empty;
        }

        /// <summary>
        ///  <para>特殊的返回消息: 字符串"success"</para>
        /// <para>微信服务器不会对此作任何处理，并且不会发起重试</para>
        /// </summary>
        /// <returns></returns>
        protected Task<IResponseMessage> Success()
        {
            return _success;
        }


        protected TextResponseMessage Text(string content, RequestMessage request)
        {
            return new TextResponseMessage
            {
                FromUserName = request.ToUserName,
                ToUserName = request.FromUserName,
                Content = content,
            };
        }

        protected ImageResponseMessage Image(string mediaId, RequestMessage request)
        {
            return new ImageResponseMessage
            {
                FromUserName = request.ToUserName,
                ToUserName = request.FromUserName,
                MediaId = mediaId,
            };
        }

        protected VoiceResponseMessage Voice(string mediaId, RequestMessage request)
        {
            return new VoiceResponseMessage
            {
                FromUserName = request.ToUserName,
                ToUserName = request.FromUserName,
                MediaId = mediaId,
            };
        }

        protected VideoResponseMessage Video(string mediaId, string title, string description, RequestMessage request)
        {
            return new VideoResponseMessage
            {
                FromUserName = request.ToUserName,
                ToUserName = request.FromUserName,
                MediaId = mediaId,
                Title = title,
                Description = description,
            };
        }

        protected MusicResponseMessage Music(string title, string description, string musicUrl, string highQualityUrl, string thumbMediaId, RequestMessage request)
        {
            return new MusicResponseMessage
            {
                FromUserName = request.ToUserName,
                ToUserName = request.FromUserName,
                Music = new MusicResponseMessage.MusicInfo
                {
                    Title = title,
                    Description = description,
                    MusicURL = musicUrl,
                    HQMusicUrl = highQualityUrl,
                    ThumbMediaId = thumbMediaId,
                },
            };
        }

        protected NewsResponseMessage News(NewsResponseMessage.NewsArticle[] articles, RequestMessage request)
        {
            return new NewsResponseMessage
            {
                FromUserName = request.ToUserName,
                ToUserName = request.FromUserName,
                ArticleCount = articles.Length,
                Articles = articles,
            };
        }

        protected TransferCustomerServiceResponseMessage TransferCustomerService(string kfAccount, RequestMessage request)
        {
            return new TransferCustomerServiceResponseMessage
            {
                FromUserName = request.ToUserName,
                ToUserName = request.FromUserName,
                TransInfo = new TransferCustomerServiceResponseMessage.TransKfInfo
                {
                    KfAccount = kfAccount,
                }
            };
        }
        #endregion
    }
}