using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 查询代金券信息
    /// </summary>
    public class QueryCouponsInfo : PayApi<QueryCouponsInfo.Outcoming, QueryCouponsInfo.Incoming, QueryCouponsInfo.ErrorCode>
    {
        public QueryCouponsInfo(Requester requester, bool checkSignature, bool generateReport)
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
            interface_url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/querycouponsinfo";
            requiresCert = false;
        }

        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 代金券id
            /// </summary>
            public string coupon_id { get; set; }
            /// <summary>
            /// 用户标识
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 代金劵对应的批次号
            /// </summary>
            public string stock_id { get; set; }
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
                yield return new KeyValuePair<string, string>("coupon_id", coupon_id);
                yield return new KeyValuePair<string, string>("openid", openid);
                yield return new KeyValuePair<string, string>("stock_id", stock_id);
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
            /// 微信支付分配的子商户号，受理模式下必填
            /// </summary>
            public string sub_mch_id { get; set; }
            /// <summary>
            /// 微信支付分配的终端设备号
            /// </summary>
            public string device_info { get; set; }
            /// <summary>
            /// 代金券批次id
            /// </summary>
            public string coupon_stock_id { get; set; }
            /// <summary>
            /// 批次类型；1-批量型，2-触发型
            /// </summary>
            public int? coupon_stock_type { get; set; }
            /// <summary>
            /// 代金券id
            /// </summary>
            public string coupon_id { get; set; }
            /// <summary>
            /// 代金券面值,单位是分
            /// </summary>
            public int? coupon_value { get; set; }
            /// <summary>
            /// 代金券使用最低限额,单位是分
            /// </summary>
            public int? coupon_mininumn { get; set; }
            /// <summary>
            /// 代金券名称
            /// </summary>
            public string coupon_name { get; set; }
            /// <summary>
            /// 代金券状态：2-已激活，4-已锁定，8-已实扣
            /// </summary>
            public int? coupon_state { get; set; }
            /// <summary>
            /// 代金券类型：1-代金券无门槛，2-代金券有门槛互斥，3-代金券有门槛叠加，
            /// </summary>
            public int? coupon_type { get; set; }
            /// <summary>
            /// 代金券描述
            /// </summary>
            public string coupon_desc { get; set; }
            /// <summary>
            /// 代金券实际使用金额
            /// </summary>
            public int? coupon_use_value { get; set; }
            /// <summary>
            /// 代金券剩余金额：部分使用情况下，可能会存在券剩余金额
            /// </summary>
            public int? coupon_remain_value { get; set; }
            /// <summary>
            /// 生效开始时间
            /// </summary>
            public DateTime? begin_time { get; set; }
            /// <summary>
            /// 生效结束时间
            /// </summary>
            public DateTime? end_time { get; set; }
            /// <summary>
            /// 发放时间
            /// </summary>
            public DateTime? send_time { get; set; }
            /// <summary>
            /// 使用时间
            /// </summary>
            public DateTime? use_time { get; set; }
            /// <summary>
            /// 代金券使用后，关联的大单收单单号
            /// </summary>
            public string trade_no { get; set; }
            /// <summary>
            /// 代金券使用后，消耗方商户名称
            /// </summary>
            public string consumer_mch_id { get; set; }
            /// <summary>
            /// 代金券使用后，消耗方商户appid
            /// </summary>
            public string consumer_mch_appid { get; set; }
            /// <summary>
            /// 发放来源
            /// </summary>
            public SendSource? send_source { get; set; }
            /// <summary>
            /// 该代金券是否允许部分使用标识：1-表示支持部分使用
            /// </summary>
            public string is_partial_use { get; set; }

            protected override void DeserializeFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                appid = GetValue(values, "appid");
                mch_id = GetValue(values, "mch_id");
                device_info = GetValue(values, "device_info");
            }

            protected override void DeserializeSuccessFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                sub_mch_id = GetValue(values, "sub_mch_id");
                coupon_stock_id = GetValue(values, "coupon_stock_id");
                coupon_stock_type = GetIntValue(values, "coupon_stock_type");
                coupon_id = GetValue(values, "coupon_id");
                coupon_name = GetValue(values, "coupon_name");
                coupon_value = GetIntValue(values, "coupon_value");
                coupon_state = GetIntValue(values, "coupon_state");
                coupon_mininumn = GetIntValue(values, "coupon_mininumn");
                coupon_type = GetIntValue(values, "coupon_type");
                coupon_desc = GetValue(values, "coupon_desc");
                coupon_use_value = GetIntValue(values, "coupon_use_value");
                coupon_remain_value = GetIntValue(values, "coupon_remain_value");
                trade_no = GetValue(values, "trade_no");
                consumer_mch_id = GetValue(values, "consumer_mch_id");
                consumer_mch_appid = GetValue(values, "consumer_mch_appid");
                is_partial_use = GetValue(values, "is_partial_use");
                send_source =(SendSource)Enum.Parse(typeof(SendSource), GetValue(values, "send_source"));

                begin_time = DateTime.ParseExact(GetValue(values, "begin_time"), "yyyyMMddHHmmss", null);
                end_time = DateTime.ParseExact(GetValue(values, "end_time"), "yyyyMMddHHmmss", null);
                send_time = DateTime.ParseExact(GetValue(values, "send_time"), "yyyyMMddHHmmss", null);
                if (GetValue(values, "use_time") != null)
                    use_time = DateTime.ParseExact(GetValue(values, "use_time"), "yyyyMMddHHmmss", null);
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 券没有查找成功 
            /// 确认券id、用户openid的正确性
            /// </summary>
            COUPON_NOT_FOUND,
            /// <summary>
            /// 签名错误 
            /// 验证签名有误，参见3.2.1
            /// </summary>
            SIGN_ERROR,
            /// <summary>
            /// 批次id不正确 
            /// 确认批次id正确性以及和商户id的所属关系是否正确
            /// </summary>
            COUPON_STOCK_ID_NOT_VALID,
            /// <summary>
            /// 输入的参数xml格式有误 
            /// 检查输入的xml格式是否正确
            /// </summary>
            REQ_PARAM_XML_ERR,
            /// <summary>
            /// 批次id为空 
            /// 确认批次id正确传入
            /// </summary>
            COUPON_STOCK_ID_EMPTY,
            /// <summary>
            /// 商户id为空 
            /// 确认商户id正确传入
            /// </summary>
            MCH_ID_EMPTY,
            /// <summary>
            /// 商户id有误 
            /// 确认商户id是否正确并合法
            /// </summary>
            CODE_2_ID_ERR,
            /// <summary>
            /// 获取批次信息失败 
            /// 确认批次id信息正确
            /// </summary>
            GET_COUPON_STOCK_FAIL,
            /// <summary>
            /// 批次信息不存在 
            /// 确认批次id信息正确
            /// </summary>
            COUPON_STOCK_NOT_FOUND,
            /// <summary>
            /// 网络环境不佳,请重试 
            /// 请重试
            /// </summary>
            NETWORKERROR,
        }

        public class CouponStockType
        {
            public const int 批量型 = 1;
            public const int 触发型 = 2;
        }

        public class CouponState
        {
            public const int 已激活 = 2;
            public const int 已锁定 = 4;
            public const int 已实扣 = 8;
        }

        public enum SendSource
        {
            /// <summary>
            /// 即发即用 
            /// </summary>
            JIFA,
            /// <summary>
            /// 普通发劵 
            /// </summary>
            NORMAL,
            /// <summary>
            /// 满送活动送劵 
            /// </summary>
            FULL_SEND,
            /// <summary>
            /// 扫码领劵
            /// </summary>
            SCAN_CODE,
            /// <summary>
            /// 刮奖领劵 
            /// </summary>
            OZ,
            /// <summary>
            /// 对账调账
            /// </summary>
            AJUST
        }
    }
}
