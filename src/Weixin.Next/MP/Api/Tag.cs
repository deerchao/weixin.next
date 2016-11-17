using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信用户标签
    /// </summary>
    public static class Tag
    {
        #region 创建标签

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="name">标签名（30个字符以内）</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<CreateTagResult> Create(string name, ApiConfig config = null)
        {
            return ApiHelper.PostResult<CreateTagResult>("https://api.weixin.qq.com/cgi-bin/tags/create?$acac$", new { tag = new { name } }, config);
        }

        public class CreateTagResult : IApiResult
        {
            public TagInfo tag { get; set; }
        }
        #endregion


        public class TagInfo
        {
            /// <summary>
            /// 标签id，由微信分配
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 标签名，UTF8编码
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 此标签下粉丝数
            /// </summary>
            public int count { get; set; }
        }

        #region 获取公众号已创建的标签

        /// <summary>
        /// 获取公众号已创建的标签
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetTagListResult> GetList(ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetTagListResult>("https://api.weixin.qq.com/cgi-bin/tags/get?$acac$", config);
        }

        public class GetTagListResult : IApiResult
        {
            public TagInfo[] tags { get; set; }
        }
        #endregion

        #region 编辑标签
        /// <summary>
        /// 编辑标签
        /// </summary>
        /// <param name="id">标签id</param>
        /// <param name="name">标签名</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task Update(int id, string name, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/tags/update?$acac$", new { tag = new { id, name } }, config);
        }
        #endregion

        #region 删除标签

        /// <summary>
        /// <para>删除标签</para>
        /// <para>注意，当某个标签下的粉丝超过10w时，后台不可直接删除标签。此时，开发者可以对该标签下的openid列表，先进行取消标签的操作，直到粉丝数不超过10w后，才可直接删除该标签。</para>
        /// </summary>
        /// <param name="id">标签id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task Delete(int id, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/tags/delete?$acac$", new { tag = new { id } }, config);
        }
        #endregion
    }
}

