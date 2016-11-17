using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信带参数二维码
    /// </summary>
    public static class Qrcode
    {
        /// <summary>
        /// 创建永久二维码
        /// </summary>
        /// <param name="scene_id">场景值ID，临时二维码时为32位非0整型，永久二维码时最大值为100000（目前参数只支持1--100000）</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<CreateResult> CreatePermanent(int scene_id, ApiConfig config = null)
        {
            return ApiHelper.PostResult<CreateResult>("https://api.weixin.qq.com/cgi-bin/qrcode/create?$acac$", new
            {
                action_name = "QR_LIMIT_SCENE",
                action_info = new
                {
                    scene = new
                    {
                        scene_id
                    }
                }
            }, config);
        }

        /// <summary>
        /// 创建永久二维码
        /// </summary>
        /// <param name="scene_str">场景值ID（字符串形式的ID），字符串类型，长度限制为1到64</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<CreateResult> CreatePermanent(string scene_str, ApiConfig config = null)
        {
            return ApiHelper.PostResult<CreateResult>("https://api.weixin.qq.com/cgi-bin/qrcode/create?$acac$", new
            {
                action_name = "QR_LIMIT_SCENE",
                action_info = new
                {
                    scene = new
                    {
                        scene_str
                    }
                }
            }, config);
        }

        /// <summary>
        /// 创建临时二维码
        /// </summary>
        /// <param name="scene_id">场景值ID，临时二维码时为32位非0整型</param>
        /// <param name="expire_seconds">该二维码有效时间，以秒为单位。最大不超过2592000（即30天），此字段如果不填，则默认有效期为30秒。</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<CreateResult> CreateTemporary(int scene_id, int expire_seconds, ApiConfig config = null)
        {
            return ApiHelper.PostResult<CreateResult>("https://api.weixin.qq.com/cgi-bin/qrcode/create?$acac$", new
            {
                expire_seconds,
                action_name = "QR_SCENE",
                action_info = new
                {
                    scene = new
                    {
                        scene_id
                    }
                }
            }, config);
        }

        public class CreateResult : IApiResult
        {
            /// <summary>
            /// 获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。
            /// </summary>
            public string ticket { get; set; }
            /// <summary>
            /// 该二维码有效时间，以秒为单位。 最大不超过2592000（即30天）。
            /// </summary>
            public int expire_seconds { get; set; }
            /// <summary>
            /// 二维码图片解析后的地址，开发者可根据该地址自行生成需要的二维码图片
            /// </summary>
            public string url { get; set; }
        }

        /// <summary>
        /// 通过ticket换取二维码
        /// </summary>
        /// <param name="ticket">获取的二维码ticket</param>
        /// <returns></returns>
        public static Task<byte[]> Show(string ticket)
        {
            var url = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + Uri.EscapeDataString(ticket);
            return new HttpClient().GetByteArrayAsync(url);
        }
    }
}
