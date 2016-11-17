using System;
using System.Linq;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信用户信息
    /// </summary>
    public static class User
    {
        #region 获取用户基本信息(UnionID机制)

        /// <summary>
        /// 获取用户基本信息(UnionID机制)
        /// </summary>
        /// <param name="openid">普通用户的标识，对当前公众号唯一</param>
        /// <param name="lang">返回国家地区语言版本，zh_CN 简体，zh_TW 繁体，en 英语</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetInfoResult> GetInfo(string openid, string lang = "zh_CN", ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetInfoResult>($"https://api.weixin.qq.com/cgi-bin/user/info?$acac$&openid={Uri.EscapeDataString(openid)}&lang={Uri.EscapeDataString(lang)}", config);
        }

        public class GetInfoResult : UserInfo, IApiResult
        {
        }

        #endregion

        public class UserInfo : UserBase
        {
            /// <summary>
            /// 用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
            /// </summary>
            public int subscribe { get; set; }
            /// <summary>
            /// 用户的语言，简体中文为zh_CN
            /// </summary>
            public string language { get; set; }
            /// <summary>
            /// 用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间
            /// </summary>
            public long subscribe_time { get; set; }
            /// <summary>
            /// 公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
            /// </summary>
            public string remark { get; set; }
            /// <summary>
            /// 用户所在的分组ID（兼容旧的用户分组接口）
            /// </summary>
            public int groupid { get; set; }
            /// <summary>
            /// 用户被打上的标签ID列表
            /// </summary>
            public int[] tagid_list { get; set; }
        }

        #region 批量获取用户基本信息

        /// <summary>
        /// 批量获取用户基本信息。最多支持一次拉取100条。
        /// </summary>
        /// <param name="openid">用户标识列表</param>
        /// <param name="lang">与用户标识对应的语言列表, null 代表全部使用 zh_CN</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<BatchGetInfoResult> BatchGet(string[] openid, string[] lang = null, ApiConfig config = null)
        {
            var user_list = openid.Zip(lang ?? Enumerable.Repeat("zh_CN", openid.Length), (o, l) => new { openid = o, lang = l }).ToArray();
            return ApiHelper.PostResult<BatchGetInfoResult>("https://api.weixin.qq.com/cgi-bin/user/info/batchget?$acac$", new { user_list }, config);
        }

        public class BatchGetInfoResult : IApiResult
        {
            public UserInfo[] user_info_list { get; set; }
        }

        #endregion

        #region 获取用户列表

        /// <summary>
        /// <para>公众号可通过本接口来获取帐号的关注者列表，关注者列表由一串OpenID（加密后的微信号，每个用户对每个公众号的OpenID是唯一的）组成。一次拉取调用最多拉取10000个关注者的OpenID，可以通过多次拉取的方式来满足需求。</para>
        /// <para>当公众号关注者数量超过10000时，可通过填写next_openid的值，从而多次拉取列表的方式来满足需求。具体而言，就是在调用接口时，将上一次调用得到的返回中的next_openid值，作为下一次调用中的next_openid值。</para>
        /// </summary>
        /// <param name="next_openid"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetOpenListResult> GetList(string next_openid = null, ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetOpenListResult>($"https://api.weixin.qq.com/cgi-bin/user/get?$acac$&next_openid={next_openid}", config);
        }

        #endregion

        #region 设置用户备注名
        /// <summary>
        /// 设置用户备注名
        /// </summary>
        /// <param name="openid">用户标识</param>
        /// <param name="remark">新的备注名，长度必须小于30字符 </param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task UpdateRemark(string openid, string remark, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/user/info/updateremark?$acac$", new { openid, remark }, config);
        }
        #endregion

        #region 获取标签下粉丝列表
        /// <summary>
        /// 获取标签下粉丝列表
        /// </summary>
        /// <param name="tagid">标签id</param>
        /// <param name="next_openid">第一个拉取的OPENID，不填默认从头开始拉取</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetListByTagResult> GetListByTag(int tagid, string next_openid = null, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetListByTagResult>("https://api.weixin.qq.com/cgi-bin/user/tag/get?$acac$", new { tagid, next_openid }, config);
        }

        public class GetListByTagResult : IApiResult
        {
            /// <summary>
            /// 这次获取的粉丝数量
            /// </summary>
            public int count { get; set; }
            /// <summary>
            /// 粉丝列表
            /// </summary>
            public OpenIdList data { get; set; }
            /// <summary>
            /// 拉取列表最后一个用户的openid
            /// </summary>
            public string next_openid { get; set; }
        }
        #endregion

        #region 批量为用户打标签
        /// <summary>
        /// 批量为用户打标签
        /// </summary>
        /// <param name="openid_list">要打标签的用户标识列表</param>
        /// <param name="tagid">要打的标签id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task BatchTagging(string[] openid_list, int tagid, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/tags/members/batchtagging?$acac$", new { openid_list, tagid }, config);
        }
        #endregion

        #region 批量为用户取消标签
        /// <summary>
        /// 批量为用户取消标签
        /// </summary>
        /// <param name="openid_list">要取消标签的用户标识列表</param>
        /// <param name="tagid">要取消的标签id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task BatchUntagging(string[] openid_list, int tagid, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/tags/members/batchuntagging?$acac$", new { openid_list, tagid }, config);
        }
        #endregion

        #region 获取用户身上的标签列表

        /// <summary>
        /// 获取用户身上的标签列表
        /// </summary>
        /// <param name="openid">用户标识</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetTagIdListResult> GetTagIdList(string openid, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetTagIdListResult>("https://api.weixin.qq.com/cgi-bin/tags/members/getidlist?$acac$", new { openid }, config);
        }

        public class GetTagIdListResult : IApiResult
        {
            /// <summary>
            /// 被置上的标签列表
            /// </summary>
            public int[] tagid_list { get; set; }
        }
        #endregion
    }


    public class GetOpenListResult : IApiResult
    {
        /// <summary>
        /// 关注该公众账号的总用户数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 拉取的OPENID个数，最大值为10000
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 列表数据，OPENID的列表
        /// </summary>
        public OpenIdList data { get; set; }
        /// <summary>
        /// 拉取列表的最后一个用户的OPENID
        /// </summary>
        public string next_openid { get; set; }
    }

    public class OpenIdList
    {
        public string[] openid { get; set; }
    }

}
