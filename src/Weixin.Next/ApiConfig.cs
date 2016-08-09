using System.Net.Http;

namespace Weixin.Next
{
    public class ApiConfig
    {
        public IJsonParser JsonParser { get; set; }
        public AccessTokenManager AccessTokenManager { get; set; }
        public HttpClient HttpClient { get; set; }
    }
}
