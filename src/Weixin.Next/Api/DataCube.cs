using System;
using System.Threading.Tasks;

namespace Weixin.Next.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 接口分析
    /// </summary>
    public static class DataCube
    {
        /// <summary>
        /// 获取接口分析数据, 最大时间跨度: 30 (天)
        /// </summary>
        /// <param name="begin_date">获取数据的起始日期，begin_date和end_date的差值需小于“最大时间跨度”（比如最大时间跨度为1时，begin_date和end_date的差值只能为0，才能小于1），否则会报错</param>
        /// <param name="end_date">获取数据的结束日期，end_date允许设置的最大值为昨日</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetInterfaceSummaryResult> GetInterfaceSummary(DateTime begin_date, DateTime end_date, ApiConfig config = null)
        {
            return ApiHelper.PostResult<GetInterfaceSummaryResult>("https://api.weixin.qq.com/datacube/getinterfacesummary?$acac$", new
            {
                begin_date = begin_date.ToString("yyyy-MM-dd"),
                end_date = end_date.ToString("yyyy-MM-dd"),
            }, config);
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
            return ApiHelper.PostResult<GetInterfaceSummaryHourResult>("https://api.weixin.qq.com/datacube/getinterfacesummaryhour?$acac$", new
            {
                begin_date = begin_date.ToString("yyyy-MM-dd"),
                end_date = end_date.ToString("yyyy-MM-dd"),
            }, config);
        }

        public class GetInterfaceSummaryResult : IApiResult
        {
            public InterfaceSummaryItem[] list { get; set; }
        }

        public class GetInterfaceSummaryHourResult : IApiResult
        {
            public InterfaceSummaryHourItem[] list { get; set; }
        }

        public class InterfaceSummaryItem
        {
            /// <summary>
            /// 数据的日期
            /// </summary>
            public string ref_date { get; set; }
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


            public DateTime ParseDate()
            {
                return DateTime.Parse(ref_date);
            }
        }

        public class InterfaceSummaryHourItem : InterfaceSummaryItem
        {
            /// <summary>
            /// 数据的小时
            /// </summary>
            public int ref_hour { get; set; }


            public DateTime ParseTime()
            {
                return ParseDate().AddHours(ref_hour);
            }
        }
    }
}
