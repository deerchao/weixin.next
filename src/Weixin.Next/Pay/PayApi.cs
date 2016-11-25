using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Weixin.Next.MP.Api;
using Weixin.Next.Utilities;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// 主动发送的接口参数, 不包含通用的参数: appid, mch_id, nonce_str, sign
    /// </summary>
    public abstract class OutcomingData
    {
        public abstract IEnumerable<KeyValuePair<string, string>> GetFields(IJsonParser jsonParser);

        public virtual string AppIdFieldName { get { return "appid"; } }
        public virtual string MerchantIdFieldName { get { return "mch_id"; } }
    }

    /// <summary>
    /// 接收到的数据(主动调用时的回应, 被动通知的请求)
    /// </summary>
    public abstract class IncomingData<TErrorCode>
        where TErrorCode : struct
    {
        /// <summary>
        /// 通信成功时 return_code 的值: SUCCESS
        /// </summary>
        public const string return_success = "SUCCESS";
        /// <summary>
        /// 通信失败时 return_code 的值: SUCCESS
        /// </summary>
        public const string return_fail = "FAIL";
        /// <summary>
        /// 调用成功时 return_code 的值: SUCCESS
        /// </summary>
        public const string result_success = "SUCCESS";
        /// <summary>
        /// 调用失败时 return_code 的值: SUCCESS
        /// </summary>
        public const string result_fail = "FAIL";

        /// <summary>
        /// 返回状态码, SUCCESS/FAIL 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息 返回信息，如非空，为错误原因 
        /// </summary>
        public string return_msg { get; set; }
        /// <summary>
        /// 业务结果 SUCCESS/FAIL
        /// </summary>
        public string result_code { get; set; }

        /// <summary>
        /// 错误代码, 仅在result_code为FAIL的时候有意义
        /// </summary>
        public string err_code { get; set; }
        /// <summary>
        /// 错误代码描述, 仅在result_code为FAIL的时候有意义
        /// </summary>
        public string err_code_des { get; set; }

        public TErrorCode? GetErrorCode()
        {
            if (err_code == null)
                return null;

            return (TErrorCode?)Enum.Parse(typeof(TErrorCode), err_code);
        }

        public void Deserialize(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
        {
            return_code = GetValue(values, "return_code");
            return_msg = GetValue(values, "return_msg");


            if (return_code == return_success)
            {
                result_code = GetValue(values, "result_code");

                DeserializeFields(values, jsonParser, xml);

                if (result_code == result_success)
                {
                    DeserializeSuccessFields(values, jsonParser, xml);
                }
                else
                {
                    err_code = GetValue(values, "err_code");
                    err_code_des = GetValue(values, "err_code_des");
                }
            }
        }

        protected virtual void DeserializeFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
        {
        }

        protected virtual void DeserializeSuccessFields(List<KeyValuePair<string, string>> values, IJsonParser jsonParser, XElement xml)
        {
        }


        protected static string GetValue(List<KeyValuePair<string, string>> values, string key)
        {
            var index = values.FindIndex(x => x.Key == key);
            return index < 0
                ? null
                : values[index].Value;
        }

        protected static int? GetIntValue(List<KeyValuePair<string, string>> values, string key)
        {
            var v = GetValue(values, key);
            return v == null
                ? (int?)null
                : int.Parse(v);
        }
    }

    public abstract class PayApi<TOutcoming, TIncoming, TErrorCode>
        where TOutcoming : OutcomingData
        where TIncoming : IncomingData<TErrorCode>, new()
        where TErrorCode : struct
    {
        private readonly Report.Outcoming _reportOutcoming;
        private DateTime _startTime;

        protected readonly Requester _requester;
        protected readonly bool _checkSignature;

        protected PayApi(Requester requester, bool checkSignature, bool generateReport)
        {
            _requester = requester;
            _checkSignature = checkSignature;
            if (generateReport)
                _reportOutcoming = new Report.Outcoming();
        }

        /// <summary>
        /// 调用接口
        /// </summary>
        /// <param name="outcoming">请求参数</param>
        /// <returns></returns>
        public async Task<TIncoming> Invoke(TOutcoming outcoming)
        {
            string interface_url;
            bool requiresCert;
            GetApiUrl(outcoming, out interface_url, out requiresCert);

            if (_reportOutcoming != null)
            {
                var device_info = GetReportDeviceNo(outcoming);
                await StartReportGeneration(interface_url, device_info).ConfigureAwait(false);
            }

            var incoming = await _requester.SendRequest<TIncoming, TErrorCode>("https://api.mch.weixin.qq.com/pay/unifiedorder", requiresCert, outcoming, _checkSignature).ConfigureAwait(false);

            if (_reportOutcoming != null)
            {
                var out_trade_no = GetReportOutTradeNo(outcoming, incoming);
                StopReportGeneration(incoming.return_code, incoming.return_msg, incoming.result_code, incoming.err_code, incoming.err_code_des, out_trade_no);
            }

            return incoming;
        }

        /// <summary>
        /// 发送报告(调用接口耗时). 在 generateReport 为 true 时才有意义
        /// </summary>
        /// <returns></returns>
        public async Task SendReport()
        {
            if (_reportOutcoming != null)
            {
                await new Report(_requester).Invoke(_reportOutcoming).ConfigureAwait(false);
            }
        }

        protected abstract void GetApiUrl(TOutcoming outcoming, out string interface_url, out bool requiresCert);

        protected abstract string GetReportDeviceNo(TOutcoming outcoming);

        protected abstract string GetReportOutTradeNo(TOutcoming outcoming, TIncoming incoming);

        private async Task StartReportGeneration(string interface_url, string device_info = null)
        {
            _reportOutcoming.interface_url = interface_url;
            _reportOutcoming.device_info = device_info;
            _reportOutcoming.user_ip = await ServerHelper.GetLocalIP().ConfigureAwait(false);

            _startTime = DateTime.UtcNow;
            _reportOutcoming.time = _startTime.ToLocalTime();
        }

        private void StopReportGeneration(string return_code, string return_msg, string result_code, string err_code, string err_code_des, string out_trade_no)
        {
            _reportOutcoming.execute_time = (int)(DateTime.UtcNow - _startTime).TotalMilliseconds;

            _reportOutcoming.return_code = return_code;
            _reportOutcoming.return_msg = return_msg;
            _reportOutcoming.result_code = result_code;
            _reportOutcoming.err_code = err_code;
            _reportOutcoming.err_code_des = err_code_des;
            _reportOutcoming.out_trade_no = out_trade_no;
        }
    }
}
