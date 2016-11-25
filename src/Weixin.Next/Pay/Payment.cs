using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信支付数据接口的主要入口
    /// </summary>
    public class Payment
    {
        private readonly Requester _requester;
        private readonly bool _checkSignature;
        private readonly bool _generateReport;

        /// <summary>
        /// 创建微信支付接口主入口对象
        /// </summary>
        /// <param name="appid">微信 appid</param>
        /// <param name="mch_id">微信支付商户 id</param>
        /// <param name="key">签名 key</param>
        /// <param name="cert">微信支付安全证书</param>
        /// <param name="checkSignature">发送请求时是否检查服务器返回数据的签名</param>
        /// <param name="generateReport">发送请求时是否生成报告, 准备发送</param>
        /// <param name="jsonParser">用于序列化/反序列化下单接口中的 detail 字段</param>
        public Payment(string appid, string mch_id, string key, X509Certificate2 cert, bool checkSignature, bool generateReport, IJsonParser jsonParser)
            : this(new Requester(appid, mch_id, key, cert, jsonParser), checkSignature, generateReport)
        {
        }

        /// <summary>
        /// 创建微信支付接口主入口对象
        /// </summary>
        /// <param name="requester">处理与服务器通信的组件</param>
        /// <param name="checkSignature">发送请求时是否检查服务器返回数据的签名</param>
        /// <param name="generateReport">发送请求时是否生成报告, 准备发送</param>
        public Payment(Requester requester, bool checkSignature, bool generateReport)
        {
            _requester = requester;
            _checkSignature = checkSignature;
            _generateReport = generateReport;
        }

        /// <summary>
        /// 统一下单
        /// </summary>
        /// <returns></returns>
        public UnifiedOrder UnifiedOrder()
        {
            return new UnifiedOrder(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <returns></returns>
        public OrderQuery OrderQuery()
        {
            return new OrderQuery(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <returns></returns>
        public CloseOrder CloseOrder()
        {
            return new CloseOrder(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 申请退款
        /// </summary>
        /// <returns></returns>
        public Refund Refund()
        {
            return new Refund(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 查询退款
        /// </summary>
        /// <returns></returns>
        public RefundQuery RefundQuery()
        {
            return new RefundQuery(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 下载对账单
        /// </summary>
        /// <returns></returns>
        public DownloadBill DownloadBill()
        {
            return new DownloadBill(_requester, _checkSignature);
        }

        /// <summary>
        /// 支付保障
        /// </summary>
        /// <returns></returns>
        public Report Report()
        {
            return new Report(_requester);
        }

        /// <summary>
        /// 根据通知数据生成通知参数
        /// </summary>
        /// <param name="incomingBody">微信发过来的支付通知请求消息体文本</param>
        /// <returns></returns>
        public Notify.Incoming ParseNotify(string incomingBody)
        {
            return _requester.ParseResponse<Notify.Incoming, OrderQuery.ErrorCode>(incomingBody, true);
        }

        /// <summary>
        /// 根据通知数据生成通知参数
        /// </summary>
        /// <param name="incomingStream">微信发过来的支付通知请求消息体流</param>
        /// <returns></returns>
        public async Task<Notify.Incoming> ParseNotify(Stream incomingStream)
        {
            using (var reader = new StreamReader(incomingStream, Encoding.UTF8))
            {
                var incomingBody = await reader.ReadToEndAsync().ConfigureAwait(false);
                return ParseNotify(incomingBody);
            }
        }

        /// <summary>
        /// 生成短网址
        /// </summary>
        /// <returns></returns>
        public ShortUrl ShortUrl()
        {
            return new ShortUrl(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// (刷卡支付)授权码查询 openid
        /// </summary>
        /// <returns></returns>
        public AuthCodeToOpenId AuthCodeToOpenId()
        {
            return new AuthCodeToOpenId(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 刷卡支付
        /// </summary>
        /// <returns></returns>
        public MicroPay MicroPay()
        {
            return new MicroPay(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 企业付款
        /// </summary>
        /// <returns></returns>
        public Transfer Transfer()
        {
            return new Transfer(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 查询企业付款
        /// </summary>
        /// <returns></returns>
        public GetTransferInfo GetTransferInfo()
        {
            return new GetTransferInfo(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 发放代金券
        /// </summary>
        public SendCoupon SendCoupon()
        {
            return new SendCoupon(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 查询代金券批次信息
        /// </summary>
        public QueryCouponStock QueryCouponStock()
        {
            return new QueryCouponStock(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 查询代金券信息
        /// </summary>
        public QueryCouponsInfo QueryCouponsInfo()
        {
            return new QueryCouponsInfo(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 发放普通红包
        /// </summary>
        public SendRedPack SendRedPack()
        {
            return new SendRedPack(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 发放裂变红包
        /// </summary>
        /// <returns></returns>
        public SendGroupRedPack SendGroupRedPack()
        {
            return new SendGroupRedPack(_requester, _checkSignature, _generateReport);
        }

        /// <summary>
        /// 查询红包记录
        /// </summary>
        public GetHBInfo GetHBInfo()
        {
            return new GetHBInfo(_requester, _checkSignature, _generateReport);
        }
    }
}
