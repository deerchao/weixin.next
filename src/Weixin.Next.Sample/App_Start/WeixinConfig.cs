using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using Weixin.Next.Api;
using Weixin.Next.Json.Net;
using Weixin.Next.Sample.Models;

namespace Weixin.Next.Sample
{
    public static class WeixinConfig
    {
        public static SampleMessageCenter MessageCenter { get; private set; }
        public static JsapiTicketManager JsapiTicketManager { get; private set; }
        /// <summary>
        /// 是否启用 API 日志
        /// </summary>
        public static bool LoggingEnabled { get; set; }


        public static void Setup()
        {
            CreateMessageCenter();
            SetDefaultApiConfig();
            CreateJsapiTicketManager();
        }


        private static void CreateMessageCenter()
        {
            var appId = ConfigurationManager.AppSettings["weixin.appId"];
            var token = ConfigurationManager.AppSettings["weixin.token"];
            var encodingAESKey = ConfigurationManager.AppSettings["weixin.encodingAESKey"];

            var messageCenter = new SampleMessageCenter(appId, token, encodingAESKey);

            messageCenter.Initialize();

            MessageCenter = messageCenter;
        }


        private static void SetDefaultApiConfig()
        {
            var manager = CreateAccessTokenManager();
            var config = new ApiConfig
            {
                JsonParser = new JsonParser(),
                AccessTokenManager = manager,
                HttpClient = CreateHttpClient(),
            };
            manager.Config = config;

            ApiHelper.SetDefaultConfig(config);
        }

        private static AccessTokenManager CreateAccessTokenManager()
        {
            var appId = ConfigurationManager.AppSettings["weixin.appId"];
            var appSecret = ConfigurationManager.AppSettings["weixin.appSecret"];
            return new AccessTokenManager(appId, appSecret);
        }

        private static HttpClient CreateHttpClient()
        {
            // 使用 LoggingHandler 为 HttpClient 添加日志功能

            var handler = new LoggingHandler(Log,
                () => LoggingEnabled,
                new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.Deflate |
                                             DecompressionMethods.GZip
                });
            return new HttpClient(handler);
        }

        private static void Log(string message)
        {
            // 可以改为写入到文件, 使用 NLog 等
            Debug.WriteLine(message);
        }


        private static void CreateJsapiTicketManager()
        {
            JsapiTicketManager = new JsapiTicketManager(ApiHelper.DefaultConfig);
        }
    }
}