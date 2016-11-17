namespace Weixin.Next.Pay
{
    // ReSharper disable InconsistentNaming
    public enum TradeType
    {
        /// <summary>
        /// 公众号支付: 用户在微信中打开商户的H5页面，商户在H5页面通过调用微信支付提供的JSAPI接口调起微信支付模块完成支付
        /// </summary>
        JSAPI,
        /// <summary>
        /// 原生扫码支付: 商户系统按微信支付协议生成支付二维码，用户再用微信“扫一扫”完成支付的模式。该模式适用于PC网站支付、实体店单品或订单支付、媒体广告支付等场景。 
        /// </summary>
        NATIVE,
        /// <summary>
        /// app支付: 又称移动端支付，是商户通过在移动端应用APP中集成开放SDK调起微信支付模块完成支付的模式。
        /// </summary>
        APP,
        /// <summary>
        /// 刷卡支付(刷卡支付有单独的支付接口，不调用统一下单接口): 用户展示微信钱包内的“刷卡条码/二维码”给商户系统扫描后直接完成支付的模式。主要应用线下面对面收银的场景。
        /// </summary>
        MICROPAY
    }

    public enum PayLimitation
    {
        /// <summary>
        /// 指定不能使用信用卡支付
        /// </summary>
        no_credit,
    }

    /// <summary>
    /// 代金券类型
    /// </summary>
    public enum CouponType
    {
        /// <summary>
        /// 充值代金券 
        /// </summary>
        CASH,
        /// <summary>
        /// 非充值代金券 
        /// </summary>
        NO_CASH,
    }
}
