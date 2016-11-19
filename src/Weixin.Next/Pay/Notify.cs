using System.Xml.Linq;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 支付结果通知
    /// </summary>
    public class Notify
    {
        public class Incoming : OrderQuery.Incoming
        {
        }

        public class Outcoming
        {
            /// <summary>
            /// 返回状态码, SUCCESS/FAIL 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
            /// </summary>
            public string return_code { get; set; }
            /// <summary>
            /// 返回信息 返回信息，如非空，为错误原因 
            /// </summary>
            public string return_msg { get; set; }

            public string Serialize()
            {
                return new XElement("xml",
                        new XElement("return_code", return_code),
                        new XElement("return_msg", return_msg))
                    .ToString();
            }
        }
    }
}
