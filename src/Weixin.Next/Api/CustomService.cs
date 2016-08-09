using System.Threading.Tasks;

namespace Weixin.Next.Api
{
    public class CustomService
    {
        #region 获取客服基本信息
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
            public int invite_expire_time { get; set; }
            /// <summary>
            /// 邀请的状态，有等待确认“waiting”，被拒绝“rejected”，过期“expired”
            /// </summary>
            public string invite_status { get; set; }
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
    }
}
