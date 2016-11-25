using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 查询代金券批次信息
    /// </summary>
    public class QueryCouponStock : PayApi<QueryCouponStock.Outcoming, QueryCouponStock.Incoming, QueryCouponStock.ErrorCode>
    {
        public QueryCouponStock(Requester requester, bool checkSignature, bool generateReport)
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
            interface_url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/query_coupon_stock";
            requiresCert = false;
        }

        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 代金券批次id
            /// </summary>
            public string coupon_stock_id { get; set; }
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
            /// 代金券名称
            /// </summary>
            public string coupon_name { get; set; }
            /// <summary>
            /// 代金券面值,单位是分
            /// </summary>
            public int? coupon_value { get; set; }
            /// <summary>
            /// 代金券使用最低限额,单位是分
            /// </summary>
            public int? coupon_mininumn { get; set; }
            /// <summary>
            /// 代金券类型：1-代金券无门槛，2-代金券有门槛互斥，3-代金券有门槛叠加，
            /// </summary>
            public int? coupon_type { get; set; }
            /// <summary>
            /// 批次状态： 1-未激活；2-审批中；4-已激活；8-已作废；16-中止发放；
            /// </summary>
            public int? coupon_stock_status { get; set; }
            /// <summary>
            /// 代金券数量
            /// </summary>
            public int? coupon_total { get; set; }
            /// <summary>
            /// 代金券每个人最多能领取的数量, 如果为0，则表示没有限制
            /// </summary>
            public int? max_quota { get; set; }
            /// <summary>
            /// 代金券锁定数量
            /// </summary>
            public int? locked_num { get; set; }
            /// <summary>
            /// 代金券已使用数量
            /// </summary>
            public int? used_num { get; set; }
            /// <summary>
            /// 代金券已经发送的数量
            /// </summary>
            public int? is_send_num { get; set; }
            /// <summary>
            /// 生效开始时间
            /// </summary>
            public DateTime? begin_time { get; set; }
            /// <summary>
            /// 生效结束时间
            /// </summary>
            public DateTime? end_time { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime? create_time { get; set; }
            /// <summary>
            /// 代金券预算额度
            /// </summary>
            public int? coupon_budget { get; set; }

            protected override void DeserializeFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                appid = GetValue(values, "appid");
                mch_id = GetValue(values, "mch_id");
                device_info = GetValue(values, "device_info");
            }

            protected override void DeserializeSuccessFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                coupon_stock_id = GetValue(values, "coupon_stock_id");
                coupon_name = GetValue(values, "coupon_name");
                coupon_value = GetIntValue(values, "coupon_value");
                coupon_mininumn = GetIntValue(values, "coupon_mininumn");
                coupon_type = GetIntValue(values, "coupon_type");
                coupon_stock_status = GetIntValue(values, "coupon_stock_status");
                coupon_total = GetIntValue(values, "coupon_total");
                max_quota = GetIntValue(values, "max_quota");
                locked_num = GetIntValue(values, "locked_num");
                used_num = GetIntValue(values, "used_num");
                is_send_num = GetIntValue(values, "is_send_num");
                coupon_budget = GetIntValue(values, "coupon_budget");

                begin_time = DateTime.ParseExact(GetValue(values, "begin_time"), "yyyyMMddHHmmss", null);
                end_time = DateTime.ParseExact(GetValue(values, "end_time"), "yyyyMMddHHmmss", null);
                create_time = DateTime.ParseExact(GetValue(values, "create_time"), "yyyyMMddHHmmss", null);
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 批次id不正确 
            /// 确认批次id正确性，以及和商户id所属关系是否正确
            /// </summary>
            COUPON_STOCK_ID_NOT_VALID,
            /// <summary>
            /// 签名错误 
            /// 验证签名有误，参见3.2.1
            /// </summary>
            SIGN_ERROR,
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
            /// 获取批次信息失败 
            /// 确认批次id信息正确
            /// </summary>
            GET_COUPON_STOCK_FAIL,
            /// <summary>
            /// 批次信息不存在 
            /// 确认批次id信息正确
            /// </summary>
            COUPON_STOCK_NOT_FOUND,
        }

        public static class CouponType
        {
            public const int 代金券无门槛 = 1;
            public const int 代金券有门槛互斥 = 2;
            public const int 代金券有门槛叠加 = 3;
        }

        public static class CouponStockStatus
        {
            public const int 未激活 = 1;
            public const int 审批中 = 2;
            public const int 已激活 = 4;
            public const int 已作废 = 8;
            public const int 中止发放 = 16;
        }
    }
}
