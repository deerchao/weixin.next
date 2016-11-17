using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信黑名单
    /// </summary>
    public static class Blacklist
    {
        /// <summary>
        /// <para>公众号可通过该接口来获取帐号的黑名单列表，黑名单列表由一串 OpenID（加密后的微信号，每个用户对每个公众号的OpenID是唯一的）组成。</para>
        /// <para>该接口每次调用最多可拉取 10000 个OpenID，当列表数较多时，可以通过多次拉取的方式来满足需求。</para>
        /// </summary>
        /// <param name="begin_openid"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetOpenListResult> GetList(string begin_openid = null, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetOpenListResult>("https://api.weixin.qq.com/cgi-bin/tags/members/getblacklist?$acac$", new { begin_openid }, config);
        }

        /// <summary>
        /// 批量拉黑用户
        /// </summary>
        /// <param name="openid_list">需要拉入黑名单的用户的openid，一次拉黑最多允许20个</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task AddUsers(string[] openid_list, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/tags/members/batchblacklist?$acac$", new { openid_list }, config);
        }

        /// <summary>
        /// 批量取消拉黑用户
        /// </summary>
        /// <param name="openid_list">需要取消黑名单的用户的openid，一次最多允许20个</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task RemoveUsers(string[] openid_list, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/tags/members/batchunblacklist?$acac$", new { openid_list }, config);
        }
    }
}
