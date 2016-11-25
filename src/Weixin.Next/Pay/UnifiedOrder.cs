using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 统一下单
    /// </summary>
    public class UnifiedOrder : PayApi<UnifiedOrder.Outcoming, UnifiedOrder.Incoming, UnifiedOrder.ErrorCode>
    {
        public UnifiedOrder(Requester requester, bool checkSignature, bool generateReport)
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
            interface_url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            requiresCert = false;
        }

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
            /// 可选, 订单生成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010 注意：最短失效时间间隔必须大于5分钟
            /// </summary>
            public DateTime time_start { get; set; }
            /// <summary>
            /// 可选, 订单失效时间，格式为yyyyMMddHHmmss，如2009年12月27日9点10分10秒表示为20091227091010
            /// </summary>
            public DateTime time_expire { get; set; }
            /// <summary>
            /// 可选, 商品标记，代金券或立减优惠功能的参数
            /// </summary>
            public string goods_tag { get; set; }
            /// <summary>
            /// 接收微信支付异步通知回调地址，通知url必须为直接可访问的url，不能携带参数
            /// </summary>
            public string notify_url { get; set; }
            /// <summary>
            /// 交易类型
            /// </summary>
            public TradeType trade_type { get; set; }
            /// <summary>
            /// trade_type=NATIVE 时，此参数必传。此id为二维码中包含的商品ID，商户自行定义。
            /// </summary>
            public string product_id { get; set; }
            /// <summary>
            /// 可选, 支付限制
            /// </summary>
            public PayLimitation? limit_pay { get; set; }
            /// <summary>
            /// trade_type=JSAPI 时，此参数必传，用户在商户appid下的唯一标识
            /// </summary>
            public string openid { get; set; }

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
                yield return new KeyValuePair<string, string>("time_start", time_start.ToString("yyyy-MM-dd"));
                yield return new KeyValuePair<string, string>("time_expire", time_expire.ToString("yyyy-MM-dd"));
                yield return new KeyValuePair<string, string>("goods_tag", goods_tag);
                yield return new KeyValuePair<string, string>("notify_url", notify_url);
                yield return new KeyValuePair<string, string>("trade_type", trade_type.ToString("g"));
                yield return new KeyValuePair<string, string>("product_id", product_id);
                yield return new KeyValuePair<string, string>("limit_pay", limit_pay?.ToString("g"));
                yield return new KeyValuePair<string, string>("openid", openid);
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
            /// 调用接口提交的终端设备号, 仅在return_code为SUCCESS的时候有意义
            /// </summary>
            public string device_info { get; set; }

            /// <summary>
            /// 调用接口提交的交易类型，仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public TradeType trade_type { get; set; }
            /// <summary>
            /// 微信生成的预支付回话标识，用于后续接口调用中使用，该值有效期为2小时，最长64字符, 仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public string prepay_id { get; set; }
            /// <summary>
            /// trade_type=NATIVE 时才有意义，可将该参数值生成二维码展示出来进行扫码支付
            /// </summary>
            public string code_url { get; set; }

            protected override void DeserializeFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                appid = GetValue(values, "appid");
                mch_id = GetValue(values, "mch_id");
                device_info = GetValue(values, "device_info");
            }

            protected override void DeserializeSuccessFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                trade_type = (TradeType)Enum.Parse(typeof(TradeType), GetValue(values, "trade_type"));
                prepay_id = GetValue(values, "prepay_id");
                code_url = GetValue(values, "code_url");
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 商户无此接口权限
            /// 商户未开通此接口权限
            /// 请商户前往申请此接口权限
            /// </summary>
            NOAUTH,
            /// <summary>
            /// 余额不足
            /// 用户帐号余额不足
            /// 用户帐号余额不足，请用户充值或更换支付卡后再支付
            /// </summary>
            NOTENOUGH,
            /// <summary>
            /// 商户订单已支付
            /// 商户订单已支付，无需重复操作
            /// 商户订单已支付，无需更多操作
            /// </summary>
            ORDERPAID,
            /// <summary>
            /// 订单已关闭
            /// 当前订单已关闭，无法支付
            /// 当前订单已关闭，请重新下单
            /// </summary>
            ORDERCLOSED,
            /// <summary>
            /// 系统错误
            /// 系统超时
            /// 系统异常，请用相同参数重新调用
            /// </summary>
            SYSTEMERROR,
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
            /// 缺少参数
            /// 缺少必要的请求参数
            /// 请检查参数是否齐全
            /// </summary>
            LACK_PARAMS,
            /// <summary>
            /// 商户订单号重复
            /// 同一笔交易不能多次提交
            /// 请核实商户订单号是否重复提交
            /// </summary>
            OUT_TRADE_NO_USED,
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
            /// <summary>
            /// 请使用post方法
            /// 未使用post传递参数 
            /// 请检查请求参数是否通过post方法提交
            /// </summary>
            REQUIRE_POST_METHOD,
            /// <summary>
            /// post数据为空
            /// post数据不能为空
            /// 请检查post数据是否为空
            /// </summary>
            POST_DATA_EMPTY,
            /// <summary>
            /// 编码格式错误
            /// 未使用指定编码格式
            /// 请使用UTF-8编码格式
            /// </summary>
            NOT_UTF8,
        }
    }
}
