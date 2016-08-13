using System;

namespace Weixin.Next
{
    public static class TimeHelper
    {
        /// <summary>
        /// 将 DateTime 转换为 unix 时间戳. 如 DateTime.Kind 不为 Utc, 则被视为本地时间
        /// </summary>
        /// <param name="time">DateTime 值</param>
        /// <returns></returns>
        public static long ToUnixTimestamp(this DateTime time)
        {
            time = time.ToUniversalTime();
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (time.Ticks - origin.Ticks) / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 将 DateTime 转换为 unix 时间戳. 如 DateTime.Kind 不为 Utc, 则被视为本地时间
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="kind">日期类别, Local 或 Utc</param>
        /// <returns></returns>
        public static DateTime FromUnixTimestamp(this long timestamp, DateTimeKind kind = DateTimeKind.Local)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var result = origin.AddMilliseconds(timestamp);

            return kind == DateTimeKind.Utc
                       ? result
                       : result.ToLocalTime();
        }
    }
}
