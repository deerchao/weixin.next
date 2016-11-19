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
    /// <summary>
    /// 负责序列化, 签名, 验证等通信底层工作
    /// </summary>
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

        private string BuildRequestBody(OutcomingData data)
        {
            var nonce = _random.Next().ToString("D");
            var items = data.GetFields().Concat(new[]
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

        public async Task<TIncoming> SendRequest<TIncoming, TErrorCode>(string url, bool requiresClientCert, OutcomingData data, bool checkSignatue)
            where TIncoming : IncomingData<TErrorCode>, new()
            where TErrorCode : struct
        {
            var response = await GetResponse(url, requiresClientCert, data);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return ParseResponse<TIncoming, TErrorCode>(responseBody, checkSignatue);
        }

        public TIncoming ParseResponse<TIncoming, TErrorCode>(string responseBody, bool checkSignatue)
            where TIncoming : IncomingData<TErrorCode>, new()
            where TErrorCode : struct
        {
            var xml = XElement.Parse(responseBody);
            var values = xml.Elements()
                .Select(x => new KeyValuePair<string, string>(x.Name.LocalName, x.Value))
                .ToList();

            if (checkSignatue)
            {
                var codeIndex = values.FindIndex(x => x.Key == "return_code");
                if (codeIndex >= 0 && values[codeIndex].Value == IncomingData<TErrorCode>.return_success)
                {
                    var signIndex = values.FindIndex(x => x.Key == "sign");
                    if (signIndex < 0 || values[signIndex].Value != ComputeSign(values))
                        throw new IncomingSignatureException();
                }
            }

            var incoming = new TIncoming();
            incoming.Deserialize(values);
            return incoming;
        }

        public async Task<HttpResponseMessage> GetResponse(string url, bool requiresClientCert, OutcomingData data)
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
}
