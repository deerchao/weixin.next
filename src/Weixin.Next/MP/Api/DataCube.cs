using System;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable PossibleLossOfFraction
    /// <summary>
    /// 数据分析
    /// </summary>
    public static class DataCube
    {
        #region Common
        private static object BuildRequestData(DateTime begin_date, DateTime end_date)
        {
            return new
            {
                begin_date = begin_date.ToString("yyyy-MM-dd"),
                end_date = end_date.ToString("yyyy-MM-dd"),
            };
        }

        public class DataCubeItem
        {
            /// <summary>
            /// 数据的日期
            /// </summary>
            public string ref_date { get; set; }

            public DateTime ParseDate()
            {
                return DateTime.Parse(ref_date);
            }
        }
        #endregion

        #region 接口分析
        /// <summary>
        /// 获取接口分析数据, 最大时间跨度: 30 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetInterfaceSummaryResult> GetInterfaceSummary(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetInterfaceSummaryResult>("https://api.weixin.qq.com/datacube/getinterfacesummary?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        /// <summary>
        /// 获取接口分析分时数据, 最大时间跨度: 1 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetInterfaceSummaryHourResult> GetInterfaceSummaryHour(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetInterfaceSummaryHourResult>("https://api.weixin.qq.com/datacube/getinterfacesummaryhour?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetInterfaceSummaryResult : IApiResult
        {
            public InterfaceSummaryItem[] list { get; set; }
        }

        public class GetInterfaceSummaryHourResult : IApiResult
        {
            public InterfaceSummaryHourItem[] list { get; set; }
        }

        public class InterfaceSummaryItem : DataCubeItem
        {
            /// <summary>
            /// 通过服务器配置地址获得消息后，被动回复用户消息的次数
            /// </summary>
            public int callback_count { get; set; }
            /// <summary>
            /// 被动回复用户消息的失败次数
            /// </summary>
            public int fail_count { get; set; }
            /// <summary>
            /// 总耗时，除以callback_count即为平均耗时
            /// </summary>
            public long total_time_cost { get; set; }
            /// <summary>
            /// 最大耗时
            /// </summary>
            public int max_time_cost { get; set; }
        }

        public class InterfaceSummaryHourItem : InterfaceSummaryItem
        {
            /// <summary>
            /// 数据的小时
            /// </summary>
            public int ref_hour { get; set; }


            public DateTime ParseTime()
            {
                return ParseDate().AddHours(ref_hour / 100);
            }
        }
        #endregion

        #region 用户分析
        /// <summary>
        /// 获取用户增减数据, 最大时间跨度: 7 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUserSummaryResult> GetUserSummary(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUserSummaryResult>("https://api.weixin.qq.com/datacube/getusersummary?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        /// <summary>
        /// 获取累计用户数据, 最大时间跨度: 7 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUserCumulateResult> GetUserCumulate(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUserCumulateResult>("https://api.weixin.qq.com/datacube/getusercumulate?$acac$", BuildRequestData(begin_date, end_date), config);
        }


        public class GetUserSummaryResult : IApiResult
        {
            public GetUserSummaryItem[] list { get; set; }
        }

        public class GetUserSummaryItem : DataCubeItem
        {
            /// <summary>
            /// 用户的渠道，数值代表的含义如下：
            /// <item><term>0</term><description>其他合计</description></item>
            /// <item><term>1</term><description>公众号搜索</description></item>
            /// <item><term>17</term><description>名片分享</description></item>
            /// <item><term>30</term><description>扫描二维码</description></item>
            /// <item><term>43</term><description>图文页右上角菜单</description></item>
            /// <item><term>51</term><description>支付后关注（在支付完成页）</description></item>
            /// <item><term>57</term><description>图文页内公众号名称</description></item>
            /// <item><term>75</term><description>公众号文章广告</description></item>
            /// <item><term>78</term><description>朋友圈广告</description></item>
            /// </summary>
            public int user_source { get; set; }
            /// <summary>
            /// 新增的用户数量
            /// </summary>
            public int new_user { get; set; }
            /// <summary>
            /// 取消关注的用户数量，new_user减去cancel_user即为净增用户数量
            /// </summary>
            public int cancel_user { get; set; }
        }

        public class GetUserCumulateResult : IApiResult
        {
            public GetUserCumulateItem[] list { get; set; }
        }

        public class GetUserCumulateItem : DataCubeItem
        {
            /// <summary>
            /// 总用户量
            /// </summary>
            public int cumulate_user { get; set; }
        }
        #endregion

        #region 图文分析
        /// <summary>
        /// 获取图文群发每日数据, 最大时间跨度: 1 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetArticleSummaryResult> GetArticleSummary(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetArticleSummaryResult>("https://api.weixin.qq.com/datacube/getarticlesummary?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetArticleSummaryResult : IApiResult
        {
            public GetArticleSummaryItem[] list { get; set; }
        }

        public class GetArticleSummaryItem : DataCubeItem
        {
            /// <summary>
            /// 请注意：这里的msgid实际上是由msgid（图文消息id，这也就是群发接口调用后返回的msg_data_id）和index（消息次序索引）组成， 例如12003_3， 其中12003是msgid，即一次群发的消息的id； 3为index，假设该次群发的图文消息共5个文章（因为可能为多图文），3表示5个中的第3个
            /// </summary>
            public string msgid { get; set; }
            /// <summary>
            /// 图文消息的标题
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 图文页（点击群发图文卡片进入的页面）的阅读人数
            /// </summary>
            public int int_page_read_user { get; set; }
            /// <summary>
            /// 图文页的阅读次数
            /// </summary>
            public int int_page_read_count { get; set; }
            /// <summary>
            /// 原文页（点击图文页“阅读原文”进入的页面）的阅读人数，无原文页时此处数据为0
            /// </summary>
            public int ori_page_read_user { get; set; }
            /// <summary>
            /// 原文页的阅读次数
            /// </summary>
            public int ori_page_read_count { get; set; }
            /// <summary>
            /// 分享的人数
            /// </summary>
            public int share_user { get; set; }
            /// <summary>
            /// 分享的次数
            /// </summary>
            public int share_count { get; set; }
            /// <summary>
            /// 收藏的人数
            /// </summary>
            public int add_to_fav_user { get; set; }
            /// <summary>
            /// 收藏的次数
            /// </summary>
            public int add_to_fav_count { get; set; }
        }


        /// <summary>
        /// 获取图文群发总数据, 最大时间跨度: 1 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetArticleTotalResult> GetArticleTotal(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetArticleTotalResult>("https://api.weixin.qq.com/datacube/getarticletotal?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetArticleTotalResult : IApiResult
        {
            public GetArticleTotalItem[] list { get; set; }
        }

        public class GetArticleTotalItem : DataCubeItem
        {
            /// <summary>
            /// 请注意：这里的msgid实际上是由msgid（图文消息id，这也就是群发接口调用后返回的msg_data_id）和index（消息次序索引）组成， 例如12003_3， 其中12003是msgid，即一次群发的消息的id； 3为index，假设该次群发的图文消息共5个文章（因为可能为多图文），3表示5个中的第3个
            /// </summary>
            public string msgid { get; set; }
            /// <summary>
            /// 图文消息的标题
            /// </summary>
            public string title { get; set; }

            public GetArticleTotalItemDetail[] details { get; set; }
        }

        public class GetArticleTotalItemDetail
        {
            /// <summary>
            /// 统计的日期，在getarticletotal接口中，ref_date指的是文章群发出日期， 而stat_date是数据统计日期
            /// </summary>
            public string stat_date { get; set; }
            /// <summary>
            /// 送达人数，一般约等于总粉丝数（需排除黑名单或其他异常情况下无法收到消息的粉丝）
            /// </summary>
            public int target_user;
            /// <summary>
            /// 图文页（点击群发图文卡片进入的页面）的阅读人数
            /// </summary>
            public int int_page_read_user;
            /// <summary>
            /// 图文页的阅读次数
            /// </summary>
            public int int_page_read_count;
            /// <summary>
            /// 原文页（点击图文页“阅读原文”进入的页面）的阅读人数，无原文页时此处数据为0
            /// </summary>
            public int ori_page_read_user;
            /// <summary>
            /// 原文页的阅读次数
            /// </summary>
            public int ori_page_read_count;
            /// <summary>
            /// 分享的人数
            /// </summary>
            public int share_user;
            /// <summary>
            /// 分享的次数
            /// </summary>
            public int share_count;
            /// <summary>
            /// 收藏的人数
            /// </summary>
            public int add_to_fav_user;
            /// <summary>
            /// 收藏的次数
            /// </summary>
            public int add_to_fav_count;
            /// <summary>
            /// 公众号会话阅读人数
            /// </summary>
            public int int_page_from_session_read_user;
            /// <summary>
            /// 公众号会话阅读次数
            /// </summary>
            public int int_page_from_session_read_count;
            /// <summary>
            /// 历史消息页阅读人数
            /// </summary>
            public int int_page_from_hist_msg_read_user;
            /// <summary>
            /// 历史消息页阅读次数
            /// </summary>
            public int int_page_from_hist_msg_read_count;
            /// <summary>
            /// 朋友圈阅读人数
            /// </summary>
            public int int_page_from_feed_read_user;
            /// <summary>
            /// 朋友圈阅读次数
            /// </summary>
            public int int_page_from_feed_read_count;
            /// <summary>
            /// 好友转发阅读人数
            /// </summary>
            public int int_page_from_friends_read_user;
            /// <summary>
            /// 好友转发阅读次数
            /// </summary>
            public int int_page_from_friends_read_count;
            /// <summary>
            /// 其他场景阅读人数
            /// </summary>
            public int int_page_from_other_read_user;
            /// <summary>
            /// 其他场景阅读次数 
            /// </summary>
            public int int_page_from_other_read_count;
            /// <summary>
            /// 公众号会话转发朋友圈人数
            /// </summary>
            public int feed_share_from_session_user;
            /// <summary>
            /// 公众号会话转发朋友圈次数
            /// </summary>
            public int feed_share_from_session_cnt;
            /// <summary>
            /// 朋友圈转发朋友圈人数
            /// </summary>
            public int feed_share_from_feed_user;
            /// <summary>
            /// 朋友圈转发朋友圈次数
            /// </summary>
            public int feed_share_from_feed_cnt;
            /// <summary>
            /// 其他场景转发朋友圈人数
            /// </summary>
            public int feed_share_from_other_user;
            /// <summary>
            /// 其他场景转发朋友圈次数
            /// </summary>
            public int feed_share_from_other_cnt;

            public DateTime ParseDate()
            {
                return DateTime.Parse(stat_date);
            }
        }


        /// <summary>
        /// 获取图文统计数据, 最大时间跨度: 3 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUserReadResult> GetUserRead(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUserReadResult>("https://api.weixin.qq.com/datacube/getuserread?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetUserReadResult : IApiResult
        {
            public GetUserReadItem[] list { get; set; }
        }

        public class GetUserReadItem : DataCubeItem
        {
            /// <summary>
            /// 图文页（点击群发图文卡片进入的页面）的阅读人数
            /// </summary>
            public int int_page_read_user { get; set; }
            /// <summary>
            /// 图文页的阅读次数
            /// </summary>
            public int int_page_read_count { get; set; }
            /// <summary>
            /// 原文页（点击图文页“阅读原文”进入的页面）的阅读人数，无原文页时此处数据为0
            /// </summary>
            public int ori_page_read_user { get; set; }
            /// <summary>
            /// 原文页的阅读次数
            /// </summary>
            public int ori_page_read_count { get; set; }
            /// <summary>
            /// 分享的人数
            /// </summary>
            public int share_user { get; set; }
            /// <summary>
            /// 分享的次数
            /// </summary>
            public int share_count { get; set; }
            /// <summary>
            /// 收藏的人数
            /// </summary>
            public int add_to_fav_user { get; set; }
            /// <summary>
            /// 收藏的次数
            /// </summary>
            public int add_to_fav_count { get; set; }
        }


        /// <summary>
        /// 获取图文统计分时数据, 最大时间跨度: 1 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUserReadHourResult> GetUserReadHour(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUserReadHourResult>("https://api.weixin.qq.com/datacube/getuserreadhour?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetUserReadHourResult : IApiResult
        {
            public GetUserReadHourItem[] list { get; set; }
        }

        public class GetUserReadHourItem : GetUserReadItem
        {
            /// <summary>
            /// 在获取图文阅读分时数据时才有该字段，代表用户从哪里进入来阅读该图文。0:会话;1.好友;2.朋友圈;3.腾讯微博;4.历史消息页;5.其他
            /// </summary>
            public int user_source { get; set; }

            /// <summary>
            /// 数据的小时
            /// </summary>
            public int ref_hour { get; set; }


            public DateTime ParseTime()
            {
                return ParseDate().AddHours(ref_hour / 100);
            }
        }


        /// <summary>
        /// 获取图文分享转发数据, 最大时间跨度: 7 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUserShareResult> GetUserShare(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUserShareResult>("https://api.weixin.qq.com/datacube/getusershare?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetUserShareResult : IApiResult
        {
            public GetUserShareItem[] list { get; set; }
        }

        public class GetUserShareItem : DataCubeItem
        {
            /// <summary>
            /// 分享的场景
            ///1代表好友转发 2代表朋友圈 3代表腾讯微博 255代表其他
            /// </summary>
            public int share_scene { get; set; }
            /// <summary>
            /// 分享的次数
            /// </summary>
            public int share_count { get; set; }
            /// <summary>
            /// 分享的人数
            /// </summary>
            public int share_user { get; set; }
        }


        /// <summary>
        /// 获取图文分享转发分时数据, 最大时间跨度: 1 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUserShareHourResult> GetUserShareHour(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUserShareHourResult>("https://api.weixin.qq.com/datacube/getusersharehour?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetUserShareHourResult : IApiResult
        {
            public GetUserShareHourItem[] list { get; set; }
        }

        public class GetUserShareHourItem : GetUserShareItem
        {
            /// <summary>
            /// 数据的小时
            /// </summary>
            public int ref_hour { get; set; }


            public DateTime ParseTime()
            {
                return ParseDate().AddHours(ref_hour / 100);
            }
        }
        #endregion

        #region 消息分析

        /// <summary>
        /// 获取消息发送概况数据, 最大时间跨度: 7 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUpstreamMsgResult> GetUpstreamMsg(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUpstreamMsgResult>("https://api.weixin.qq.com/datacube/getupstreammsg?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetUpstreamMsgResult : IApiResult
        {
            public GetUpstreamMsgItem[] list { get; set; }
        }

        public class GetUpstreamMsgItem : UpstreamMsgItem
        {
        }


        /// <summary>
        /// 获取消息分送分时数据, 最大时间跨度: 1 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUpstreamMsgHourResult> GetUpstreamMsgHour(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUpstreamMsgHourResult>("https://api.weixin.qq.com/datacube/getupstreammsghour?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetUpstreamMsgHourResult : IApiResult
        {
            public GetUpstreamMsgHourItem[] list { get; set; }
        }

        public class GetUpstreamMsgHourItem : UpstreamMsgItem
        {
        }


        /// <summary>
        /// 获取消息发送周数据, 最大时间跨度: 30 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUpstreamMsgWeekResult> GetUpstreamMsgWeek(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUpstreamMsgWeekResult>("https://api.weixin.qq.com/datacube/getupstreammsgweek?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetUpstreamMsgWeekResult : IApiResult
        {
            public GetUpstreamMsgWeekItem[] list { get; set; }
        }

        public class GetUpstreamMsgWeekItem : UpstreamMsgItem
        {
        }


        /// <summary>
        /// 获取消息发送月数据, 最大时间跨度: 30 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUpstreamMsgMonthResult> GetUpstreamMsgMonth(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUpstreamMsgMonthResult>("https://api.weixin.qq.com/datacube/getupstreammsgmonth?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetUpstreamMsgMonthResult : IApiResult
        {
            public GetUpstreamMsgMonthItem[] list { get; set; }
        }

        public class GetUpstreamMsgMonthItem : UpstreamMsgItem
        {
        }


        /// <summary>
        /// 获取消息发送分布数据, 最大时间跨度: 15 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUpstreamMsgDistResult> GetUpstreamMsgDist(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUpstreamMsgDistResult>("https://api.weixin.qq.com/datacube/getupstreammsgdist?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetUpstreamMsgDistResult : IApiResult
        {
            public GetUpstreamMsgDistItem[] list { get; set; }
        }

        public class GetUpstreamMsgDistItem : UpstreamMsgDist
        {
        }


        /// <summary>
        /// 获取消息发送分布周数据, 最大时间跨度: 30 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUpstreamMsgDistWeekResult> GetUpstreamMsgDistWeek(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUpstreamMsgDistWeekResult>("https://api.weixin.qq.com/datacube/getupstreammsgdistweek?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetUpstreamMsgDistWeekResult : IApiResult
        {
            public GetUpstreamMsgDistWeekItem[] list { get; set; }
        }

        public class GetUpstreamMsgDistWeekItem : UpstreamMsgDist
        {
        }


        /// <summary>
        /// 获取消息发送分布月数据, 最大时间跨度: 30 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetUpstreamMsgDistMonthResult> GetUpstreamMsgDistMonth(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetUpstreamMsgDistMonthResult>("https://api.weixin.qq.com/datacube/getupstreammsgdistmonth?$acac$", BuildRequestData(begin_date, end_date), config);
        }

        public class GetUpstreamMsgDistMonthResult : IApiResult
        {
            public GetUpstreamMsgDistMonthItem[] list { get; set; }
        }

        public class GetUpstreamMsgDistMonthItem : UpstreamMsgDist
        {
        }

        public class UpstreamMsgItem : DataCubeItem
        {
            /// <summary>
            /// 消息类型，代表含义如下：
            /// 1代表文字 2代表图片 3代表语音 4代表视频 6代表第三方应用消息（链接消息）
            /// </summary>
            public int msg_type { get; set; }
            /// <summary>
            /// 上行发送了（向公众号发送了）消息的用户数
            /// </summary>
            public int msg_user { get; set; }
            /// <summary>
            /// 上行发送了消息的消息总数
            /// </summary>
            public int msg_count { get; set; }
        }

        public class UpstreamMsgHourItem : UpstreamMsgItem
        {
            /// <summary>
            /// 数据的小时
            /// </summary>
            public int ref_hour { get; set; }


            public DateTime ParseTime()
            {
                return ParseDate().AddHours(ref_hour / 100);
            }
        }

        public class UpstreamMsgDist : DataCubeItem
        {
            /// <summary>
            /// 当日发送消息量分布的区间，0代表 “0”，1代表“1-5”，2代表“6-10”，3代表“10次以上”
            /// </summary>
            public int count_interval { get; set; }
            /// <summary>
            /// 上行发送了（向公众号发送了）消息的用户数
            /// </summary>
            public int msg_user { get; set; }
        }
        #endregion
    }
}
