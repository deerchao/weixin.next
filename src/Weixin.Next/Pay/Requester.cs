using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    public class Requester
    {
        private static readonly Random _random = new Random();
        private readonly string _appid;
        private readonly string _mch_id;
        private readonly string _key;
        private readonly X509Certificate2 _cert;

        public Requester(string appid, string mch_id, string key, X509Certificate2 cert)
        {
            _appid = appid;
            _mch_id = mch_id;
            _key = key;
            _cert = cert;
        }

        private string BuildRequestBody(RequestData data)
        {
            var nonce = _random.Next().ToString("D");
            var items = data.GetParameters().Concat(new[]
                {
                    new KeyValuePair<string, string>("appid", _appid),
                    new KeyValuePair<string, string>("mch_id", _mch_id),
                    new KeyValuePair<string, string>("nonce_str", nonce),
                }).Where(x => !string.IsNullOrEmpty(x.Value))
                .ToList();

            items.Add(new KeyValuePair<string, string>("sign", ComputeSign(items)));

            var xml = new XElement("xml", items.Select(x => new XElement(x.Key, x.Value)));
            return xml.ToString(SaveOptions.DisableFormatting);
        }

        private string ComputeSign(List<KeyValuePair<string, string>> items)
        {
            var stringA = string.Join("&", items.Where(x => x.Key != "sign").OrderBy(x => x.Key).Select(x => $"{x.Key}={x.Value}"));
            var stringSignTemp = stringA + "&key=" + _key;
            var sign = string.Concat(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(stringSignTemp))
                .Select(x => x.ToString("X2")));
            return sign;
        }

        public async Task<TResult> SendRequest<TResult, TErrorCode>(string url, bool requiresClientCert, RequestData data, bool checkSignatue)
            where TResult : ResponseData<TErrorCode>, new()
            where TErrorCode : struct 
        {
            var response = await GetResponse(url, requiresClientCert, data);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return ParseResponse<TResult, TErrorCode>(responseBody, checkSignatue);
        }

        public TResult ParseResponse<TResult, TErrorCode>(string responseBody, bool checkSignatue)
            where TResult : ResponseData<TErrorCode>, new()
            where TErrorCode : struct
        {
            var xml = XElement.Parse(responseBody);
            var values = xml.Elements()
                .Select(x => new KeyValuePair<string, string>(x.Name.LocalName, x.Value))
                .ToList();

            if (checkSignatue)
            {
                var codeIndex = values.FindIndex(x => x.Key == "return_code");
                if (codeIndex >= 0 && values[codeIndex].Value == ResponseData<TErrorCode>.return_success)
                {
                    var signIndex = values.FindIndex(x => x.Key == "sign");
                    if (signIndex < 0 || values[signIndex].Value != ComputeSign(values))
                        throw new ResponseSignatureException();
                }
            }

            var result = new TResult();
            result.Deserialize(values);
            return result;
        }

        public async Task<HttpResponseMessage> GetResponse(string url, bool requiresClientCert, RequestData data)
        {
            var requestBody = BuildRequestBody(data);
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(requestBody, Encoding.UTF8) };

            WebRequestHandler handler = null;
            if (requiresClientCert)
            {
                handler = new WebRequestHandler { ClientCertificateOptions = ClientCertificateOption.Manual };
                handler.ClientCertificates.Add(_cert);
            }

            var http = CreateHttpClient(handler);

            var response = await http.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response;
        }

        protected virtual HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return new HttpClient(handler);
        }
    }

    /// <summary>
    /// 接口参数, 不包含通用的参数: appid, mch_id, nonce_str, sign
    /// </summary>
    public abstract class RequestData
    {
        public abstract IEnumerable<KeyValuePair<string, string>> GetParameters();
    }

    /// <summary>
    /// 返回数据
    /// </summary>
    public abstract class ResponseData<TErrorCode>
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

        public void Deserialize(List<KeyValuePair<string, string>> values)
        {
            return_code = GetValue(values, "return_code");
            return_msg = GetValue(values, "return_msg");


            if (return_code == return_success)
            {
                result_code = GetValue(values, "result_code");

                DeserializeFields(values);

                if (result_code == result_success)
                {
                    DeserializeSuccessFields(values);
                }
                else
                {
                    err_code = GetValue(values, "err_code");
                    err_code_des = GetValue(values, "err_code_des");
                }
            }
        }

        protected virtual void DeserializeFields(List<KeyValuePair<string, string>> values)
        {
        }

        protected virtual void DeserializeSuccessFields(List<KeyValuePair<string, string>> values)
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
}
