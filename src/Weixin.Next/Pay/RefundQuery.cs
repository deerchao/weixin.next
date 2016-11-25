using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 查询退款
    /// </summary>
    public class RefundQuery : PayApi<RefundQuery.Outcoming, RefundQuery.Incoming, RefundQuery.ErrorCode>
    {
        public RefundQuery(Requester requester, bool checkSignature, bool generateReport)
            : base(requester, checkSignature, generateReport)
        {
        }

        protected override string GetReportOutTradeNo(Outcoming outcoming, Incoming incoming)
        {
            return outcoming.out_trade_no;
        }

        protected override string GetReportDeviceNo(Outcoming outcoming)
        {
            return outcoming.device_info;
        }

        protected override void GetApiUrl(Outcoming outcoming, out string interface_url, out bool requiresCert)
        {
            interface_url = "https://api.mch.weixin.qq.com/pay/refundquery";
            requiresCert = false;
        }

        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 可选, 商户自定义的终端设备号，如门店编号、设备的ID等
            /// </summary>
            public string device_info { get; set; }
            /// <summary>
            /// 微信的订单号， transaction_id, out_trade_no, out_refund_no, refund_id 四个应提供一个
            /// </summary>
            public string transaction_id { get; set; }
            /// <summary>
            /// 商户系统内部的订单号， transaction_id, out_trade_no, out_refund_no, refund_id 四个应提供一个
            /// </summary>
            public string out_trade_no { get; set; }
            /// <summary>
            /// 商户侧传给微信的退款单号， transaction_id, out_trade_no, out_refund_no, refund_id 四个应提供一个
            /// </summary>
            public string out_refund_no { get; set; }
            /// <summary>
            /// 微信生成的退款单号，在申请退款接口有返回， transaction_id, out_trade_no, out_refund_no, refund_id 四个应提供一个
            /// </summary>
            public string refund_id { get; set; }

            public override IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser)
            {
                yield return new KeyValuePair<string, string>("transaction_id", transaction_id);
                yield return new KeyValuePair<string, string>("out_trade_no", out_trade_no);
                yield return new KeyValuePair<string, string>("out_refund_no", out_refund_no);
                yield return new KeyValuePair<string, string>("refund_id", refund_id);
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
            /// 微信支付订单号，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public string transaction_id { get; set; }
            /// <summary>
            /// 商户系统的订单号，与请求一致，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public string out_trade_no { get; set; }
            /// <summary>
            /// 订单总金额，单位为分, 仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public int? total_fee { get; set; }
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
            public int? cash_fee { get; set; }
            /// <summary>
            /// 退款记录数
            /// </summary>
            public int? refund_count { get; set; }

            public Refund[] refunds { get; set; }

            protected override void DeserializeFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                appid = GetValue(values, "appid");
                mch_id = GetValue(values, "mch_id");
            }

            protected override void DeserializeSuccessFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                device_info = GetValue(values, "device_info");
                total_fee = GetIntValue(values, "total_fee") ?? 0;
                settlement_total_fee = GetIntValue(values, "settlement_total_fee");
                fee_type = Currency.Find(GetValue(values, "fee_type"));
                cash_fee = GetIntValue(values, "cash_fee") ?? 0;
                transaction_id = GetValue(values, "transaction_id");
                out_trade_no = GetValue(values, "out_trade_no");
                refund_count = GetIntValue(values, "refund_count");

                if (refund_count > 0)
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    refunds = new Refund[refund_count.Value];

                    for (var n = 0; n < refund_count.Value; n++)
                    {
                        var sn = "_$" + n.ToString("D");
                        refunds[n] = new Refund
                        {
                            out_refund_no = GetValue(values, "out_refund_no" + sn),
                            refund_id = GetValue(values, "refund_id" + sn),
                            refund_channel = (RefundChannel)Enum.Parse(typeof(RefundChannel), GetValue(values, "refund_channel" + sn)),
                            refund_fee = GetIntValue(values, "refund_fee" + sn) ?? 0,
                            settlement_refund_fee = GetIntValue(values, "settlement_refund_fee" + sn) ?? 0,
                            refund_account = (RefundSource)Enum.Parse(typeof(RefundSource), GetValue(values, "refund_account" + sn)),
                            coupon_type = (CouponType)Enum.Parse(typeof(CouponType), GetValue(values, "coupon_type" + sn)),
                            coupon_refund_fee = GetIntValue(values, "coupon_refund_fee" + sn) ?? 0,
                            coupon_refund_count = GetIntValue(values, "coupon_refund_count" + sn) ?? 0,
                            refund_status = (RefundStatus)Enum.Parse(typeof(RefundStatus), GetValue(values, "refund_status" + sn)),
                            refund_recv_accout = GetValue(values, "refund_recv_accout" + sn),
                        };

                        if (refunds[n].coupon_refund_count > 0)
                        {
                            refunds[n].coupons = new RefundCoupon[refunds[n].coupon_refund_count];

                            for (var m = 0; m < refunds[n].coupon_refund_count; m++)
                            {
                                var snm = sn + "_" + m.ToString("D");

                                refunds[n].coupons[m] = new RefundCoupon
                                {
                                    coupon_refund_batch_id = GetValue(values, "coupon_refund_batch_id" + snm),
                                    coupon_refund_id = GetValue(values, "coupon_refund_id" + snm),
                                    coupon_refund_fee = GetIntValue(values, "coupon_refund_fee" + snm) ?? 0,
                                };
                            }
                        }
                    }
                }
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 接口返回错误
            /// 系统超时
            /// 请尝试再次掉调用API。
            /// </summary>
            SYSTEMERROR,
            /// <summary>
            /// 退款订单查询失败
            /// 订单号错误或订单状态不正确
            /// 请检查订单号是否有误以及订单状态是否正确，如：未支付、已支付未退款
            /// </summary>
            REFUNDNOTEXIST,
            /// <summary>
            /// 无效transaction_id
            /// 请求参数未按指引进行填写
            /// 请求参数错误，检查原交易号是否存在或发起支付交易接口返回失败
            /// </summary>
            INVALID_TRANSACTIONID,
            /// <summary>
            /// 参数错误
            /// 请求参数未按指引进行填写
            /// 请求参数错误，请检查参数再调用退款申请
            /// </summary>
            PARAM_ERROR,
            /// <summary>
            /// APPID不存在
            /// 参数中缺少APPID
            /// 请检查APPID是否正确
            /// </summary>
            APPID_NOT_EXIST,
            /// <summary>
            /// MCHID不存在
            /// 参数中缺少MCHID
            /// 请检查MCHID是否正确
            /// </summary>
            MCHID_NOT_EXIST,
            /// <summary>
            /// appid和mch_id不匹配
            /// appid和mch_id不匹配
            /// 请确认appid和mch_id是否匹配
            /// </summary>
            APPID_MCHID_NOT_MATCH,
            /// <summary>
            /// 请使用post方法
            /// 未使用post传递参数 
            /// 请检查请求参数是否通过post方法提交
            /// </summary>
            REQUIRE_POST_METHOD,
            /// <summary>
            /// 签名错误
            /// 参数签名结果不正确
            /// 请检查签名参数和方法是否都符合签名算法要求
            /// </summary>
            SIGNERROR,
            /// <summary>
            /// XML格式错误
            /// XML格式错误
            /// 请检查XML参数格式是否正确
            /// </summary>
            XML_FORMAT_ERROR,
        }

        public enum RefundStatus
        {
            /// <summary>
            /// 退款成功 
            /// </summary>
            SUCCESS,
            /// <summary>
            /// 退款失败 
            /// </summary>
            FAIL,
            /// <summary>
            /// 退款处理中 
            /// </summary>
            PROCESSING,
            /// <summary>
            /// 转入代发，退款到银行发现用户的卡作废或者冻结了，导致原路退款银行卡失败，资金回流到商户的现金帐号，需要商户人工干预，通过线下或者财付通转账的方式进行退款。
            /// </summary>
            CHANGE,
        }

        public class Refund
        {
            public string out_refund_no { get; set; }
            public string refund_id { get; set; }
            public RefundChannel refund_channel { get; set; }
            public int refund_fee { get; set; }
            public int settlement_refund_fee { get; set; }
            public RefundSource refund_account { get; set; }
            public CouponType coupon_type { get; set; }
            public int coupon_refund_fee { get; set; }
            public int coupon_refund_count { get; set; }
            public RefundCoupon[] coupons { get; set; }
            public RefundStatus refund_status { get; set; }
            public string refund_recv_accout { get; set; }
        }

        public class RefundCoupon
        {
            public string coupon_refund_batch_id { get; set; }
            public string coupon_refund_id { get; set; }
            public int coupon_refund_fee { get; set; }
        }
    }
}
