using System;
using System.Threading.Tasks;
using Weixin.Next.MP.Messaging.Requests;
using Weixin.Next.MP.Messaging.Responses;

namespace Weixin.Next.MP.Messaging
{
    public abstract class MessageHandler : IMessageHandler
    {
        private static readonly Task<bool> _done = Task.FromResult(true);
        private static readonly Task<IResponseMessage> _empty = Task.FromResult((IResponseMessage)RawResponseMessage.Empty);
        private static readonly Task<IResponseMessage> _success = Task.FromResult((IResponseMessage)RawResponseMessage.Success);

        protected RequestMessage Request { get; private set; }

        public async Task<IResponseMessage> Handle(RequestMessage request)
        {
            Request = request;

            await OnHandling().ConfigureAwait(false);

            Task<IResponseMessage> responseTask;
            switch (request.MsgType)
            {
                case RequestMessageType.@event:
                    responseTask = HandleEventMessage((EventMessage)request);
                    break;
                case RequestMessageType.unknown:
                    responseTask = HandleUnknownRequest((UnknownRequestMessage)request);
                    break;
                case RequestMessageType.text:
                case RequestMessageType.image:
                case RequestMessageType.voice:
                case RequestMessageType.video:
                case RequestMessageType.shortvideo:
                case RequestMessageType.location:
                case RequestMessageType.link:
                    responseTask = HandleNormalRequest((NormalRequestMessage)request);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var response = await responseTask.ConfigureAwait(false);
            await OnHandled(response).ConfigureAwait(false);

            return response;
        }

        protected virtual Task OnHandling()
        {
            return _done;
        }

        protected virtual Task OnHandled(IResponseMessage response)
        {
            return _done;
        }

        #region Normal Requests
        protected virtual Task<IResponseMessage> HandleNormalRequest(NormalRequestMessage request)
        {
            switch (request.MsgType)
            {
                case RequestMessageType.text:
                    return HandleTextRequest((TextRequestMessage)request);
                case RequestMessageType.image:
                    return HandleImageRequest((ImageRequestMessage)request);
                case RequestMessageType.voice:
                    return HandleVoiceRequest((VoiceRequestMessage)request);
                case RequestMessageType.video:
                    return HandleVideoRequest((VideoRequestMessage)request);
                case RequestMessageType.shortvideo:
                    return HandleShortVideoRequest((ShortVideoRequestMessage)request);
                case RequestMessageType.location:
                    return HandleLocationRequest((LocationRequestMessage)request);
                case RequestMessageType.link:
                    return HandleLinkRequest((LinkRequestMessage)request);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        protected virtual Task<IResponseMessage> HandleTextRequest(TextRequestMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleImageRequest(ImageRequestMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleVoiceRequest(VoiceRequestMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleVideoRequest(VideoRequestMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleShortVideoRequest(ShortVideoRequestMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleLocationRequest(LocationRequestMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleLinkRequest(LinkRequestMessage request)
        {
            return DefaultResponse();
        }


        protected virtual Task<IResponseMessage> HandleUnknownRequest(UnknownRequestMessage request)
        {
            return DefaultResponse();
        }
        #endregion

        #region Events
        protected virtual Task<IResponseMessage> HandleEventMessage(EventMessage request)
        {
            switch (request.Event)
            {
                case EventMessageType.subscribe:
                    return HandleSubscribeEvent((SubscribeEventMessage)request);
                case EventMessageType.unsubscribe:
                    return HandleUnsubscribeEvent((UnsubscribeEventMessage)request);
                case EventMessageType.scan:
                    return HandleScanEvent((ScanEventMessage)request);
                case EventMessageType.location:
                    return HandleLocationEvent((LocationEventMessage)request);
                case EventMessageType.templatesendjobfinish:
                    return HandleTemplateSendJobFinishEvent((TemplateSendJobFinishEventMessage)request);
                case EventMessageType.masssendjobfinish:
                    return HandleMassSendJobFinishEvent((MassSendJobFinishEventMessage)request);

                case EventMessageType.click:
                    return HandleClickMenu((ClickMenuMessage)request);
                case EventMessageType.view:
                    return HandleViewMenu((ViewMenuMessage)request);
                case EventMessageType.scancode_push:
                    return HandleScanCodePushMenu((ScanCodePushMenuMessage)request);
                case EventMessageType.scancode_waitmsg:
                    return HandleScanCodeWaitMsgMenu((ScanCodeWaitMsgMenuMessage)request);
                case EventMessageType.pic_sysphoto:
                    return HandlePicSysPhotoMenu((PicSysPhotoMenuMessage)request);
                case EventMessageType.pic_photo_or_album:
                    return HandlePicPhotoOrAlbumMenu((PicPhotoOrAlbumMenuMessage)request);
                case EventMessageType.pic_weixin:
                    return HandlePicWeixinMenu((PicWeixinMenuMessage)request);
                case EventMessageType.location_select:
                    return HandleLocationSelectMenu((LocationSelectMenuMessage)request);

                case EventMessageType.qualification_verify_success:
                    return HandleQualificationVerifySuccessEvent((QualificationVerifySuccessEvent)request);
                case EventMessageType.qualification_verify_fail:
                    return HandleQualificationVerifyFailEvent((QualificationVerifyFailEvent)request);
                case EventMessageType.naming_verify_success:
                    return HandleNamingVerifySuccessEvent((NamingVerifySuccessEvent)request);
                case EventMessageType.naming_verify_fail:
                    return HandleNamingVerifyFailEvent((NamingVerifyFailEvent)request);
                case EventMessageType.annual_renew:
                    return HandleAnnualRenewEvent((AnnualRenewEvent)request);
                case EventMessageType.verify_expired:
                    return HandleVerifyExpiredEvent((VerifyExpiredEvent)request);

                case EventMessageType.unknown:
                    return HandleUnknownEvent((UnknownEventMessage)request);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual Task<IResponseMessage> HandleSubscribeEvent(SubscribeEventMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleUnsubscribeEvent(UnsubscribeEventMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleScanEvent(ScanEventMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleLocationEvent(LocationEventMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleTemplateSendJobFinishEvent(TemplateSendJobFinishEventMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleMassSendJobFinishEvent(MassSendJobFinishEventMessage request)
        {
            return DefaultResponse();
        }

        #region Menu Events
        protected virtual Task<IResponseMessage> HandleClickMenu(ClickMenuMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleViewMenu(ViewMenuMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleScanCodePushMenu(ScanCodePushMenuMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleScanCodeWaitMsgMenu(ScanCodeWaitMsgMenuMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandlePicSysPhotoMenu(PicSysPhotoMenuMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandlePicPhotoOrAlbumMenu(PicPhotoOrAlbumMenuMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandlePicWeixinMenu(PicWeixinMenuMessage request)
        {
            return DefaultResponse();
        }

        protected virtual Task<IResponseMessage> HandleLocationSelectMenu(LocationSelectMenuMessage request)
        {
            return DefaultResponse();
        }
        #endregion

        #region Verify Events
        private Task<IResponseMessage> HandleQualificationVerifySuccessEvent(QualificationVerifySuccessEvent request)
        {
            return DefaultResponse();
        }

        private Task<IResponseMessage> HandleQualificationVerifyFailEvent(QualificationVerifyFailEvent request)
        {
            return DefaultResponse();
        }

        private Task<IResponseMessage> HandleNamingVerifySuccessEvent(NamingVerifySuccessEvent request)
        {
            return DefaultResponse();

        }

        private Task<IResponseMessage> HandleNamingVerifyFailEvent(NamingVerifyFailEvent request)
        {
            return DefaultResponse();

        }

        private Task<IResponseMessage> HandleAnnualRenewEvent(AnnualRenewEvent request)
        {
            return DefaultResponse();

        }

        private Task<IResponseMessage> HandleVerifyExpiredEvent(VerifyExpiredEvent request)
        {
            return DefaultResponse();

        }
        #endregion

        protected virtual Task<IResponseMessage> HandleUnknownEvent(UnknownEventMessage request)
        {
            return DefaultResponse();
        }
        #endregion

        protected virtual Task<IResponseMessage> DefaultResponse()
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

        protected Task<IResponseMessage> Result(IResponseMessage request)
        {
            return Task.FromResult(request);
        }

        protected TextResponseMessage Text(string content)
        {
            return new TextResponseMessage
            {
                FromUserName = Request.ToUserName,
                ToUserName = Request.FromUserName,
                Content = content,
            };
        }

        protected ImageResponseMessage Image(string mediaId)
        {
            return new ImageResponseMessage
            {
                FromUserName = Request.ToUserName,
                ToUserName = Request.FromUserName,
                MediaId = mediaId,
            };
        }

        protected VoiceResponseMessage Voice(string mediaId)
        {
            return new VoiceResponseMessage
            {
                FromUserName = Request.ToUserName,
                ToUserName = Request.FromUserName,
                MediaId = mediaId,
            };
        }

        protected VideoResponseMessage Video(string mediaId, string title, string description)
        {
            return new VideoResponseMessage
            {
                FromUserName = Request.ToUserName,
                ToUserName = Request.FromUserName,
                MediaId = mediaId,
                Title = title,
                Description = description,
            };
        }

        protected MusicResponseMessage Music(string title, string description, string musicUrl, string highQualityUrl, string thumbMediaId)
        {
            return new MusicResponseMessage
            {
                FromUserName = Request.ToUserName,
                ToUserName = Request.FromUserName,
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

        protected NewsResponseMessage News(NewsResponseMessage.NewsArticle[] articles)
        {
            return new NewsResponseMessage
            {
                FromUserName = Request.ToUserName,
                ToUserName = Request.FromUserName,
                ArticleCount = articles.Length,
                Articles = articles,
            };
        }

        protected TransferCustomerServiceResponseMessage TransferCustomerService(string kfAccount)
        {
            return new TransferCustomerServiceResponseMessage
            {
                FromUserName = Request.ToUserName,
                ToUserName = Request.FromUserName,
                TransInfo = new TransferCustomerServiceResponseMessage.TransKfInfo
                {
                    KfAccount = kfAccount,
                }
            };
        }
        #endregion
    }
}