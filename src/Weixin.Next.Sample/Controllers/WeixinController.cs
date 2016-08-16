using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using Weixin.Next.Messaging;

namespace Weixin.Next.Sample.Controllers
{
    public class WeixinController : Controller
    {
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(string signature, string timestamp, string nonce, string echostr)
        {
            var token = ConfigurationManager.AppSettings["weixin.token"];
            if (Signature.Check(token, timestamp, nonce, signature))
            {
                return Content(echostr);
            }

            return View("Get");
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<ActionResult> Post(PostUrlParameters p)
        {
            var response = await WeixinConfig.MessageCenter.ProcessMessage(p, Request.InputStream).ConfigureAwait(false);
            return Content(response);
        }
    }
}