using System;
using System.Collections.Generic;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 查询企业付款
    /// </summary>
    public class GetTransferInfo : PayApi<GetTransferInfo.Outcoming, GetTransferInfo.Incoming, GetTransferInfo.ErrorCode>
    {
        public GetTransferInfo(Requester requester, bool checkSignature, bool generateReport)
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
            interface_url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/gettransferinfo";
            requiresCert = true;
        }


        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 商户订单号
            /// </summary>
            public string partner_trade_no { get; set; }

            public override IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser)
            {
                yield return new KeyValuePair<string, string>("partner_trade_no", partner_trade_no);
            }
        }

        public class Incoming : IncomingData<ErrorCode>
        {
            /// <summary>
            /// 调用接口提交的公众账号ID，仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public string appid { get; set; }
            /// <summary>
            /// 调用接口提交的商户号，仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public string mch_id { get; set; }

            /// <summary>
            /// 商户订单号，仅在return_code 和result_code都为SUCCESS的时候有意义 
            /// </summary>
            public string partner_trade_no { get; set; }
            /// <summary>
            /// 付款单号
            /// </summary>
            public string detail_id { get; set; }
            /// <summary>
            /// 转账状态
            /// </summary>
            public TransferStatus Status { get; set; }
            /// <summary>
            /// 失败原因
            /// </summary>
            public string reason { get; set; }
            /// <summary>
            /// 收款用户标识
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 收款用户姓名 
            /// </summary>
            public string transfer_name { get; set; }
            /// <summary>
            /// 付款金额,单位分
            /// </summary>
            public int payment_amount { get; set; }
            /// <summary>
            /// 转账时间
            /// </summary>
            public DateTime transfer_time { get; set; }
            /// <summary>
            /// 付款描述
            /// </summary>
            public string desc { get; set; }
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
            /// 系统繁忙。
            /// </summary>

            SYSTEMERROR,
            /// <summary>
            /// 指定单号数据不存在 
            /// 查询单号对应的数据不存在，请使用正确的商户订单号查询 
            /// </summary>
            NOT_FOUND,
        }

        public enum TransferStatus
        {
            /// <summary>
            /// 转账成功
            /// </summary>
            SUCCESS,
            /// <summary>
            /// 转账失败
            /// </summary>
            FAILED,
            /// <summary>
            /// 处理中
            /// </summary>
            PROCESSING,
        }
    }
}
