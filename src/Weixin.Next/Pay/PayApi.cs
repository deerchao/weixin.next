using System;
using System.Threading.Tasks;
using Weixin.Next.Utilities;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    public abstract class PayApi<TParameter, TResult, TErrorCode>
        where TParameter : RequestData
        where TResult : ResponseData<TErrorCode>, new()
        where TErrorCode : struct
    {
        private readonly Report.Parameters _reportParameters;
        private DateTime _startTime;

        protected readonly Requester _requester;
        protected readonly bool _checkSignature;

        protected PayApi(Requester requester, bool checkSignature, bool generateReport)
        {
            _requester = requester;
            _checkSignature = checkSignature;
            if (generateReport)
                _reportParameters = new Report.Parameters();
        }

        public async Task<TResult> Invoke(TParameter parameter)
        {
            string interface_url;
            bool requiresCert;
            GetApiUrl(parameter, out interface_url, out requiresCert);

            if (_reportParameters != null)
            {
                var device_info = GetReportDeviceNo(parameter);
                await StartReportGeneration(interface_url, device_info).ConfigureAwait(false);
            }

            var result = await _requester.SendRequest<TResult, TErrorCode>("https://api.mch.weixin.qq.com/pay/unifiedorder", requiresCert, parameter, _checkSignature).ConfigureAwait(false);

            if (_reportParameters != null)
            {
                var out_trade_no = GetReportOutTradeNo(parameter, result);
                StopReportGeneration(result.return_code, result.return_msg, result.result_code, result.err_code, result.err_code_des, out_trade_no);
            }

            return result;
        }

        public async Task SendReport()
        {
            if (_reportParameters != null)
            {
                await new Report(_requester).Invoke(_reportParameters).ConfigureAwait(false);
            }
        }

        protected abstract void GetApiUrl(TParameter parameter, out string interface_url, out bool requiresCert);

        protected abstract string GetReportDeviceNo(TParameter parameter);

        protected abstract string GetReportOutTradeNo(TParameter parameter, TResult result);

        private async Task StartReportGeneration(string interface_url, string device_info = null)
        {
            _reportParameters.interface_url = interface_url;
            _reportParameters.device_info = device_info;
            _reportParameters.user_ip = await ServerHelper.GetLocalIP().ConfigureAwait(false);

            _startTime = DateTime.UtcNow;
            _reportParameters.time = _startTime.ToLocalTime();
        }

        private void StopReportGeneration(string return_code, string return_msg, string result_code, string err_code, string err_code_des, string out_trade_no)
        {
            _reportParameters.execute_time = (int)(DateTime.UtcNow - _startTime).TotalMilliseconds;

            _reportParameters.return_code = return_code;
            _reportParameters.return_msg = return_msg;
            _reportParameters.result_code = result_code;
            _reportParameters.err_code = err_code;
            _reportParameters.err_code_des = err_code_des;
            _reportParameters.out_trade_no = out_trade_no;
        }
    }
}
