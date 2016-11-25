using System.Collections.Generic;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 发放代金券
    /// </summary>
    public class SendCoupon : PayApi<SendCoupon.Outcoming, SendCoupon.Incoming, SendCoupon.ErrorCode>
    {
        public SendCoupon(Requester requester, bool checkSignature, bool generateReport)
            : base(requester, checkSignature, generateReport)
        {
        }

        protected override string GetReportOutTradeNo(Outcoming outcoming, Incoming incoming)
        {
            return null;
        }

        protected override string GetReportDeviceNo(Outcoming outcoming)
        {
            return null;
        }

        protected override void GetApiUrl(Outcoming outcoming, out string interface_url, out bool requiresCert)
        {
            interface_url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/send_coupon";
            requiresCert = true;
        }

        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 代金券批次id
            /// </summary>
            public string coupon_stock_id { get; set; }
            /// <summary>
            /// openid记录数（目前支持 1）
            /// </summary>
            public string openid_count { get; set; }
            /// <summary>
            /// 商户此次发放凭据号（格式：商户id+日期+流水号），商户侧需保持唯一性
            /// </summary>
            public string partner_trade_no { get; set; }
            /// <summary>
            /// 用户openid
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 操作员帐号, 默认为商户号, 可在商户平台配置操作员对应的api权限
            /// </summary>
            public string op_user_id { get; set; }
            /// <summary>
            /// 微信支付分配的终端设备号
            /// </summary>
            public string device_info { get; set; }

            /// <summary>
            /// 协议版本, 默认1.0
            /// </summary>
            public string version { get; set; } = "1.0";

            /// <summary>
            /// 协议类型, XML【目前仅支持默认XML】
            /// </summary>
            public string type { get; set; } = "XML";

            public override IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser)
            {
                yield return new KeyValuePair<string, string>("coupon_stock_id", coupon_stock_id);
                yield return new KeyValuePair<string, string>("openid_count", openid_count);
                yield return new KeyValuePair<string, string>("partner_trade_no", partner_trade_no);
                yield return new KeyValuePair<string, string>("openid", openid);
                yield return new KeyValuePair<string, string>("op_user_id", op_user_id);
                yield return new KeyValuePair<string, string>("device_info", device_info);
                yield return new KeyValuePair<string, string>("version", version);
                yield return new KeyValuePair<string, string>("type", type);
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
            /// 微信支付分配的终端设备号
            /// </summary>
            public string device_info { get; set; }
            /// <summary>
            /// 代金券批次id
            /// </summary>
            public string coupon_stock_id { get; set; }
            /// <summary>
            /// 返回记录数
            /// </summary>
            public int? resp_count { get; set; }
            /// <summary>
            /// 成功记录数
            /// </summary>
            public int? success_count { get; set; }
            /// <summary>
            /// 失败记录数
            /// </summary>
            public int? failed_count { get; set; }
            /// <summary>
            /// 用户标识
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 返回码
            /// </summary>
            public string ret_code { get; set; }
            /// <summary>
            /// 返回信息
            /// </summary>
            public string ret_msg { get; set; }
            /// <summary>
            /// 代金券id
            /// </summary>
            public string coupon_id { get; set; }

            protected override void DeserializeFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                appid = GetValue(values, "appid");
                mch_id = GetValue(values, "mch_id");
                device_info = GetValue(values, "device_info");
            }

            protected override void DeserializeSuccessFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                coupon_stock_id = GetValue(values, "coupon_stock_id");
                resp_count = GetIntValue(values, "resp_count");
                success_count = GetIntValue(values, "success_count");
                failed_count = GetIntValue(values, "failed_count");
                openid = GetValue(values, "openid");
                ret_code = GetValue(values, "ret_code");
                ret_msg = GetValue(values, "ret_msg");
                coupon_id = GetValue(values, "coupon_id");
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 你已领取过该代金券 
            /// 用户已领过，正常逻辑报错
            /// </summary>
            USER_AL_GET_COUPON,

            /// <summary>
            /// 网络环境不佳，请重试 
            /// 请重试
            /// </summary>
            NETWORK_ERROR,

            /// <summary>
            /// 活动已结束 
            /// 活动已结束，属于正常逻辑错误
            /// </summary>
            AL_STOCK_OVER,

            /// <summary>
            /// 超过发放频率限制，请稍后再试 
            /// 请求对发放请求做频率控制
            /// </summary>
            FREQ_OVER_LIMIT,

            /// <summary>
            /// 校验参数错误（会返回具体哪个参数错误） 
            /// 根据错误提示确认参数无误并更正
            /// </summary>
            PARAM_ERROR,

            /// <summary>
            /// 签名错误 
            /// 验证签名有误，参见3.2.1
            /// </summary>
            SIGN_ERROR,

            /// <summary>
            /// 证书有误 
            /// 确认证书正确，或者联系商户平台更新证书
            /// </summary>
            CA_ERROR,

            /// <summary>
            /// 输入参数xml格式有误 
            /// 检查入参的xml格式是否正确
            /// </summary>
            REQ_PARAM_XML_ERR,

            /// <summary>
            /// 批次ID为空 
            /// 确保批次id正确传入
            /// </summary>
            COUPON_STOCK_ID_EMPTY,

            /// <summary>
            /// 商户ID为空 
            /// 确保商户id正确传入
            /// </summary>
            MCH_ID_EMPTY,

            /// <summary>
            /// 商户id有误 
            /// 检查商户id是否正确并合法
            /// </summary>
            CODE_2_ID_ERR,

            /// <summary>
            /// 用户openid为空 
            /// 检查用户openid是否正确并合法
            /// </summary>
            OPEN_ID_EMPTY,

            /// <summary>
            /// 获取客户端证书序列号失败! 
            /// 检查证书是否正确
            /// </summary>
            ERR_VERIFY_SSL_SERIAL,

            /// <summary>
            /// 获取客户端证书特征名称(DN)域失败! 
            /// 检查证书是否正确
            /// </summary>
            ERR_VERIFY_SSL_SN,

            /// <summary>
            /// 证书验证失败 
            /// 检查证书是否正确
            /// </summary>
            CA_VERIFY_FAILED,

            /// <summary>
            /// 抱歉，该代金券已失效 
            ///  
            /// </summary>
            STOCK_IS_NOT_VALID,

        }
    }
}
