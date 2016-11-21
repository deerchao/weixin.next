using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Weixin.Next.MP.Api;
using Weixin.Next.Utilities;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 下载对账单
    /// </summary>
    public class DownloadBill
    {
        private readonly Requester _requester;
        private readonly bool _checkSignature;

        public DownloadBill(Requester requester, bool checkSignature)
        {
            _requester = requester;
            _checkSignature = checkSignature;
        }

        /// <summary>
        /// 调用接口
        /// </summary>
        /// <param name="outcoming">请求参数</param>
        /// <param name="stream">正常情况下返回的数据流, 内容是 CSV 格式的数据</param>
        /// <param name="incoming">出错时的返回结果</param>
        /// <returns>正常情况: true, 出错时: false</returns>
        public async Task<bool> Invoke(Outcoming outcoming, AsyncOutParameter<Stream> stream, AsyncOutParameter<Incoming> incoming)
        {
            var response = await _requester.GetResponse("https://api.mch.weixin.qq.com/pay/downloadbill", false, outcoming).ConfigureAwait(false);
            var netStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var peekStream = new PeekableStream(netStream, 1);
            var buffer = new byte[1];
            await peekStream.PeekAsync(buffer, 0, 1).ConfigureAwait(false);
            if (buffer[0] != '<')
            {
                stream.SetValue(peekStream);
                return true;
            }

            using (var reader = new StreamReader(peekStream, Encoding.UTF8))
            {
                var responseBody = await reader.ReadToEndAsync().ConfigureAwait(false);
                incoming.SetValue(_requester.ParseResponse<Incoming, ErrorCode>(responseBody, _checkSignature));
                return false;
            }
        }

        public class Outcoming : OutcomingData
        {
            /// <summary>
            /// 可选, 终端设备号(门店号或收银设备ID)，注意：PC网页或公众号内支付请传"WEB"
            /// </summary>
            public string device_info { get; set; }
            /// <summary>
            /// 下载对账单的日期，格式：20140603
            /// </summary>
            public DateTime bill_date { get; set; }
            /// <summary>
            /// 账单类型
            /// </summary>
            public BillType bill_type { get; set; }
            /// <summary>
            /// 压缩账单,非必传参数，固定值：GZIP，返回格式为.gzip的压缩包账单。不传则默认为数据流形式。
            /// </summary>
            public TarType? tar_type { get; set; }

            public override IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser)
            {
                yield return new KeyValuePair<string, string>("device_info", device_info);
                yield return new KeyValuePair<string, string>("bill_date", bill_date.ToString("yyyyMMdd"));
                yield return new KeyValuePair<string, string>("bill_type", bill_type.ToString("G"));
                yield return new KeyValuePair<string, string>("tar_type", tar_type?.ToString("G"));
            }
        }

        public class Incoming : IncomingData<ErrorCode>
        {
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 接口返回错误
            /// 系统超时
            /// 请尝试再次查询。
            /// </summary>
            SYSTEMERROR,
            /// <summary>
            /// 无效transaction_id
            /// 请求参数未按指引进行填写
            /// 参数错误，请重新检查
            /// </summary>
            INVALID_TRANSACTIONID,
            /// <summary>
            /// 参数错误
            /// 请求参数未按指引进行填写
            /// 参数错误，请重新检查
            /// </summary>
            PARAM_ERROR,
        }

        public enum BillType
        {
            /// <summary>
            /// 返回当日所有订单信息，默认值 
            /// </summary>
            ALL,
            /// <summary>
            /// 返回当日成功支付的订单 
            /// </summary>
            SUCCESS,
            /// <summary>
            /// 返回当日退款订单 
            /// </summary>
            REFUND,
        }

        public enum TarType
        {
            GZIP,
        }
    }
}
