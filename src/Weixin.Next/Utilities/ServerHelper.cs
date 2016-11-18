using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Weixin.Next.Utilities
{
    public static class ServerHelper
    {
        private static string _localIP;

        public static async Task<string> GetLocalIP()
        {
            if (_localIP == null)
            {
                _localIP = (await Dns.GetHostAddressesAsync(Dns.GetHostName()).ConfigureAwait(false))
                     .Where(x => x.AddressFamily == AddressFamily.InterNetwork)
                     .Select(x => x.ToString())
                     .FirstOrDefault();
            }
            return _localIP;
        }

        public static void SetLocalIP(string ipAddress)
        {
            _localIP = ipAddress;
        }
    }
}
