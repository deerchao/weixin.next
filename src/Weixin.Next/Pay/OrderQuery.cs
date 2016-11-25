using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 查询订单
    /// </summary>
    public class OrderQuery : PayApi<OrderQuery.Outcoming, OrderQuery.Incoming, OrderQuery.ErrorCode>
    {
        public OrderQuery(Requester requester, bool checkSignature, bool generateReport)
            : base(requester, checkSignature, generateReport)
        {
        }

        protected override void GetApiUrl(Outcoming outcoming, out string interface_url, out bool requiresCert)
        {
            interface_url = "https://api.mch.weixin.qq.com/pay/orderquery";
            requiresCert = false;
        }

        protected override string GetReportOutTradeNo(Outcoming outcoming, Incoming incoming)
        {
            return outcoming.out_trade_no ?? incoming.out_trade_no;
        }

        protected override string GetReportDeviceNo(Outcoming outcoming)
        {
            return null;
        }

        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 微信的订单号，优先使用 
            /// </summary>
            public string transaction_id { get; set; }
            /// <summary>
            /// 商户系统内部的订单号，当没提供transaction_id时需要传这个。 
            /// </summary>
            public string out_trade_no { get; set; }

            public override IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser)
            {
                yield return new KeyValuePair<string, string>("transaction_id", transaction_id);
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
            /// 微信支付分配的终端设备号，仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public string device_info { get; set; }
            /// <summary>
            /// 用户在商户appid下的唯一标识 ，仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 用户是否关注公众账号，Y-关注，N-未关注，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public bool is_subscribe { get; set; }
            /// <summary>
            /// 调用接口提交的交易类型，仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public TradeType trade_type { get; set; }
            /// <summary>
            /// 交易状态，仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public TradeState trade_state { get; set; }
            /// <summary>
            /// 付款银行，采用字符串类型的银行标识, 仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public Bank bank_type { get; set; }
            /// <summary>
            /// 订单总金额，单位为分, 仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public int total_fee { get; set; }
            /// <summary>
            /// 应结订单金额=订单金额-非充值代金券金额，应结订单金额&lt;=订单金额，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public int? settlement_total_fee { get; set; }
            /// <summary>
            /// 货币类型，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public Currency fee_type { get; set; }
            /// <summary>
            /// 现金支付金额订单现金支付金额，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public int cash_fee { get; set; }
            /// <summary>
            /// 现金支付货币类型，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public Currency cash_fee_type { get; set; }
            /// <summary>
            /// “代金券”金额&lt;=订单金额，订单金额-“代金券”金额=现金支付金额，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public int? coupon_fee { get; set; }
            /// <summary>
            /// 代金券使用数量，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public int? coupon_count { get; set; }
            /// <summary>
            /// 使用的代金券，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public CouponPayment[] coupons { get; set; }
            /// <summary>
            /// 微信支付订单号，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public string transaction_id { get; set; }
            /// <summary>
            /// 商户系统的订单号，与请求一致，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public string out_trade_no { get; set; }
            /// <summary>
            /// 附加数据，原样返回，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public string attach { get; set; }
            /// <summary>
            /// 订单支付时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public DateTime time_end { get; set; }
            /// <summary>
            /// 对当前查询订单状态的描述和下一步操作的指引, 如: 支付失败，请重新下单支付，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public string trade_state_desc { get; set; }

            protected override void DeserializeFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                appid = GetValue(values, "appid");
                mch_id = GetValue(values, "mch_id");
            }

            protected override void DeserializeSuccessFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                device_info = GetValue(values, "device_info");
                openid = GetValue(values, "openid");
                is_subscribe = GetValue(values, "is_subscribe") == "Y";
                trade_type = (TradeType)Enum.Parse(typeof(TradeType), GetValue(values, "trade_type"));
                trade_state = (TradeState)Enum.Parse(typeof(TradeState), GetValue(values, "trade_state"));
                bank_type = Bank.Find(GetValue(values, "bank_type"));
                total_fee = GetIntValue(values, "total_fee") ?? 0;
                settlement_total_fee = GetIntValue(values, "settlement_total_fee");
                fee_type = Currency.Find(GetValue(values, "fee_type"));
                cash_fee = GetIntValue(values, "cash_fee") ?? 0;
                cash_fee_type = Currency.Find(GetValue(values, "cash_fee_type"));
                coupon_fee = GetIntValue(values, "coupon_fee");
                coupon_count = GetIntValue(values, "coupon_count");
                transaction_id = GetValue(values, "transaction_id");
                out_trade_no = GetValue(values, "out_trade_no");
                attach = GetValue(values, "attach");
                time_end = DateTime.ParseExact(GetValue(values, "time_end"), "yyyMMddHHmmss", null);
                trade_state_desc = GetValue(values, "trade_state_desc");

                if (coupon_count > 0)
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    coupons = new CouponPayment[coupon_count.Value];

                    for (var n = 0; n < coupon_count.Value; n++)
                    {
                        var sn = "_$" + n.ToString("D");
                        coupons[n] = new CouponPayment
                        {
                            coupon_type = (CouponType)Enum.Parse(typeof(CouponType), GetValue(values, "coupon_type" + sn)),
                            coupon_id = GetValue(values, "coupon_id" + sn),
                            coupon_fee = GetIntValue(values, "coupon_fee" + sn) ?? 0,
                        };
                    }
                }
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 此交易订单号不存在 
            /// 查询系统中不存在此交易订单号 
            /// 该API只能查提交支付交易返回成功的订单，请商户检查需要查询的订单号是否正确 
            /// </summary>
            ORDERNOTEXIST,
            /// <summary>
            /// 系统错误 
            /// 后台系统返回错误 
            /// 系统异常，请再调用发起查询
            /// </summary>
            SYSTEMERROR,
        }

        public enum TradeState
        {
            /// <summary>
            /// 支付成功
            /// </summary>
            SUCCESS,
            /// <summary>
            /// 转入退款
            /// </summary>
            REFUND,
            /// <summary>
            /// 未支付
            /// </summary>
            NOTPAY,
            /// <summary>
            /// 已关闭
            /// </summary>
            CLOSED,
            /// <summary>
            /// 已撤销（刷卡支付） 
            /// </summary>
            REVOKED,
            /// <summary>
            /// 用户支付中
            /// </summary>
            USERPAYING,
            /// <summary>
            /// 支付失败(其他原因，如银行返回失败)
            /// </summary>
            PAYERROR
        }

        public class CouponPayment
        {
            /// <summary>
            /// 代金券类型
            /// </summary>
            public CouponType coupon_type { get; set; }
            /// <summary>
            /// 代金券ID
            /// </summary>
            public string coupon_id { get; set; }
            /// <summary>
            /// 该代金券支付的金额
            /// </summary>
            public int coupon_fee { get; set; }
        }
    }
}
