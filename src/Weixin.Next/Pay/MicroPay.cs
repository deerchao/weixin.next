using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 提交刷卡支付
    /// </summary>
    public class MicroPay : PayApi<MicroPay.Outcoming, MicroPay.Incoming, MicroPay.ErrorCode>
    {
        public MicroPay(Requester requester, bool checkSignature, bool generateReport)
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
            interface_url = "https://api.mch.weixin.qq.com/pay/micropay";
            requiresCert = false;
        }

        /// <summary>
        /// 提交刷卡支付
        /// </summary>
        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 可选, 终端设备号(门店号或收银设备ID)，注意：PC网页或公众号内支付请传"WEB"
            /// </summary>
            public string device_info { get; set; }
            /// <summary>
            /// 商品简单描述，该字段须严格按照规范传递, 具体请见 https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=4_2
            /// </summary>
            public string body { get; set; }
            /// <summary>
            /// 可选, 商品详情
            /// </summary>
            public GoodDetails detail { get; set; }
            /// <summary>
            /// 可选, 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据
            /// </summary>
            public string attach { get; set; }
            /// <summary>
            /// 商户系统内部的订单号,32个字符内、可包含字母, 需要唯一
            /// </summary>
            public string out_trade_no { get; set; }
            /// <summary>
            /// 可选, 货币，默认人民币：CNY
            /// </summary>
            public Currency fee_type { get; set; }
            /// <summary>
            /// 订单总金额，单位为分
            /// </summary>
            public int total_fee { get; set; }
            /// <summary>
            /// APP和网页支付提交用户端ip，Native支付填调用微信支付API的机器IP
            /// </summary>
            public string spbill_create_ip { get; set; }
            /// <summary>
            /// 可选, 商品标记，代金券或立减优惠功能的参数
            /// </summary>
            public string goods_tag { get; set; }
            /// <summary>
            /// 扫码支付授权码，设备读取用户微信中的条码或者二维码信息
            /// </summary>
            public string auth_code { get; set; }

            public override IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser)
            {
                yield return new KeyValuePair<string, string>("device_info", device_info);
                yield return new KeyValuePair<string, string>("body", body);
                yield return new KeyValuePair<string, string>("detail", detail?.goods_detail?.Length > 0 ? jsonParser.ToString(detail) : null);
                yield return new KeyValuePair<string, string>("attach", attach);
                yield return new KeyValuePair<string, string>("out_trade_no", out_trade_no);
                yield return new KeyValuePair<string, string>("fee_type", fee_type.Code);
                yield return new KeyValuePair<string, string>("total_fee", total_fee.ToString("D"));
                yield return new KeyValuePair<string, string>("spbill_create_ip", spbill_create_ip);
                yield return new KeyValuePair<string, string>("goods_tag", goods_tag);
                yield return new KeyValuePair<string, string>("auth_code", auth_code);
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
            /// 微信支付分配的终端设备号，仅在return_code为SUCCESS的时候有意义 
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
            /// 商品详情, 与提交数据一致, 仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public GoodDetails detail { get; set; }
            /// <summary>
            /// 付款银行，采用字符串类型的银行标识, 仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public Bank bank_type { get; set; }
            /// <summary>
            /// 货币类型，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public Currency fee_type { get; set; }
            /// <summary>
            /// 订单总金额，单位为分, 仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public int? total_fee { get; set; }
            /// <summary>
            /// 应结订单金额=订单金额-非充值代金券金额，应结订单金额&lt;=订单金额，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public int? settlement_total_fee { get; set; }
            /// <summary>
            /// “代金券”金额&lt;=订单金额，订单金额-“代金券”金额=现金支付金额，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public int? coupon_fee { get; set; }
            /// <summary>
            /// 现金支付金额订单现金支付金额，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public int cash_fee { get; set; }
            /// <summary>
            /// 现金支付货币类型，仅在公众账号类型支付有效，仅在return_code 和result_code都为SUCCESS的时候有意义
            /// </summary>
            public Currency cash_fee_type { get; set; }
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
                bank_type = Bank.Find(GetValue(values, "bank_type"));
                fee_type = Currency.Find(GetValue(values, "fee_type"));
                total_fee = GetIntValue(values, "total_fee") ?? 0;
                settlement_total_fee = GetIntValue(values, "settlement_total_fee");
                coupon_fee = GetIntValue(values, "coupon_fee");
                cash_fee = GetIntValue(values, "cash_fee") ?? 0;
                cash_fee_type = Currency.Find(GetValue(values, "cash_fee_type"));
                transaction_id = GetValue(values, "transaction_id");
                out_trade_no = GetValue(values, "out_trade_no");
                attach = GetValue(values, "attach");
                time_end = DateTime.ParseExact(GetValue(values, "time_end"), "yyyMMddHHmmss", null);
                var d = GetValue(values, "detail");
                if (d != null)
                {
                    detail = jsonParser.Build<GoodDetails>(jsonParser.Parse(d));
                }
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 接口返回错误
            /// 支付结果未知
            /// 系统超时
            /// 请立即调用被扫订单结果查询API，查询当前订单状态，并根据订单的状态决定下一步的操作。
            /// </summary>
            SYSTEMERROR,
            /// <summary>
            /// 参数错误
            /// 支付确认失败
            /// 请求参数未按指引进行填写
            /// 请根据接口返回的详细信息检查您的程序
            /// </summary>
            PARAM_ERROR,
            /// <summary>
            /// 订单已支付
            /// 支付确认失败
            /// 订单号重复
            /// 请确认该订单号是否重复支付，如果是新单，请使用新订单号提交
            /// </summary>
            ORDERPAID,
            /// <summary>
            /// 商户无权限
            /// 支付确认失败
            /// 商户没有开通被扫支付权限 
            /// 请开通商户号权限。请联系产品或商务申请
            /// </summary>
            NOAUTH,
            /// <summary>
            /// 二维码已过期，请用户在微信上刷新后再试
            /// 支付确认失败
            /// 用户的条码已经过期 
            /// 请收银员提示用户，请用户在微信上刷新条码，然后请收银员重新扫码。 直接将错误展示给收银员
            /// </summary>
            AUTHCODEEXPIRE,
            /// <summary>
            /// 余额不足 
            /// 支付确认失败
            /// 用户的零钱余额不足 
            /// 请收银员提示用户更换当前支付的卡，然后请收银员重新扫码。建议：商户系统返回给收银台的提示为“用户余额不足.提示用户换卡支付”
            /// </summary>
            NOTENOUGH,
            /// <summary>
            /// 不支持卡类型
            /// 支付确认失败
            /// 用户使用卡种不支持当前支付形式
            /// 请用户重新选择卡种 建议：商户系统返回给收银台的提示为“该卡不支持当前支付，提示用户换卡支付或绑新卡支付”
            /// </summary>
            NOTSUPORTCARD,
            /// <summary>
            /// 订单已关闭
            /// 支付确认失败
            /// 该订单已关
            /// 商户订单号异常，请重新下单支付
            /// </summary>
            ORDERCLOSED,
            /// <summary>
            /// 订单已撤销
            /// 支付确认失败
            /// 当前订单已经被撤销
            /// 当前订单状态为“订单已撤销”，请提示用户重新支付
            /// </summary>
            ORDERREVERSED,
            /// <summary>
            /// 银行系统异常
            /// 支付结果未知
            /// 银行端超时
            /// 请立即调用被扫订单结果查询API，查询当前订单的不同状态，决定下一步的操作。
            /// </summary>
            BANKERROR,
            /// <summary>
            /// 用户支付中，需要输入密码
            /// 支付结果未知
            /// 该笔交易因为业务规则要求，需要用户输入支付密码。
            /// 等待5秒，然后调用被扫订单结果查询API，查询当前订单的不同状态，决定下一步的操作。
            /// </summary>
            USERPAYING,
            /// <summary>
            /// 授权码参数错误
            /// 支付确认失败
            /// 请求参数未按指引进行填写
            /// 每个二维码仅限使用一次，请刷新再试
            /// </summary>
            AUTH_CODE_ERROR,
            /// <summary>
            /// 授权码检验错误
            /// 支付确认失败
            /// 收银员扫描的不是微信支付的条码 
            /// 请扫描微信支付被扫条码/二维码
            /// </summary>
            AUTH_CODE_INVALID,
            /// <summary>
            /// XML格式错误
            /// 支付确认失败
            /// XML格式错误
            /// 请检查XML参数格式是否正确
            /// </summary>
            XML_FORMAT_ERROR,
            /// <summary>
            /// 请使用post方法
            /// 支付确认失败
            /// 未使用post传递参数
            /// 请检查请求参数是否通过post方法提交
            /// </summary>
            REQUIRE_POST_METHOD,
            /// <summary>
            /// 签名错误
            /// 支付确认失败
            /// 参数签名结果不正确 
            /// 请检查签名参数和方法是否都符合签名算法要求
            /// </summary>
            SIGNERROR,
            /// <summary>
            /// 缺少参数
            /// 支付确认失败
            /// 缺少必要的请求参数
            /// 请检查参数是否齐全
            /// </summary>
            LACK_PARAMS,
            /// <summary>
            /// 编码格式错误
            /// 支付确认失败
            /// 未使用指定编码格式 
            /// 请使用UTF-8编码格式
            /// </summary>
            NOT_UTF8,
            /// <summary>
            /// 支付帐号错误
            /// 支付确认失败
            /// 暂不支持同一笔订单更换支付方
            /// 请确认支付方是否相同
            /// </summary>
            BUYER_MISMATCH,
            /// <summary>
            /// APPID不存在
            /// 支付确认失败
            /// 参数中缺少APPID
            /// 请检查APPID是否正确
            /// </summary>
            APPID_NOT_EXIST,
            /// <summary>
            /// MCHID不存在
            /// 支付确认失败
            /// 参数中缺少MCHID
            /// 请检查MCHID是否正确
            /// </summary>
            MCHID_NOT_EXIST,
            /// <summary>
            /// 商户订单号重复
            /// 支付确认失败
            /// 同一笔交易不能多次提交
            /// 请核实商户订单号是否重复提交
            /// </summary>
            OUT_TRADE_NO_USED,
            /// <summary>
            /// appid和mch_id不匹配
            /// 支付确认失败
            /// appid和mch_id不匹配
            /// 请确认appid和mch_id是否匹配
            /// </summary>
            APPID_MCHID_NOT_MATCH,
        }
    }
}
