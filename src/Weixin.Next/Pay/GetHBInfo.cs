using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    //todo: 确认服务器端的签名算法是怎么处理树形数据的
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 查询红包记录
    /// </summary>
    public class GetHBInfo : PayApi<GetHBInfo.Outcoming, GetHBInfo.Incoming, GetHBInfo.ErrorCode>
    {
        public GetHBInfo(Requester requester, bool checkSignature, bool generateReport)
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
            interface_url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/gethbinfo";
            requiresCert = true;
        }
        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 商户订单号（每个订单号必须唯一）组成：mch_id+yyyymmdd+10位一天内不能重复的数字
            /// </summary>
            public string mch_billno { get; set; }
            /// <summary>
            /// 订单类型 
            /// </summary>
            public BillType bill_type { get; set; }

            public override IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser)
            {
                yield return new KeyValuePair<string, string>("mch_billno", mch_billno);
                yield return new KeyValuePair<string, string>("bill_type", bill_type.ToString("G"));
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
            /// 红包单号 
            /// </summary>
            public string detail_id { get; set; }
            /// <summary>
            /// 红包状态 
            /// </summary>
            public RedPackStatus status { get; set; }
            /// <summary>
            /// 发放类型 
            /// </summary>
            public SendType send_type { get; set; }
            /// <summary>
            /// 红包类型 
            /// </summary>
            public RedPackType hb_type { get; set; }
            /// <summary>
            /// 红包个数 
            /// </summary>
            public int total_num { get; set; }
            /// <summary>
            /// 红包总金额（单位分） 
            /// </summary>
            public int? total_amount { get; set; }
            /// <summary>
            /// 发送失败原因 
            /// </summary>
            public string reason { get; set; }
            /// <summary>
            /// 红包发送时间 
            /// </summary>
            public DateTime send_time { get; set; }
            /// <summary>
            /// 红包的退款时间（如果其未领取的退款） 
            /// </summary>
            public DateTime? refund_time { get; set; }
            /// <summary>
            /// 红包退款金额 
            /// </summary>
            public int refund_amount { get; set; }
            /// <summary>
            /// 祝福语 
            /// </summary>
            public string wishing { get; set; }
            /// <summary>
            /// 活动描述，低版本微信可见 
            /// </summary>
            public string remark { get; set; }
            /// <summary>
            /// 发红包的活动名称 
            /// </summary>
            public string act_name { get; set; }
            /// <summary>
            /// 裂变红包的领取列表 
            /// </summary>
            public HBInfo[] hblist { get; set; }

            protected override void DeserializeSuccessFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
            {
                mch_billno = GetValue(values, "mch_billno");
                mch_id = GetValue(values, "mch_id");
                detail_id = GetValue(values, "detail_id");
                status = (RedPackStatus)Enum.Parse(typeof(RedPackStatus), GetValue(values, "status"));
                send_type = (SendType)Enum.Parse(typeof(SendType), GetValue(values, "send_type"));
                hb_type = (RedPackType)Enum.Parse(typeof(RedPackType), GetValue(values, "hb_type"));
                total_num = GetIntValue(values, "total_num") ?? 0;
                refund_amount = GetIntValue(values, "refund_amount") ?? 0;
                reason = GetValue(values, "reason");
                total_amount = GetIntValue(values, "total_amount");
                wishing = GetValue(values, "wishing");
                remark = GetValue(values, "remark");
                act_name = GetValue(values, "act_name");
                send_time = DateTime.ParseExact(GetValue(values, "send_time"), "yyyy-MM-dd HH:mm:ss", null);
                if (GetValue(values, "refund_time") != null)
                {
                    refund_time = DateTime.ParseExact(GetValue(values, "refund_time"), "yyyy-MM-dd HH:mm:ss", null);
                }

                hblist = xml.Element("hblist")
                    ?.Elements("hbinfo")
                    .Select(x => new HBInfo
                    {
                        openid = x.Element("openid")?.Value,
                        amount = int.Parse(x.Element("amount").Value),
                        rcv_time = DateTime.ParseExact(x.Element("rcv_time").Value, "yyyy-MM-dd HH:mm:ss", null),
                    })
                    .ToArray();
            }
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 请求未携带证书，或请求携带的证书出错 
            /// 到商户平台下载证书，请求带上证书后重试。
            /// </summary>
            CA_ERROR,
            /// <summary>
            /// 商户签名错误 
            /// 按文档要求重新生成签名后再重试。
            /// </summary>
            SIGN_ERROR,
            /// <summary>
            /// 没有权限 
            /// 请联系微信支付开通api权限
            /// </summary>
            NO_AUTH,
            /// <summary>
            /// 指定单号数据不存在 
            /// 查询单号对应的数据不存在，请使用正确的商户订单号查询
            /// </summary>
            NOT_FOUND,
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
            /// 系统繁忙，请再试。 
            /// 红包系统繁忙。
            /// </summary>
            SYSTEMERROR,
        }

        public enum RedPackStatus
        {
            /// <summary>
            /// 发放中
            /// </summary>
            SENDING,
            /// <summary>
            /// 已发放待领取
            /// </summary>
            SENT,
            /// <summary>
            /// 发放失败
            /// </summary>
            FAILED,
            /// <summary>
            /// 已领取
            /// </summary>
            RECEIVED,
            /// <summary>
            /// 退款中
            /// </summary>
            RFUND_ING,
            /// <summary>
            /// 已退款
            /// </summary>
            REFUND,
        }

        public enum SendType
        {
            /// <summary>
            /// 通过API接口发放
            /// </summary>
            API,
            /// <summary>
            /// 通过上传文件方式发放
            /// </summary>
            UPLOAD,
            /// <summary>
            /// 通过活动方式发放
            /// </summary>
            ACTIVITY
        }

        public enum RedPackType
        {
            /// <summary>
            /// 裂变红包
            /// </summary>
            GROUP,
            /// <summary>
            /// 普通红包
            /// </summary>
            NORMAL
        }

        public class HBInfo
        {
            /// <summary>
            /// 领取红包的Openid
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 领取金额 
            /// </summary>
            public int amount { get; set; }
            /// <summary>
            /// 领取红包的时间
            /// </summary>
            public DateTime rcv_time { get; set; }
        }

        public enum BillType
        {
            /// <summary>
            /// 通过商户订单号获取红包信息
            /// </summary>
            MCHT
        }
    }
}
