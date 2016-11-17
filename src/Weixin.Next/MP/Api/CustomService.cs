using System;
using System.IO;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信客服
    /// </summary>
    public static class CustomService
    {
        #region 获取客服基本信息

        /// <summary>
        /// 邀请状态: 等待确认
        /// </summary>
        public const string InviteStatusWaiting = "waiting";
        /// <summary>
        /// 邀请状态: 被拒绝
        /// </summary>
        public const string InviteStatusRejected = "rejected";
        /// <summary>
        /// 邀请状态: 过期
        /// </summary>
        public const string InviteStatusExpired = "expired";

        /// <summary>
        /// 获取客服基本信息
        /// </summary>
        public static Task<GetKfListResult> GetKfList(ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetKfListResult>("https://api.weixin.qq.com/cgi-bin/customservice/getkflist?$acac$", config);
        }

        public class GetKfListResult : IApiResult
        {
            public KfRecord[] kf_list { get; set; }
        }

        public class KfRecord
        {
            /// <summary>
            /// 完整客服帐号，格式为：帐号前缀@公众号微信号
            /// </summary>
            public string kf_account { get; set; }
            /// <summary>
            /// 客服头像
            /// </summary>
            public string kf_headimgurl { get; set; }
            /// <summary>
            /// 客服编号
            /// </summary>
            public string kf_id { get; set; }
            /// <summary>
            /// 客服昵称
            /// </summary>
            public string kf_nick { get; set; }
            /// <summary>
            /// 如果客服帐号已绑定了客服人员微信号，则此处显示微信号
            /// </summary>
            public string kf_wx { get; set; }
            /// <summary>
            /// 如果客服帐号尚未绑定微信号，但是已经发起了一个绑定邀请，则此处显示绑定邀请的微信号
            /// </summary>
            public string invite_wx { get; set; }
            /// <summary>
            /// 如果客服帐号尚未绑定微信号，但是已经发起过一个绑定邀请，邀请的过期时间，为unix 时间戳
            /// </summary>
            public long invite_expire_time { get; set; }
            /// <summary>
            /// 邀请的状态，有等待确认“waiting”，被拒绝“rejected”，过期“expired”
            /// </summary>
            public string invite_status { get; set; }
        }
        #endregion

        #region 获取在线客服列表

        /// <summary>
        /// 获取在线客服列表
        /// </summary>
        /// <param name="config"></param>
        public static Task<GetOnlineKfListResult> GetOnlineKfList(ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetOnlineKfListResult>("https://api.weixin.qq.com/cgi-bin/customservice/getonlinekflist?$acac$", config);
        }

        public class GetOnlineKfListResult : IApiResult
        {
            public KfOnlineRecord[] kf_online_list { get; set; }
        }

        public class KfOnlineRecord
        {
            /// <summary>
            /// 完整客服帐号，格式为：帐号前缀@公众号微信号
            /// </summary>
            public string kf_account { get; set; }
            /// <summary>
            /// 客服在线状态，目前为：1、web 在线
            /// </summary>
            public int status { get; set; }
            /// <summary>
            /// 客服编号
            /// </summary>
            public string kf_id { get; set; }
            /// <summary>
            /// 客服当前正在接待的会话数
            /// </summary>
            public int accepted_case { get; set; }
        }
        #endregion


        #region 添加客服帐号

        /// <summary>
        /// 添加客服帐号
        /// </summary>
        /// <param name="kf_account">完整客服帐号，格式为：帐号前缀@公众号微信号，帐号前缀最多10个字符，必须是英文、数字字符或者下划线，后缀为公众号微信号，长度不超过30个字符</param>
        /// <param name="nickname">客服昵称，最长16个字</param>
        /// <param name="config"></param>
        public static Task AddKfAccount(string kf_account, string nickname, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/customservice/kfaccount/add?$acac$", new { kf_account, nickname }, config);
        }
        #endregion

        #region 邀请绑定客服帐号

        /// <summary>
        /// 邀请绑定客服帐号
        /// </summary>
        /// <param name="kf_account">完整客服帐号，格式为：帐号前缀@公众号微信号</param>
        /// <param name="invite_wx">接收绑定邀请的客服微信号</param>
        /// <param name="config"></param>
        public static Task InviteWorker(string kf_account, string invite_wx, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/customservice/kfaccount/inviteworker?$acac$", new { kf_account, invite_wx }, config);
        }
        #endregion

        #region 设置客服信息

        /// <summary>
        /// 设置客服信息
        /// </summary>
        /// <param name="kf_account">完整客服帐号，格式为：帐号前缀@公众号微信号</param>
        /// <param name="nickname">客服昵称，最长16个字</param>
        /// <param name="config"></param>
        public static Task UpdateAccount(string kf_account, string nickname, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/customservice/kfaccount/update?$acac$", new { kf_account, nickname }, config);
        }
        #endregion

        #region 上传客服头像

        public static Task UploadHeadImg(string kf_account, string filename, Stream fileStream, ApiConfig config = null)
        {
            return ApiHelper.UploadVoid("https://api.weixin.qq.com/customservice/kfaccount/uploadheadimg?$acac$&kf_account=" + Uri.EscapeDataString(kf_account),
                "media",
                filename,
                fileStream,
                null,
                config);
        }

        #endregion

        #region 删除客服帐号

        /// <summary>
        /// 删除客服帐号
        /// </summary>
        /// <param name="kf_account">完整客服帐号，格式为：帐号前缀@公众号微信号</param>
        /// <param name="config"></param>
        public static Task DeleteAccount(string kf_account, ApiConfig config = null)
        {
            return ApiHelper.GetVoid("https://api.weixin.qq.com/customservice/kfaccount/del?$acac$&kf_account=" + Uri.EscapeDataString(kf_account), config);
        }
        #endregion


        #region 获取聊天记录

        /// <summary>
        /// <para>获取聊天记录</para>
        /// <para>对某个时间段首次请求时 msgid 应为 1；</para>
        /// <para>若请求的 number 和返回的 number 一样，该时间段可能还有聊天记录未获取，将返回的 msgid 填进下次请求中；</para>
        /// <para>若请求的 number 和返回的 number 不一样，则该时间段的后续聊天记录获取完毕；</para>
        /// </summary>
        /// <param name="starttime">起始时间，unix时间戳</param>
        /// <param name="endtime">结束时间，unix时间戳，每次查询时段不能超过24小时</param>
        /// <param name="msgid">消息id顺序从小到大，从1开始</param>
        /// <param name="number">每次获取条数，最多10000条</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetMsgListResult> GetMsgList(long starttime, long endtime, long msgid, int number, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetMsgListResult>("https://api.weixin.qq.com/customservice/msgrecord/getmsglist?$acac$", new { starttime, endtime, msgid, number }, config);
        }

        public class GetMsgListResult : IApiResult
        {
            public MsgRecord[] recordlist { get; set; }
            /// <summary>
            /// 此次返回的记录数量
            /// </summary>
            public int number { get; set; }
            /// <summary>
            /// 继续查询时应填入的 msgid
            /// </summary>
            public long msgid { get; set; }
        }

        public class MsgRecord
        {
            /// <summary>
            /// 用户标识
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 操作码，2002（客服发送信息），2003（客服接收消息）
            /// </summary>
            public int opercode { get; set; }
            /// <summary>
            /// 聊天记录
            /// </summary>
            public string text { get; set; }
            /// <summary>
            /// 操作时间，unix时间戳
            /// </summary>
            public long time { get; set; }
            /// <summary>
            /// 完整客服帐号，格式为：帐号前缀@公众号微信号
            /// </summary>
            public string worker { get; set; }
        }

        #endregion


        #region 创建会话

        /// <summary>
        /// 创建会话, 此接口在客服和用户之间创建一个会话，如果该客服和用户会话已存在，则直接返回。指定的客服帐号必须已经绑定微信号且在线。
        /// </summary>
        /// <param name="kf_account">完整客服帐号，格式为：帐号前缀@公众号微信号</param>
        /// <param name="openid">粉丝的openid</param>
        /// <param name="config"></param>
        public static Task CreateSession(string kf_account, string openid, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/customservice/kfsession/create?$acac$", new { kf_account, openid }, config);
        }

        #endregion

        #region 关闭会话

        /// <summary>
        /// 关闭会话
        /// </summary>
        /// <param name="kf_account">完整客服帐号，格式为：帐号前缀@公众号微信号</param>
        /// <param name="openid">粉丝的openid</param>
        /// <param name="config"></param>
        public static Task CloseSession(string kf_account, string openid, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/customservice/kfsession/close?$acac$", new { kf_account, openid }, config);
        }

        #endregion

        #region 获取客户会话状态

        /// <summary>
        /// 获取一个客户的会话，如果不存在，则kf_account为空。
        /// </summary>
        /// <param name="openid">粉丝的openid</param>
        /// <param name="config"></param>
        public static Task<GetSessionResult> GetSession(string openid, ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetSessionResult>("https://api.weixin.qq.com/customservice/kfsession/getsession?$acac$&openid=" + Uri.EscapeDataString(openid), config);
        }

        public class GetSessionResult : IApiResult
        {
            /// <summary>
            /// 会话接入的时间, unix时间戳
            /// </summary>
            public long createtime { get; set; }
            /// <summary>
            /// 正在接待的客服，为空表示没有人在接待
            /// </summary>
            public string kf_account { get; set; }
        }

        #endregion

        #region 获取客服会话列表

        /// <summary>
        /// 获取客服会话列表
        /// </summary>
        /// <param name="kf_account">完整客服帐号，格式为：帐号前缀@公众号微信号</param>
        /// <param name="config"></param>
        public static Task<GetSessionListResult> GetSessionList(string kf_account, ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetSessionListResult>("https://api.weixin.qq.com/customservice/kfsession/getsessionlist?$acac$&kf_account=" + Uri.EscapeDataString(kf_account), config);
        }

        public class GetSessionListResult : IApiResult
        {
            public SessionRecord[] sessionlist { get; set; }
        }

        public class SessionRecord
        {
            /// <summary>
            /// 会话接入的时间, unix时间戳
            /// </summary>
            public long createtime { get; set; }
            /// <summary>
            /// 粉丝openid
            /// </summary>
            public string openid { get; set; }
        }

        #endregion

        #region 获取未接入会话列表

        /// <summary>
        /// 获取客服会话列表
        /// </summary>
        /// <param name="config"></param>
        public static Task<GetWaitCaseResult> GetWaitCase(ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetWaitCaseResult>("https://api.weixin.qq.com/customservice/kfsession/getwaitcase?$acac$", config);
        }

        public class GetWaitCaseResult : IApiResult
        {
            /// <summary>
            /// 未接入会话数量
            /// </summary>
            public int count { get; set; }
            /// <summary>
            /// 未接入会话列表，最多返回100条数据，按照来访顺序
            /// </summary>
            public WaitCase[] waitcaselist { get; set; }
        }

        public class WaitCase
        {
            /// <summary>
            /// 粉丝的最后一条消息的时间, unix时间戳
            /// </summary>
            public long latest_time { get; set; }
            /// <summary>
            /// 粉丝的openid
            /// </summary>
            public string openid { get; set; }
        }

        #endregion
    }
}
