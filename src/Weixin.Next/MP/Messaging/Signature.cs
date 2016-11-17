using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Weixin.Next.MP.Messaging
{
    public static class Signature
    {
        /// <summary>
        /// 检查消息签名
        /// </summary>
        /// <param name="token">用于生成签名的 Token</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机字符串</param>
        /// <param name="signature">签名</param>
        /// <returns></returns>
        public static bool Check(string token, string timestamp, string nonce, string signature)
        {
            if (string.IsNullOrEmpty(signature) || 
                string.IsNullOrEmpty(nonce) ||
                string.IsNullOrEmpty(timestamp) ||
                string.IsNullOrEmpty(token))
                return false;

            var items = new[] { token, timestamp, nonce }.OrderBy(x => x);
            var text = string.Concat(items);

            byte[] signatureData;
            using (var sha1 = SHA1.Create())
            {
                signatureData = sha1.ComputeHash(Encoding.UTF8.GetBytes(text));
            }

            if (signatureData.Length * 2 != signature.Length)
                return false;

            return !signatureData.Select(t => t.ToString("x2"))
                .Where((s, i) => s[0] != signature[i * 2] || s[1] != signature[i * 2 + 1])
                .Any();
        }
    }
}
