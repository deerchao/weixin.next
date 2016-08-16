using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Weixin.Next.Sample.Models
{
    /// <summary>
    /// Provides full http logging
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        private readonly Action<string> _log;
        private readonly Func<bool> _shouldLog;

        public LoggingHandler(Action<string> log, Func<bool> shouldLog, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _log = log;
            _shouldLog = shouldLog;
        }

        private bool ShouldLog { get { return _log != null && _shouldLog != null && _shouldLog(); } }

        private void Log(string message)
        {
            _log?.Invoke(message);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (ShouldLog)
            {
                var log = $"{request.Method} {request.RequestUri}\r\n";
                log += request.Headers.ToString();
                if (request.Content != null)
                {
                    log += request.Content.Headers.ToString();
                    log += await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                Log(log);
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (ShouldLog)
            {
                var log = $"{response.StatusCode:D} {response.ReasonPhrase}\r\n";
                log += response.Headers.ToString();
                if (response.Content != null)
                {
                    log += response.Content.Headers.ToString();
                    log += await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                Log(log);
            }

            return response;
        }
    }
}