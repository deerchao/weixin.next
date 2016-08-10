using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Weixin.Next.Api
{
    public static class ApiHelper
    {
        private static ApiConfig _defaultConfig;


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
        public static async Task<string> GetString(string url, ApiConfig config = null)
        {
            url = await FormatUrl(url, config).ConfigureAwait(false);
            var http = config?.HttpClient ?? _defaultConfig.HttpClient;
            return await http.GetStringAsync(url).ConfigureAwait(false);
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
            try
            {
                var text = await GetString(url, config).ConfigureAwait(false);
                return BuildResult<T>(text, config);
            }
            catch (ApiException ex)
            {
                //access_token超时
                if (ex.Code == 42001)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.GetToken(true).ConfigureAwait(false);

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
            try
            {
                var text = await GetString(url, config).ConfigureAwait(false);
                BuildVoid(text, config);
            }
            catch (ApiException ex)
            {
                //access_token超时
                if (ex.Code == 42001)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.GetToken(true).ConfigureAwait(false);

                    var text = await GetString(url, config).ConfigureAwait(false);
                    BuildVoid(text, config);
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
        public static async Task<string> PostString(string url, object data, ApiConfig config = null)
        {
            url = await FormatUrl(url, config).ConfigureAwait(false);

            var parser = config?.JsonParser ?? _defaultConfig.JsonParser;
            var body = parser.ToString(parser.Generate(data));

            var http = config?.HttpClient ?? _defaultConfig.HttpClient;
            var response = await http.PostAsync(url, new StringContent(body)).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
            try
            {
                var text = await PostString(url, data, config).ConfigureAwait(false);
                return BuildResult<T>(text, config);
            }
            catch (ApiException ex)
            {
                //access_token超时
                if (ex.Code == 42001)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.GetToken(true).ConfigureAwait(false);

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
            try
            {
                var text = await PostString(url, data, config).ConfigureAwait(false);
                BuildVoid(text, config);
            }
            catch (ApiException ex)
            {
                //access_token超时
                if (ex.Code == 42001)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.GetToken(true).ConfigureAwait(false);

                    var text = await PostString(url, data, config).ConfigureAwait(false);
                    BuildVoid(text, config);
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
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<string> UploadString(string url, string fieldName, string fileName, Stream fileStream, ApiConfig config = null)
        {
            var content = BuildUploadContent(fieldName, fileName, fileStream);

            url = await FormatUrl(url, config).ConfigureAwait(false);
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
        /// <param name="config"></param>
        /// <exception cref="ApiException">返回的 JSON 字符串中包含了错误信息</exception>
        public static async Task<T> UploadResult<T>(string url, string fieldName, string fileName, Stream fileStream, ApiConfig config = null)
            where T : IApiResult
        {
            try
            {
                var text = await UploadString(url, fieldName, fileName, fileStream, config).ConfigureAwait(false);
                return BuildResult<T>(text, config);
            }
            catch (ApiException ex)
            {
                //access_token超时
                if (ex.Code == 42001)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.GetToken(true).ConfigureAwait(false);

                    var text = await UploadString(url, fieldName, fileName, fileStream, config).ConfigureAwait(false);
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
        /// <param name="config"></param>
        /// <exception cref="ApiException">返回的 JSON 字符串中包含了错误信息</exception>
        public static async Task UploadVoid(string url, string fieldName, string fileName, Stream fileStream, ApiConfig config = null)
        {
            try
            {
                var text = await UploadString(url, fieldName, fileName, fileStream, config).ConfigureAwait(false);
                BuildVoid(text, config);
            }
            catch (ApiException ex)
            {
                //access_token超时
                if (ex.Code == 42001)
                {
                    var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
                    await m.GetToken(true).ConfigureAwait(false);

                    var text = await UploadString(url, fieldName, fileName, fileStream, config).ConfigureAwait(false);
                    BuildVoid(text, config);
                }

                throw;
            }
        }
        #endregion

        private static MultipartFormDataContent BuildUploadContent(string fieldName, string fileName, Stream fileStream)
        {
            var boundary = Guid.NewGuid().ToString("n");
            var content = new MultipartFormDataContent(boundary);

            if (fieldName != null)
                fieldName = '"' + fieldName + '"';

            if (fileName != null)
                fileName = '"' + fileName + '"';

            content.Add(new StreamContent(fileStream), fieldName, fileName);

            //微信服务器不接受引号括起来的 boundary..
            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
            return content;
        }

        private static async Task<string> FormatUrl(string url, ApiConfig config)
        {
            var m = config?.AccessTokenManager ?? _defaultConfig.AccessTokenManager;
            var accessToken = await m.GetToken().ConfigureAwait(false);
            var escapedToken = Uri.EscapeDataString(accessToken);

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
            if (!v.HasField("errcode") || v.Field("errcode").As<int>() == 0)
            {
                return parser.Build<T>(v);
            }

            throw new ApiException(v.Field("errcode").As<int>(), v.Field("errmsg").As<string>());
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
            if (!v.HasField("errcode") || v.Field("errcode").As<int>() == 0)
                return;

            throw new ApiException(v.Field("errcode").As<int>(), v.Field("errmsg").As<string>());
        }
    }
}