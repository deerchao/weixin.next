using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    public enum RefundChannel
    {
        /// <summary>
        /// 原路退款 
        /// </summary>
        ORIGINAL,
        /// <summary>
        /// 退回到余额
        /// </summary>
        BALANCE,
    }

    public enum RefundSource
    {
        /// <summary>
        /// 未结算资金退款（默认使用未结算资金退款）
        /// </summary>
        REFUND_SOURCE_UNSETTLED_FUNDS,
        /// <summary>
        /// 可用余额退款
        /// </summary>
        REFUND_SOURCE_RECHARGE_FUNDS
    }

    /// <summary>
    /// 申请退款
    /// </summary>
    public class Refund
    {
        public static Task<Result> Invoke(Parameters parameters, Requester requester)
        {
            return requester.SendRequest<Result>("https://api.mch.weixin.qq.com/secapi/pay/refund", true, parameters, true);
        }

        public class Parameters : RequestData
        {
            /// <summary>
            /// 可选, 终端设备号(门店号或收银设备ID)，注意：PC网页或公众号内支付请传"WEB"
            /// </summary>
            public string device_info { get; set; }
            /// <summary>
            /// 微信的订单号，优先使用 
            /// </summary>
            public string transaction_id { get; set; }
            /// <summary>
            /// 商户系统内部的订单号，当没提供transaction_id时需要传这个。 
            /// </summary>
            public string out_trade_no { get; set; }
            /// <summary>
            /// 商户系统内部的退款单号，商户系统内部唯一，同一退款单号多次请求只退一笔
            /// </summary>
            public string out_refund_no { get; set; }
            /// <summary>
            /// 订单总金额，单位为分
            /// </summary>
            public int total_fee { get; set; }
            /// <summary>
            /// 退款总金额，订单总金额，单位为分
            /// </summary>
            public int refund_fee { get; set; }
            /// <summary>
            /// 可选, 货币，默认人民币：CNY
            /// </summary>
            public Currency refund_fee_type { get; set; }
            /// <summary>
            /// 操作员帐号, 默认为商户号
            /// </summary>
            public string op_user_id { get; set; }
            /// <summary>
            /// 可选, 退款资金来源, 默认使用未结算资金退款
            /// </summary>
            public RefundSource refund_account { get; set; }

            public override IEnumerable<KeyValuePair<string, string>> GetParameters()
            {
                yield return new KeyValuePair<string, string>("device_info", device_info);
                yield return new KeyValuePair<string, string>("transaction_id", transaction_id);
                yield return new KeyValuePair<string, string>("out_trade_no", out_trade_no);
                yield return new KeyValuePair<string, string>("out_refund_no", out_refund_no);
                yield return new KeyValuePair<string, string>("total_fee", total_fee.ToString("D"));
                yield return new KeyValuePair<string, string>("refund_fee", refund_fee.ToString("D"));
                yield return new KeyValuePair<string, string>("refund_fee_type", refund_fee_type.Code);
                yield return new KeyValuePair<string, string>("op_user_id", op_user_id);
                yield return new KeyValuePair<string, string>("refund_account", refund_account.ToString("G"));
            }
        }

        public class Result : ResponseData
        {
            /// <summary>
            /// 错误代码, 仅在result_code为FAIL的时候有意义
            /// </summary>
            public ErrorCode? err_code { get; set; }
            /// <summary>
            /// 错误代码描述, 仅在result_code为FAIL的时候有意义
            /// </summary>
            public string err_code_des { get; set; }

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
            /// 商户退款单号，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public string out_refund_no { get; set; }
            /// <summary>
            /// 微信退款单号，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public string refund_id { get; set; }
            /// <summary>
            /// 退款渠道，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public RefundChannel refund_channel { get; set; }
            /// <summary>
            /// 申请退款金额,单位为分,可以做部分退款
            /// </summary>
            public int? refund_fee { get; set; }
            /// <summary>
            /// 退款金额, 去掉非充值代金券退款金额后的退款金额，退款金额=申请退款金额-非充值代金券退款金额，退款金额&lt;=申请退款金额
            /// </summary>
            public int? settlement_refund_fee { get; set; }
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
            /// 现金退款金额，单位为分
            /// </summary>
            public int? cash_refund_fee { get; set; }
            /// <summary>
            /// 退款的代金券，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public CouponRefund[] coupons { get; set; }

            protected override void DeserializeFields(List<KeyValuePair<string, string>> values)
            {
                appid = GetValue(values, "appid");
                mch_id = GetValue(values, "mch_id");

                if (result_code == result_success)
                {
                    device_info = GetValue(values, "device_info");
                    out_refund_no = GetValue(values, "out_refund_no");
                    refund_id = GetValue(values, "refund_id");
                    refund_channel = (RefundChannel)Enum.Parse(typeof(RefundChannel), GetValue(values, "refund_channel"));
                    total_fee = GetIntValue(values, "total_fee");
                    settlement_total_fee = GetIntValue(values, "settlement_total_fee");
                    fee_type = Currency.Find(GetValue(values, "fee_type"));
                    cash_fee = GetIntValue(values, "cash_fee");
                    cash_refund_fee = GetIntValue(values, "cash_refund_fee");
                    refund_fee = GetIntValue(values, "refund_fee");
                    settlement_refund_fee = GetIntValue(values, "settlement_refund_fee");
                    transaction_id = GetValue(values, "transaction_id");
                    out_trade_no = GetValue(values, "out_trade_no");

                    var refunds = new List<CouponRefund>();
                    for (var n = 0; ; n++)
                    {
                        var sn = "_$" + n.ToString("D");
                        var type = GetValue(values, "coupon_type_" + sn);
                        if (type == null)
                            break;

                        var fefund = new CouponRefund
                        {
                            coupon_type = (CouponType)Enum.Parse(typeof(CouponType), type),
                            coupon_refund_fee = GetIntValue(values, "coupon_refund_fee" + sn) ?? 0,
                            coupon_refund_count = GetIntValue(values, "coupon_refund_count" + sn) ?? 0,
                        };
                        fefund.coupon_refund_batches = new CouponRefundBatch[fefund.coupon_refund_count];
                        refunds.Add(fefund);
                        for (var m = 0; ; m++)
                        {
                            var snm = sn + "_" + m.ToString("D");
                            fefund.coupon_refund_batches[m] = new CouponRefundBatch
                            {
                                coupon_refund_batch_id = GetValue(values, "coupon_refund_batch_id" + snm),
                                coupon_refund_id = GetValue(values, "coupon_refund_id" + snm),
                                coupon_refund_fee = GetIntValue(values, "coupon_refund_fee" + snm) ?? 0,
                            };
                        }
                    }
                    coupons = refunds.ToArray();
                }
                else
                {
                    err_code = (ErrorCode)Enum.Parse(typeof(ErrorCode), GetValue(values, "err_code"));
                    err_code_des = GetValue(values, "err_code_des");
                }
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 接口返回错误
            /// 系统超时
            /// 请用相同参数再次调用API
            /// </summary>
            SYSTEMERROR,
            /// <summary>
            /// 退款请求失败
            /// 用户帐号注销
            /// 此状态代表退款申请失败，商户可自行处理退款。
            /// </summary>
            USER_ACCOUNT_ABNORMAL,
            /// <summary>
            /// 余额不足
            /// 商户可用退款余额不足
            /// 此状态代表退款申请失败，商户可根据具体的错误提示做相应的处理。
            /// </summary>
            NOTENOUGH,
            /// <summary>
            /// 无效transaction_id
            /// 请求参数未按指引进行填写
            /// 请求参数错误，检查原交易号是否存在或发起支付交易接口返回失败
            /// </summary>
            INVALID_TRANSACTIONID,
            /// <summary>
            /// 参数错误
            /// 请求参数未按指引进行填写
            /// 请求参数错误，请重新检查再调用退款申请
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


        public class CouponRefund
        {
            /// <summary>
            /// 代金券类型
            /// </summary>
            public CouponType coupon_type { get; set; }

            /// <summary>
            /// 代金券退款金额
            /// </summary>
            public int coupon_refund_fee { get; set; }
            /// <summary>
            /// 代金券退款金额&lt;=退款金额，退款金额-代金券或立减优惠退款金额为现金
            /// </summary>
            public int coupon_refund_count { get; set; }
            /// <summary>
            /// 此类代金券退款明细
            /// </summary>
            public CouponRefundBatch[] coupon_refund_batches { get; set; }
        }

        public class CouponRefundBatch
        {
            /// <summary>
            /// 退款代金券批次ID
            /// </summary>
            public string coupon_refund_batch_id { get; set; }
            /// <summary>
            /// 退款代金券ID
            /// </summary>
            public string coupon_refund_id { get; set; }
            /// <summary>
            /// 单个退款代金券支付金额
            /// </summary>
            public int coupon_refund_fee { get; set; }
        }
    }
}
