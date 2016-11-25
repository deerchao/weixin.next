using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 企业付款
    /// </summary>
    public class Transfer : PayApi<Transfer.Outcoming, Transfer.Incoming, Transfer.ErrorCode>
    {
        public Transfer(Requester requester, bool checkSignature, bool generateReport)
            : base(requester, checkSignature, generateReport)
        {
        }

        protected override string GetReportOutTradeNo(Outcoming outcoming, Incoming incoming)
        {
            return null;
        }

        protected override string GetReportDeviceNo(Outcoming outcoming)
        {
            return outcoming.device_info;
        }

        protected override void GetApiUrl(Outcoming outcoming, out string interface_url, out bool requiresCert)
        {
            interface_url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";
            requiresCert = true;
        }

        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 可选, 终端设备号(门店号或收银设备ID)，注意：PC网页或公众号内支付请传"WEB"
            /// </summary>
            public string device_info { get; set; }
            /// <summary>
            /// 商户订单号
            /// </summary>
            public string partner_trade_no { get; set; }
            /// <summary>
            /// 收款用户标志
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 校验用户姓名选项
            /// </summary>
            public NameCheckOption check_name { get; set; }
            /// <summary>
            /// 收款用户姓名, 不检验姓名时不填
            /// </summary>
            public string re_user_name { get; set; }
            /// <summary>
            /// 企业付款金额，单位为分
            /// </summary>
            public int amount { get; set; }
            /// <summary>
            /// 企业付款操作说明信息。必填。
            /// </summary>
            public string desc { get; set; }
            /// <summary>
            /// 调用接口的机器Ip地址
            /// </summary>
            public string spbill_create_ip { get; set; }

            public override string AppIdFieldName
            {
                get { return "mch_appid"; }
            }

            public override string MerchantIdFieldName
            {
                get { return "mchid"; }
            }

            public override IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser)
            {
                yield return new KeyValuePair<string, string>("device_info", device_info);
                yield return new KeyValuePair<string, string>("partner_trade_no", partner_trade_no);
                yield return new KeyValuePair<string, string>("openid", openid);
                yield return new KeyValuePair<string, string>("check_name", check_name.ToString("G"));
                yield return new KeyValuePair<string, string>("re_user_name", re_user_name);
                yield return new KeyValuePair<string, string>("amount", amount.ToString("D"));
                yield return new KeyValuePair<string, string>("desc", desc);
                yield return new KeyValuePair<string, string>("spbill_create_ip", spbill_create_ip);
            }
        }

        public class Incoming : IncomingData<ErrorCode>
        {
            /// <summary>
            /// 调用接口提交的公众账号ID, 仅在return_code为SUCCESS的时候有意义
            /// </summary>
            public string mch_appid { get; set; }
            /// <summary>
            /// 调用接口提交的商户号, 仅在return_code为SUCCESS的时候有意义
            /// </summary>
            public string mchid { get; set; }
            /// <summary>
            /// 调用接口提交的终端设备号, 仅在return_code为SUCCESS的时候有意义
            /// </summary>
            public string device_info { get; set; }

            /// <summary>
            /// 商户订单号，仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public string partner_trade_no { get; set; }
            /// <summary>
            /// 微信订单号，仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public string payment_no { get; set; }
            /// <summary>
            /// 企业付款成功时间，仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public DateTime payment_time { get; set; }

            protected override void DeserializeFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                mch_appid = GetValue(values, "mch_appid");
                mchid = GetValue(values, "mchid");
                device_info = GetValue(values, "device_info");
            }

            protected override void DeserializeSuccessFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                partner_trade_no = GetValue(values, "partner_trade_no");
                payment_no = GetValue(values, "payment_no");
                payment_time = DateTime.ParseExact(GetValue(values, "payment_time"), "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 没有权限 
            /// 没有授权请求此api 
            /// 请联系微信支付开通api权限
            /// </summary>
            NOAUTH,

            /// <summary>
            /// 付款金额不能小于最低限额 
            /// 付款金额不能小于最低限额 
            /// 每次付款金额必须大于1元
            /// </summary>
            AMOUNT_LIMIT,

            /// <summary>
            /// 参数错误 
            /// 参数缺失，或参数格式出错，参数不合法等 
            /// 请查看err_code_des，修改设置错误的参数
            /// </summary>

            PARAM_ERROR,
            /// <summary>
            /// Openid错误 
            /// Openid格式错误或者不属于商家公众账号 
            /// 请核对商户自身公众号appid和用户在此公众号下的openid。
            /// </summary>

            OPENID_ERROR,
            /// <summary>
            /// 余额不足 
            /// 帐号余额不足 
            /// 请用户充值或更换支付卡后再支付
            /// </summary>

            NOTENOUGH,
            /// <summary>
            /// 系统繁忙，请稍后再试。 
            /// 系统错误，请重试 
            /// 使用原单号以及原请求参数重试
            /// </summary>

            SYSTEMERROR,
            /// <summary>
            /// 姓名校验出错 
            /// 请求参数里填写了需要检验姓名，但是输入了错误的姓名 
            /// 填写正确的用户姓名
            /// </summary>

            NAME_MISMATCH,
            /// <summary>
            /// 签名错误 
            /// 没有按照文档要求进行签名 
            ///  签名前没有按照要求进行排序。没有使用商户平台设置的密钥进行签名; 参数有空格或者进行了encode后进行签名。
            /// </summary>

            SIGN_ERROR,
            /// <summary>
            /// Post内容出错 
            /// Post请求数据不是合法的xml格式内容 
            /// 修改post的内容
            /// </summary>

            XML_ERROR,
            /// <summary>
            /// 两次请求参数不一致 
            /// 两次请求商户单号一样，但是参数不一致 
            /// 如果想重试前一次的请求，请用原参数重试，如果重新发送，请更换单号。
            /// </summary>

            FATAL_ERROR,
            /// <summary>
            /// 证书出错 
            /// 请求没带证书或者带上了错误的证书 
            /// 到商户平台下载证书; 请求的时候带上该证书
            /// </summary>
            CA_ERROR,
        }

        public enum NameCheckOption
        {
            /// <summary>
            /// 不校验真实姓名
            /// </summary>
            NO_CHECK,
            /// <summary>
            /// 强校验真实姓名（未实名认证的用户会校验失败，无法转账）
            /// </summary>
            FORCE_CHECK,
            /// <summary>
            /// 针对已实名认证的用户才校验真实姓名（未实名认证用户不校验，可以转账成功）
            /// </summary>
            OPTION_CHECK
        }
    }
}
