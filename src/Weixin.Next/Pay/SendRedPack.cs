using System.Collections.Generic;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 发放普通红包
    /// </summary>
    public class SendRedPack : PayApi<SendRedPack.Outcoming, SendRedPack.Incoming, SendRedPack.ErrorCode>
    {
        public SendRedPack(Requester requester, bool checkSignature, bool generateReport)
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
            interface_url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";
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
            /// 红包祝福语
            /// </summary>
            public string wishing { get; set; }
            /// <summary>
            /// 调用接口的机器Ip地址
            /// </summary>
            public string client_ip { get; set; }
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
                yield return new KeyValuePair<string, string>("wishing", wishing);
                yield return new KeyValuePair<string, string>("client_ip", client_ip);
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
            /// 用户账号异常，被拦截 
            /// 请提醒用户检查自身帐号是否异常。使用常用的活跃的微信号可避免这种情况。
            /// </summary>
            NO_AUTH,
            /// <summary>
            /// 该用户今日领取红包个数超过限制 
            /// 该用户今日领取红包个数超过你在微信支付商户平台配置的上限 
            /// 如有需要、请在微信支付商户平台【api安全】中重新配置 【每日同一用户领取本商户红包不允许超过的个数】。
            /// </summary>
            SENDNUM_LIMIT,
            /// <summary>
            /// 非法appid，请确认是否为公众号的appid，不能为APP的appid 
            /// 错误传入了app的appid 
            /// 接口传入的所有appid应该为公众号的appid（在mp.weixin.qq.com申请的），不能为APP的appid（在open.weixin.qq.com申请的）。
            /// </summary>
            ILLEGAL_APPID,
            /// <summary>
            /// 红包金额发放限制 
            /// 发送红包金额不再限制范围内 
            /// 每个红包金额必须大于1元，小于200元（可联系微信支付wxhongbao@tencent.com申请调高额度）
            /// </summary>
            MONEY_LIMIT,
            /// <summary>
            /// 红包发放失败,请更换单号再重试 
            /// 该红包已经发放失败 
            /// 如果需要重新发放，请更换单号再发放
            /// </summary>
            SEND_FAILED,
            /// <summary>
            /// openid和原始单参数不一致 
            /// 更换了openid，但商户单号未更新 
            /// 请商户检查代码实现逻辑
            /// </summary>
            FATAL_ERROR,
            /// <summary>
            /// 更换了金额，但商户单号未更新 
            /// 请商户检查代码实现逻辑 
            /// 请检查金额、商户订单号是否正确
            /// </summary>
            金额和原始单参数不一致,
            /// <summary>
            /// CA证书出错，请登录微信支付商户平台下载证书 
            /// 请求携带的证书出错 
            /// 到商户平台下载证书，请求带上证书后重试
            /// </summary>
            CA_ERROR,
            /// <summary>
            /// 签名错误
            /// </summary>
            SIGN_ERROR,
            /// <summary>
            /// 请求已受理，请稍后使用原单号查询发放结果 
            /// 系统无返回明确发放结果 
            /// 使用原单号调用接口，查询发放结果，如果使用新单号调用接口，视为新发放请求
            /// </summary>
            SYSTEMERROR,
            /// <summary>
            /// 输入xml参数格式错误 
            /// 请求的xml格式错误，或者post的数据为空 
            /// 检查请求串，确认无误后重试
            /// </summary>
            XML_ERROR,
            /// <summary>
            /// 超过频率限制,请稍后再试 
            /// 受频率限制 
            /// 请对请求做频率控制（可联系微信支付wxhongbao@tencent.com申请调高）
            /// </summary>
            FREQ_LIMIT,
            /// <summary>
            /// 帐号余额不足，请到商户平台充值后再重试 
            /// 账户余额不足 
            /// 充值后重试
            /// </summary>
            NOTENOUGH,
            /// <summary>
            /// openid和appid不匹配 
            /// openid和appid不匹配 
            /// 发红包的openid必须是本appid下的openid
            /// </summary>
            OPENID_ERROR,
            /// <summary>
            /// 触达消息给用户appid有误 
            /// msgappid与主、子商户号的绑定关系校验失败 
            /// 检查下msgappid是否填写错误，msgappid需要跟主、子商户号 有绑定关系
            /// </summary>
            MSGAPPID_ERROR,
            /// <summary>
            /// 主、子商户号关系校验失败 
            /// 服务商模式下主商户号与子商户号关系校验失败 
            /// 确认传入的主商户号与子商户号是否有受理关系
            /// </summary>
            ACCEPTMODE_ERROR,
            /// <summary>
            /// 请求已受理，请稍后使用原单号查询发放结果 
            /// 发红包流程正在处理 
            /// 二十分钟后查询,按照查询结果成功失败进行处理
            /// </summary>
            PROCESSING,
            /// <summary>
            /// 参数错误
            /// </summary>
            PARAM_ERROR
        }

    }
}
