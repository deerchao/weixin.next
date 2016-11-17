namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    public static class JsApi
    {
        #region 分享接口
        /// <summary>
        /// 获取“分享到朋友圈”按钮点击状态及自定义分享内容接口
        /// </summary>
        public const string onMenuShareTimeline = "onMenuShareTimeline";
        /// <summary>
        /// 获取“分享给朋友”按钮点击状态及自定义分享内容接口
        /// </summary>
        public const string onMenuShareAppMessage = "onMenuShareAppMessage";
        /// <summary>
        /// 获取“分享到QQ”按钮点击状态及自定义分享内容接口
        /// </summary>
        public const string onMenuShareQQ = "onMenuShareQQ";
        /// <summary>
        /// 获取“分享到腾讯微博”按钮点击状态及自定义分享内容接口
        /// </summary>
        public const string onMenuShareWeibo = "onMenuShareWeibo";
        /// <summary>
        /// 获取“分享到QQ空间”按钮点击状态及自定义分享内容接口
        /// </summary>
        public const string onMenuShareQZone = "onMenuShareQZone";

        #endregion

        #region 图像接口
        /// <summary>
        /// 拍照或从手机相册中选图接口
        /// </summary>
        public const string chooseImage = "chooseImage";
        /// <summary>
        /// 预览图片接口
        /// </summary>
        public const string previewImage = "previewImage";
        /// <summary>
        /// 上传图片接口
        /// </summary>
        public const string uploadImage = "uploadImage";
        /// <summary>
        /// 下载图片接口
        /// </summary>
        public const string downloadImage = "downloadImage";

        #endregion

        #region 音频接口
        /// <summary>
        /// 开始录音接口
        /// </summary>
        public const string startRecord = "startRecord";
        /// <summary>
        /// 停止录音接口
        /// </summary>
        public const string stopRecord = "stopRecord";
        /// <summary>
        /// 监听录音自动停止接口
        /// </summary>
        public const string onVoiceRecordEnd = "onVoiceRecordEnd";
        /// <summary>
        /// 播放语音接口
        /// </summary>
        public const string playVoice = "playVoice";
        /// <summary>
        /// 暂停播放接口
        /// </summary>
        public const string pauseVoice = "pauseVoice";
        /// <summary>
        /// 停止播放接口
        /// </summary>
        public const string stopVoice = "stopVoice";
        /// <summary>
        /// 监听语音播放完毕接口
        /// </summary>
        public const string onVoicePlayEnd = "onVoicePlayEnd";
        /// <summary>
        /// 上传语音接口
        /// </summary>
        public const string uploadVoice = "uploadVoice";
        /// <summary>
        /// 下载语音接口
        /// </summary>
        public const string downloadVoice = "downloadVoice";

        #endregion

        #region 智能接口
        /// <summary>
        /// 识别音频并返回识别结果接口
        /// </summary>
        public const string translateVoice = "translateVoice";
        #endregion

        #region 设备信息
        /// <summary>
        /// 获取网络状态接口
        /// </summary>
        public const string getNetworkType = "getNetworkType";
        #endregion

        #region 地理位置
        /// <summary>
        /// 使用微信内置地图查看位置接口
        /// </summary>
        public const string openLocation = "openLocation";
        /// <summary>
        /// 获取地理位置接口
        /// </summary>
        public const string getLocation = "getLocation";
        #endregion

        #region 摇一摇周边
        /// <summary>
        /// 开启查找周边ibeacon设备接口
        /// </summary>
        public const string startSearchBeacons = "startSearchBeacons";
        /// <summary>
        /// 关闭查找周边ibeacon设备接口
        /// </summary>
        public const string stopSearchBeacons = "stopSearchBeacons";
        /// <summary>
        /// 监听周边ibeacon设备接口
        /// </summary>
        public const string onSearchBeacons = "onSearchBeacons";
        #endregion

        #region 界面操作
        /// <summary>
        /// 隐藏右上角菜单接口
        /// </summary>
        public const string hideOptionMenu = "hideOptionMenu";
        /// <summary>
        /// 显示右上角菜单接口
        /// </summary>
        public const string showOptionMenu = "showOptionMenu";
        /// <summary>
        /// 关闭当前网页窗口接口
        /// </summary>
        public const string closeWindow = "closeWindow";
        /// <summary>
        /// 批量隐藏功能按钮接口
        /// </summary>
        public const string hideMenuItems = "hideMenuItems";
        /// <summary>
        /// 批量显示功能按钮接口
        /// </summary>
        public const string showMenuItems = "showMenuItems";
        /// <summary>
        /// 隐藏所有非基础按钮接口
        /// </summary>
        public const string hideAllNonBaseMenuItem = "hideAllNonBaseMenuItem";
        /// <summary>
        /// 显示所有功能按钮接口
        /// </summary>
        public const string showAllNonBaseMenuItem = "showAllNonBaseMenuItem";
        #endregion

        #region 微信扫一扫
        /// <summary>
        /// 调起微信扫一扫接口
        /// </summary>
        public const string scanQRCode = "scanQRCode";
        #endregion

        #region 微信小店
        /// <summary>
        /// 跳转微信商品页接口
        /// </summary>
        public const string openProductSpecificView = "openProductSpecificView";
        #endregion

        #region 微信卡券
        /// <summary>
        /// 拉取适用卡券列表并获取用户选择信息
        /// </summary>
        public const string chooseCard = "chooseCard";
        /// <summary>
        /// 批量添加卡券接口
        /// </summary>
        public const string addCard = "addCard";
        /// <summary>
        /// 查看微信卡包中的卡券接口
        /// </summary>
        public const string openCard = "openCard";
        #endregion

        #region 微信支付
        /// <summary>
        /// 发起一个微信支付请求
        /// </summary>
        public const string chooseWXPay = "chooseWXPay";
        #endregion
    }
}
