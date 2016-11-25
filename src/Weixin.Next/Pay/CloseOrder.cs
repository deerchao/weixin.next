using System.Collections.Generic;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 关闭订单
    /// </summary>
    public class CloseOrder : PayApi<CloseOrder.Outcoming, CloseOrder.Incoming, CloseOrder.ErrorCode>
    {
        public CloseOrder(Requester requester, bool checkSignature, bool generateReport)
            : base(requester, checkSignature, generateReport)
        {
        }

        protected override string GetReportOutTradeNo(Outcoming outcoming, Incoming incoming)
        {
            return outcoming.out_trade_no;
        }

        protected override string GetReportDeviceNo(Outcoming outcoming)
        {
            return null;
        }

        protected override void GetApiUrl(Outcoming outcoming, out string interface_url, out bool requiresCert)
        {
            interface_url = "https://api.mch.weixin.qq.com/pay/closeorder";
            requiresCert = false;
        }

        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 商户系统内部的订单号，当没提供transaction_id时需要传这个。 
            /// </summary>
            public string out_trade_no { get; set; }

            public override IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser)
            {
                yield return new KeyValuePair<string, string>("out_trade_no", out_trade_no);
            }
        }

        public class Incoming : IncomingData<ErrorCode>
        {
            /// <summary>
            /// 调用接口提交的公众账号ID, 仅在return_code为SUCCESS的时候有意义
            /// </summary>
            public string appid { get; set; }
            /// <summary>
            /// 调用接口提交的商户号, 仅在return_code为SUCCESS的时候有意义
            /// </summary>
            public string mch_id { get; set; }

            /// <summary>
            /// 对于业务执行的详细描述，仅在return_code为SUCCESS的时候有意义
            /// </summary>
            public string result_msg { get; set; }


            protected override void DeserializeFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                appid = GetValue(values, "appid");
                mch_id = GetValue(values, "mch_id");
            }

            protected override void DeserializeSuccessFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                result_msg = GetValue(values, "result_msg");
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 订单已支付
            /// 订单已支付，不能发起关单
            /// 订单已支付，不能发起关单，请当作已支付的正常交易
            /// </summary>
            ORDERPAID,
            /// <summary>
            /// 系统错误
            /// 系统错误
            /// 系统异常，请重新调用该API
            /// </summary>
            SYSTEMERROR,
            /// <summary>
            /// 订单不存在
            /// 订单系统不存在此订单
            /// 不需要关单，当作未提交的支付的订单
            /// </summary>
            ORDERNOTEXIST,
            /// <summary>
            /// 订单已关闭
            /// 订单已关闭，无法重复关闭
            /// 订单已关闭，无需继续调用
            /// </summary>
            ORDERCLOSED,
            /// <summary>
            /// 签名错误
            /// 参数签名结果不正确
            /// 请检查签名参数和方法是否都符合签名算法要求
            /// </summary>
            SIGNERROR,
            /// <summary>
            /// 请使用post方法
            /// 未使用post传递参数 
            /// 请检查请求参数是否通过post方法提交
            /// </summary>
            REQUIRE_POST_METHOD,
            /// <summary>
            /// XML格式错误
            /// XML格式错误
            /// 请检查XML参数格式是否正确
            /// </summary>
            XML_FORMAT_ERROR,
        }
    }
}
