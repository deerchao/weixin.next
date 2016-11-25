using System.Collections.Generic;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 发放裂变红包
    /// </summary>
    public class SendGroupRedPack : PayApi<SendGroupRedPack.Outcoming, SendGroupRedPack.Incoming, SendGroupRedPack.ErrorCode>
    {
        public SendGroupRedPack(Requester requester, bool checkSignature, bool generateReport)
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
            interface_url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendgroupredpack";
            requiresCert = true;
        }

        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 商户订单号（每个订单号必须唯一）组成：mch_id+yyyymmdd+10位一天内不能重复的数字
            /// </summary>
            public string mch_billno { get; set; }
            /// <summary>
            /// 红包发送者名称
            /// </summary>
            public string send_name { get; set; }
            /// <summary>
            /// 用户openid
            /// </summary>
            public string re_openid { get; set; }
            /// <summary>
            /// 付款金额，单位分
            /// </summary>
            public int total_amount { get; set; }
            /// <summary>
            /// 红包发放总人数, 目前只支持 1
            /// </summary>
            public int total_num { get; set; }
            /// <summary>
            /// 红包金额设置方式
            /// </summary>
            public AmtType amt_type { get; set; }
            /// <summary>
            /// 红包祝福语
            /// </summary>
            public string wishing { get; set; }
            /// <summary>
            /// 活动名称
            /// </summary>
            public string act_name { get; set; }
            /// <summary>
            /// 备注信息
            /// </summary>
            public string remark { get; set; }
            /// <summary>
            /// 发放红包使用场景，红包金额大于200时必传
            /// </summary>
            public RedPackSceneId? scene_id { get; set; }
            /// <summary>
            /// 活动信息
            /// </summary>
            public RedPackRiskInfo risk_info { get; set; }
            /// <summary>
            /// 资金授权商户号, 服务商替特约商户发放时使用
            /// </summary>
            public string consume_mch_id { get; set; }

            public override string AppIdFieldName
            {
                get { return "wxappid"; }
            }

            public override IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser)
            {
                yield return new KeyValuePair<string, string>("mch_billno", mch_billno);
                yield return new KeyValuePair<string, string>("send_name", send_name);
                yield return new KeyValuePair<string, string>("re_openid", re_openid);
                yield return new KeyValuePair<string, string>("total_amount", $"{total_amount:D}");
                yield return new KeyValuePair<string, string>("total_num", $"{total_num:D}");
                yield return new KeyValuePair<string, string>("amt_type", amt_type.ToString("G"));
                yield return new KeyValuePair<string, string>("wishing", wishing);
                yield return new KeyValuePair<string, string>("act_name", act_name);
                yield return new KeyValuePair<string, string>("remark", remark);
                yield return new KeyValuePair<string, string>("scene_id", $"{scene_id:G}");
                yield return new KeyValuePair<string, string>("risk_info", $"{risk_info}");
                yield return new KeyValuePair<string, string>("consume_mch_id", consume_mch_id);
            }
        }

        public class Incoming : IncomingData<ErrorCode>
        {
            /// <summary>
            /// 商户订单号（每个订单号必须唯一）组成：mch_id+yyyymmdd+10位一天内不能重复的数字
            /// </summary>
            public string mch_billno { get; set; }
            /// <summary>
            /// 微信支付分配的商户号
            /// </summary>
            public string mch_id { get; set; }
            /// <summary>
            /// 公众账号appid
            /// </summary>
            public string wxappid { get; set; }
            /// <summary>
            /// 用户openid
            /// </summary>
            public string re_openid { get; set; }
            /// <summary>
            /// 付款金额，单位分
            /// </summary>
            public int? total_amount { get; set; }
            /// <summary>
            /// 红包订单的微信单号
            /// </summary>
            public string send_listid { get; set; }

            protected override void DeserializeSuccessFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                mch_billno = GetValue(values, "mch_billno");
                mch_id = GetValue(values, "mch_id");
                wxappid = GetValue(values, "wxappid");
                re_openid = GetValue(values, "re_openid");
                total_amount = GetIntValue(values, "total_amount");
                send_listid = GetValue(values, "send_listid");
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 发放失败，此请求可能存在风险，已被微信拦截 
            /// 请提醒用户检查自身帐号是否异常。使用常用的活跃的微信号可避免这种情况。
            /// </summary>
            NO_AUTH,
            /// <summary>
            /// 该用户今日领取红包个数超过限制 
            /// 如有需要、请在微信支付商户平台【api安全】中重新配置 【每日同一用户领取本商户红包不允许超过的个数】。
            /// </summary>
            SENDNUM_LIMIT,
            /// <summary>
            /// 请求未携带证书，或请求携带的证书出错 
            /// 到商户平台下载证书，请求带上证书后重试。
            /// </summary>
            CA_ERROR,
            /// <summary>
            /// 错误传入了app的appid 
            /// 接口传入的所有appid应该为公众号的appid（在mp.weixin.qq.com申请的），不能为APP的appid（在open.weixin.qq.com申请的）。
            /// </summary>
            ILLEGAL_APPID,
            /// <summary>
            /// 商户签名错误 
            /// 按文档要求重新生成签名后再重试。
            /// </summary>
            SIGN_ERROR,
            /// <summary>
            /// 受频率限制 
            /// 请对请求做频率控制
            /// </summary>
            FREQ_LIMIT,
            /// <summary>
            /// 请求的xml格式错误，或者post的数据为空 
            /// 检查请求串，确认无误后重试
            /// </summary>
            XML_ERROR,
            /// <summary>
            /// 参数错误 
            /// 请查看err_code_des，修改设置错误的参数
            /// </summary>
            PARAM_ERROR,
            /// <summary>
            /// Openid错误 
            /// 根据用户在商家公众账号上的openid，获取用户在红包公众账号上的openid 错误。请核对商户自身公众号appid和用户在此公众号下的openid。
            /// </summary>
            OPENID_ERROR,
            /// <summary>
            /// 余额不足 
            /// 商户账号余额不足，请登录微信支付商户平台充值
            /// </summary>
            NOTENOUGH,
            /// <summary>
            /// 重复请求时，参数与原单不一致 
            /// 使用相同商户单号进行重复请求时，参数与第一次请求时不一致，请检查并修改参数后再重试。
            /// </summary>
            FATAL_ERROR,
            /// <summary>
            /// 企业红包的按分钟发放受限 
            /// 每分钟发送红包数量不得超过1800个；（可联系微信支付wxhongbao@tencent.com调高额度）
            /// </summary>
            SECOND_OVER_LIMITED,
            /// <summary>
            /// 企业红包的按天日发放受限 
            /// 单个商户日发送红包数量不大于10000个；（可联系微信支付wxhongbao@tencent.com调高额度）
            /// </summary>
            DAY_OVER_LIMITED,
            /// <summary>
            /// 红包金额发放限制 
            /// 每个红包金额必须大于1元，小于1000元（可联系微信支付wxhongbao@tencent.com调高额度至4999元）
            /// </summary>
            MONEY_LIMIT,
            /// <summary>
            /// 红包发放失败,请更换单号再重试 
            /// 原商户单号已经失败，如果还要对同一个用户发放红包， 需要更换新的商户单号再试。
            /// </summary>
            SEND_FAILED,
            /// <summary>
            /// 系统繁忙，请再试。 
            /// 可用同一商户单号再次调用，只会发放一个红包
            /// </summary>
            SYSTEMERROR,
            /// <summary>
            /// 请求已受理，请稍后使用原单号查询发放结果 
            /// 二十分钟后查询,按照查询结果成功失败进行处理 
            /// </summary>
            PROCESSING,
        }

        public enum AmtType
        {
            /// <summary>
            /// 全部随机,商户指定总金额和红包发放总人数，由微信支付随机计算出各红包金额
            /// </summary>
            ALL_RAND,
        }
    }
}
