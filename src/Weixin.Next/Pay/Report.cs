using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 交易保障
    /// </summary>
    public class Report
    {
        private readonly Requester _requester;

        public Report(Requester requester)
        {
            _requester = requester;
        }

        public Task<Incoming> Invoke(Outcoming outcoming)
        {
            return _requester.SendRequest<Incoming, ErrorCode>("https://api.mch.weixin.qq.com/payitil/report", false, outcoming, false);
        }

        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 可选, 终端设备号(门店号或收银设备ID)，注意：PC网页或公众号内支付请传"WEB"
            /// </summary>
            public string device_info { get; set; }
            /// <summary>
            /// 报对应的接口的完整URL
            /// </summary>
            public string interface_url { get; set; }
            /// <summary>
            /// 接口耗时情况，单位为毫秒
            /// </summary>
            public int execute_time { get; set; }
            /// <summary>
            /// 调用结果的 return_code
            /// </summary>
            public string return_code { get; set; }
            /// <summary>
            /// 调用结果的 return_msg
            /// </summary>
            public string return_msg { get; set; }
            /// <summary>
            /// 调用结果的 result_code
            /// </summary>
            public string result_code { get; set; }
            /// <summary>
            /// 调用结果的 err_code
            /// </summary>
            public string err_code { get; set; }
            /// <summary>
            /// 调用结果的 err_code_des
            /// </summary>
            public string err_code_des { get; set; }
            /// <summary>
            /// 商户系统内部的订单号,商户可以在上报时提供相关商户订单号方便微信支付更好的提高服务质量。 
            /// </summary>
            public string out_trade_no { get; set; }
            /// <summary>
            /// 发起接口调用时的机器IP 
            /// </summary>
            public string user_ip { get; set; }
            /// <summary>
            /// 系统时间，格式为yyyyMMddHHmmss
            /// </summary>
            public DateTime time { get; set; }

            public override IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser)
            {
                yield return new KeyValuePair<string, string>("device_info", device_info);
                yield return new KeyValuePair<string, string>("interface_url", interface_url);
                yield return new KeyValuePair<string, string>("execute_time", execute_time.ToString("D"));
                yield return new KeyValuePair<string, string>("return_code", return_code);
                yield return new KeyValuePair<string, string>("return_msg", return_msg);
                yield return new KeyValuePair<string, string>("result_code", result_code);
                yield return new KeyValuePair<string, string>("err_code", err_code);
                yield return new KeyValuePair<string, string>("err_code_des", err_code_des);
                yield return new KeyValuePair<string, string>("out_trade_no", out_trade_no);
                yield return new KeyValuePair<string, string>("user_ip", user_ip);
                yield return new KeyValuePair<string, string>("time", time.ToString("格式为yyyyMMddHHmmss"));
            }
        }

        public class Incoming : IncomingData<ErrorCode>
        {
        }

        public enum ErrorCode
        {
            
        }
    }
}
