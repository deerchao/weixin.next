using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Weixin.Next.Utilities;

namespace Weixin.Next.MP.Api
{
    public static class ApiHelper
    {
        private static ApiConfig _defaultConfig;
        private const int ErrorAccessTokenExpired = ApiErrorCode.E42001;
        private const int ErrorAccessTokenInvalid = ApiErrorCode.E40001;
        private const int ErrorSuccess = ApiErrorCode.E0;

        public static ApiConfig DefaultConfig { get { return _defaultConfig; } }

        public static void SetDefaultConfig(ApiConfig defaultConfig)
        {
            _defaultConfig = defaultConfig;
        }

        #region Get
        /// <summary>
        /// 发送 Get 请求, 返回字符串
        /// </summary>
        /// <param name="url">接口网址, 里边的 $acac$ 会被自动替换成 access_token=xxx </param>
        /// <param name="config"></param>
        public static Task<string> GetString(string url, ApiConfig config = null)
        {
            return GetStringWithToken(url, config, null);
        }

        private static async Task<string> GetStringWithToken(string url, ApiConfig config, AsyncOutParameter<string> token)
        {
            var stream = await GetStreamWithToken(url, config, token).ConfigureAwait(false);
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 发送 Get 请求, 返回流
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<Stream> GetStream(string url, ApiConfig config = null)
        {
            return GetStreamWithToken(url, config, null);
        }

        private static async Task<Stream> GetStreamWithToken(string url, ApiConfig config, AsyncOutParameter<string> token)
        {
            url = await FormatUrl(url, config, token).ConfigureAwait(false);
            var http = config?.HttpClient ?? _defaultConfig.HttpClient;
            return await http.GetStreamAsync(url).ConfigureAwait(false);
        }

        /// <summary>
        /// 发送 Get 请求, 返回结果对象；如遇 access_token 超时错误, 会自动获取新的 access_token 并重试
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="url">接口网址, 里边的 $acac$ 会被自动替换成 access_token=xxx </param>
        /// <param name="config"></param>
        /// <exception cref="ApiException">返回的 JSON 字符串中包含了错误信息</exception>
        public static async Task<T> GetResult<T>(string url, ApiConfig config = null)
            where T : IApiResult
        {
            var token = new AsyncOutParameter<string>();
            try
            {
                var text = await GetStringWithToken(url, config, token).ConfigureAwait(false);
                return BuildResult<T>(text, config);
            }
            catch (ApiException ex)
            {
                if (ex.Code == ErrorAccessTokenExpired || ex.Code == ErrorAccessTokenInvalid)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.RefreshTokenInfo(token.Value).ConfigureAwait(false);

                    var text = await GetString(url, config).ConfigureAwait(false);
                    return BuildResult<T>(text, config);
                }

                throw;
            }
        }

        /// <summary>
        /// 发送 Get 请求, 并检查返回结果是否用错误；如遇 access_token 超时错误, 会自动获取新的 access_token 并重试
        /// </summary>
        /// <param name="url">接口网址, 里边的 $acac$ 会被自动替换成 access_token=xxx </param>
        /// <param name="config"></param>
        /// <exception cref="ApiException">返回的 JSON 字符串中包含了错误信息</exception>
        public static async Task GetVoid(string url, ApiConfig config = null)
        {
            var token = new AsyncOutParameter<string>();
            try
            {
                var text = await GetStringWithToken(url, config, token).ConfigureAwait(false);
                BuildVoid(text, config);
            }
            catch (ApiException ex)
            {
                if (ex.Code == ErrorAccessTokenExpired || ex.Code == ErrorAccessTokenInvalid)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.RefreshTokenInfo(token.Value).ConfigureAwait(false);

                    var text = await GetString(url, config).ConfigureAwait(false);
                    BuildVoid(text, config);
                    return;
                }

                throw;
            }
        }
        #endregion

        #region Post
        /// <summary>
        /// 发送 Post 请求, 返回字符串
        /// </summary>
        /// <param name="url">接口网址, 里边的 $acac$ 会被自动替换成 access_token=xxx </param>
        /// <param name="data">将会转换为 JSON 格式作为 Post 请求的正文内容</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<string> PostString(string url, object data, ApiConfig config = null)
        {
            return PostStringWithToken(url, data, config, null);
        }

        private static async Task<string> PostStringWithToken(string url, object data, ApiConfig config, AsyncOutParameter<string> token)
        {
            var stream = await PostStreamWithToken(url, data, config, token).ConfigureAwait(false);
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 发送 Post 请求, 返回流
        /// </summary>
        /// <param name="url">接口网址, 里边的 $acac$ 会被自动替换成 access_token=xxx </param>
        /// <param name="data">将会转换为 JSON 格式作为 Post 请求的正文内容</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<Stream> PostStream(string url, object data, ApiConfig config = null)
        {
            return PostStreamWithToken(url, data, config, null);
        }

        private static async Task<Stream> PostStreamWithToken(string url, object data, ApiConfig config, AsyncOutParameter<string> token)
        {
            url = await FormatUrl(url, config, token).ConfigureAwait(false);

            var parser = config?.JsonParser ?? _defaultConfig.JsonParser;
            var body = parser.ToString(data);

            var http = config?.HttpClient ?? _defaultConfig.HttpClient;
            var response = await http.PostAsync(url, new StringContent(body)).ConfigureAwait(false);
            return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 发送 Post 请求, 并返回结果对象；如遇 access_token 超时错误, 会自动获取新的 access_token 并重试
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="url">接口网址, 里边的 $acac$ 会被自动替换成 access_token=xxx </param>
        /// <param name="data">将会转换为 JSON 格式作为 Post 请求的正文内容</param>
        /// <param name="config"></param>
        /// <exception cref="ApiException">返回的 JSON 字符串中包含了错误信息</exception>
        public static async Task<T> PostResult<T>(string url, object data, ApiConfig config = null)
            where T : IApiResult
        {
            var token = new AsyncOutParameter<string>();
            try
            {
                var text = await PostString(url, data, config).ConfigureAwait(false);
                return BuildResult<T>(text, config);
            }
            catch (ApiException ex)
            {
                if (ex.Code == ErrorAccessTokenExpired || ex.Code == ErrorAccessTokenInvalid)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.RefreshTokenInfo(token.Value).ConfigureAwait(false);

                    var text = await PostString(url, data, config).ConfigureAwait(false);
                    return BuildResult<T>(text, config);
                }

                throw;
            }
        }

        /// <summary>
        /// 发送 Post 请求, 并检查返回结果是否用错误；如遇 access_token 超时错误, 会自动获取新的 access_token 并重试
        /// </summary>
        /// <param name="url">接口网址, 里边的 $acac$ 会被自动替换成 access_token=xxx </param>
        /// <param name="data">将会转换为 JSON 格式作为 Post 请求的正文内容</param>
        /// <param name="config"></param>
        /// <exception cref="ApiException">返回的 JSON 字符串中包含了错误信息</exception>
        public static async Task PostVoid(string url, object data, ApiConfig config = null)
        {
            var token = new AsyncOutParameter<string>();
            try
            {
                var text = await PostStringWithToken(url, data, config, token).ConfigureAwait(false);
                BuildVoid(text, config);
            }
            catch (ApiException ex)
            {
                if (ex.Code == ErrorAccessTokenExpired || ex.Code == ErrorAccessTokenInvalid)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.RefreshTokenInfo(token.Value).ConfigureAwait(false);

                    var text = await PostString(url, data, config).ConfigureAwait(false);
                    BuildVoid(text, config);
                    return;
                }

                throw;
            }
        }
        #endregion

        #region Upload

        /// <summary>
        /// 上传文件, 返回字符串
        /// </summary>
        /// <param name="url">接口网址, 里边的 $acac$ 会被自动替换成 access_token=xxx </param>
        /// <param name="fieldName">文件字段名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileStream">文件内容流</param>
        /// <param name="additionalFields">要在 POST 内容中发送的其它字段</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<string> UploadString(string url, string fieldName, string fileName, Stream fileStream, KeyValuePair<string, string>[] additionalFields = null, ApiConfig config = null)
        {
            return UploadStringWithToken(url, fieldName, fileName, fileStream, additionalFields, config, null);
        }

        private static async Task<string> UploadStringWithToken(string url, string fieldName, string fileName, Stream fileStream, KeyValuePair<string, string>[] additionalFields, ApiConfig config, AsyncOutParameter<string> token)
        {
            var content = BuildUploadContent(fieldName, fileName, fileStream, additionalFields);

            url = await FormatUrl(url, config, token).ConfigureAwait(false);
            var http = config?.HttpClient ?? _defaultConfig.HttpClient;

            var response = await http.PostAsync(url, content).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 上传文件, 并返回结果对象；如遇 access_token 超时错误, 会自动获取新的 access_token 并重试
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="url">接口网址, 里边的 $acac$ 会被自动替换成 access_token=xxx </param>
        /// <param name="fieldName">文件字段名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileStream">文件内容流</param>
        /// <param name="additionalFields">要在 POST 内容中发送的其它字段</param>
        /// <param name="config"></param>
        /// <exception cref="ApiException">返回的 JSON 字符串中包含了错误信息</exception>
        public static async Task<T> UploadResult<T>(string url, string fieldName, string fileName, Stream fileStream, KeyValuePair<string, string>[] additionalFields = null, ApiConfig config = null)
            where T : IApiResult
        {
            var token = new AsyncOutParameter<string>();
            try
            {
                var text = await UploadStringWithToken(url, fieldName, fileName, fileStream, additionalFields, config, token).ConfigureAwait(false);
                return BuildResult<T>(text, config);
            }
            catch (ApiException ex)
            {
                if (ex.Code == ErrorAccessTokenExpired || ex.Code == ErrorAccessTokenInvalid)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.RefreshTokenInfo(token.Value).ConfigureAwait(false);

                    var text = await UploadString(url, fieldName, fileName, fileStream, additionalFields, config).ConfigureAwait(false);
                    return BuildResult<T>(text, config);
                }

                throw;
            }
        }

        /// <summary>
        /// 上传文件, 并检查返回结果是否用错误；如遇 access_token 超时错误, 会自动获取新的 access_token 并重试
        /// </summary>
        /// <param name="url">接口网址, 里边的 $acac$ 会被自动替换成 access_token=xxx </param>
        /// <param name="fieldName">文件字段名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileStream">文件内容流</param>
        /// <param name="additionalFields">要在 POST 内容中发送的其它字段</param>
        /// <param name="config"></param>
        /// <exception cref="ApiException">返回的 JSON 字符串中包含了错误信息</exception>
        public static async Task UploadVoid(string url, string fieldName, string fileName, Stream fileStream, KeyValuePair<string, string>[] additionalFields = null, ApiConfig config = null)
        {
            var token = new AsyncOutParameter<string>();
            try
            {
                var text = await UploadStringWithToken(url, fieldName, fileName, fileStream, additionalFields, config, token).ConfigureAwait(false);
                BuildVoid(text, config);
            }
            catch (ApiException ex)
            {
                if (ex.Code == ErrorAccessTokenExpired || ex.Code == ErrorAccessTokenInvalid)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.RefreshTokenInfo(token.Value).ConfigureAwait(false);

                    var text = await UploadString(url, fieldName, fileName, fileStream, additionalFields, config).ConfigureAwait(false);
                    BuildVoid(text, config);
                    return;
                }

                throw;
            }
        }
        #endregion

        private static MultipartFormDataContent BuildUploadContent(string fieldName, string fileName, Stream fileStream, KeyValuePair<string, string>[] additionalFields)
        {
            var boundary = Guid.NewGuid().ToString("n");
            var content = new MultipartFormDataContent(boundary);

            if (fieldName != null)
                fieldName = '"' + fieldName + '"';

            if (fileName != null)
                fileName = '"' + fileName + '"';

            content.Add(new StreamContent(fileStream), fieldName, fileName);

            if (additionalFields != null)
            {
                foreach (var field in additionalFields)
                {
                    if (field.Value != null)
                    {
                        var name = field.Key;
                        if (name != null)
                            name = '"' + name + '"';

                        content.Add(new StringContent(field.Value), name);
                    }
                }
            }

            //微信服务器不接受引号括起来的 boundary..
            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
            return content;
        }

        private static async Task<string> FormatUrl(string url, ApiConfig config, AsyncOutParameter<string> token)
        {
            if (url.IndexOf("$acac$", StringComparison.InvariantCulture) < 0)
                return url;

            var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
            var accessToken = await m.GetToken().ConfigureAwait(false);
            var escapedToken = Uri.EscapeDataString(accessToken);

            token.SetValue(accessToken);

            return url.Replace("$acac$", "access_token=" + escapedToken);
        }

        /// <summary>
        /// 根据接口返回的 JSON 字符串构建对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">JSON 字符串</param>
        /// <param name="config"></param>
        /// <exception cref="ApiException">JSON 字符串中包含了错误信息</exception>
        public static T BuildResult<T>(string json, ApiConfig config = null)
            where T : IApiResult
        {
            var parser = config?.JsonParser ?? _defaultConfig.JsonParser;
            var v = parser.Parse(json);
            if (!v.HasField("errcode") || v.FieldValue<int>("errcode") == ErrorSuccess)
            {
                return parser.Build<T>(v);
            }

            throw new ApiException(v.FieldValue<int>("errcode"), v.FieldValue<string>("errmsg"));
        }

        /// <summary>
        /// 检查 JSON 字符串中是否有错误信息
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <param name="config"></param>
        /// <exception cref="ApiException">JSON 字符串中包含了错误信息</exception>
        public static void BuildVoid(string json, ApiConfig config = null)
        {
            var parser = config?.JsonParser ?? _defaultConfig.JsonParser;
            var v = parser.Parse(json);
            if (!v.HasField("errcode") || v.FieldValue<int>("errcode") == ErrorSuccess)
                return;

            throw new ApiException(v.FieldValue<int>("errcode"), v.FieldValue<string>("errmsg"));
        }

        /// <summary>
        /// 将对象转化为 JSON 字符串
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string ToJsonString(object o, ApiConfig config = null)
        {
            var parser = config?.JsonParser ?? _defaultConfig.JsonParser;
            return parser.ToString(o);
        }
    }
}