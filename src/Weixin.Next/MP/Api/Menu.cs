using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 微信自定义菜单
    /// </summary>
    public static class Menu
    {
        #region 自定义菜单创建接口

        /// <summary>
        /// 自定义菜单创建接口
        /// </summary>
        /// <param name="button">一级菜单项, 1~3 个</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task Create(Button[] button, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/menu/create?$acac$", new { button }, config);
        }
        #endregion

        #region 自定义菜单查询接口

        /// <summary>
        /// 自定义菜单查询接口
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetMenuResult> Get(ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetMenuResult>("https://api.weixin.qq.com/cgi-bin/menu/get?$acac$", config);
        }

        public class GetMenuResult : IApiResult
        {
            public BasicMenu menu { get; set; }
            /// <summary>
            /// 只有使用了个性化菜单时才会有此属性
            /// </summary>
            public ConditionalMenu[] conditionalmenu { get; set; }
        }

        public class ConditionalMenu : BasicMenu
        {
            public MatchRule matchrule { get; set; }
        }

        public class BasicMenu
        {
            public Button[] button { get; set; }
            /// <summary>
            /// 只有使用了个性化菜单时才会有此属性
            /// </summary>
            public long menuid { get; set; }
        }
        #endregion

        #region 自定义菜单删除接口

        /// <summary>
        /// <para>自定义菜单删除接口</para>
        /// <para>注意，在个性化菜单时，调用此接口会删除默认菜单及全部个性化菜单。</para>
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task Delete(ApiConfig config = null)
        {
            return ApiHelper.GetVoid("https://api.weixin.qq.com/cgi-bin/menu/delete?$acac$", config);
        }
        #endregion


        #region 创建个性化菜单

        /// <summary>
        /// 创建个性化菜单
        /// </summary>
        /// <param name="button">一级菜单项, 1~3个</param>
        /// <param name="matchrule">匹配规则</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<AddConditionalResult> AddConditional(Button[] button, MatchRule matchrule, ApiConfig config = null)
        {
            return ApiHelper.PostResult<AddConditionalResult>("https://api.weixin.qq.com/cgi-bin/menu/addconditional?$acac$", new { button, matchrule }, config);
        }

        public class AddConditionalResult : IApiResult
        {
            /// <summary>
            /// 菜单id
            /// </summary>
            public string menuid { get; set; }
        }
        #endregion

        #region 删除个性化菜单

        /// <summary>
        /// 删除个性化菜单
        /// </summary>
        /// <param name="menuid">菜单id</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task DeleteConditional(string menuid, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/menu/delconditional?$acac$", new { menuid }, config);
        }
        #endregion

        #region 测试个性化菜单匹配结果
        /// <summary>
        /// 测试用户与个性化菜单的匹配结果
        /// </summary>
        /// <param name="user_id">粉丝的OpenID，或粉丝的微信号</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<TryMatchResult> TryMatch(string user_id, ApiConfig config)
        {
            return ApiHelper.PostResult<TryMatchResult>("https://api.weixin.qq.com/cgi-bin/menu/trymatch?$acac$", new { user_id }, config);
        }

        public class TryMatchResult : IApiResult
        {
            public Button[] button { get; set; }
        }
        #endregion

        public class Button
        {
            /// <summary>
            /// <para>菜单的响应动作类型</para>
            /// <para>最多有3个1级菜单项, 每个1级菜单最多有5个2级子菜单项. 非叶项只应设置 name 和 sub_button 属性; 叶子项不应设置 sub_button 属性, 其它属性按 type 所需进行设置.</para>
            /// <para>1、click：点击推事件用户点击click类型按钮后，微信服务器会通过消息接口推送消息类型为event的结构给开发者（参考消息接口指南），并且带上按钮中开发者填写的key值，开发者可以通过自定义的key值与用户进行交互；<br />
            /// 2、view：跳转URL用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。<br />
            /// 3、scancode_push：扫码推事件用户点击按钮后，微信客户端将调起扫一扫工具，完成扫码操作后显示扫描结果（如果是URL，将进入URL），且会将扫码的结果传给开发者，开发者可以下发消息。<br />
            /// 4、scancode_waitmsg：扫码推事件且弹出“消息接收中”提示框用户点击按钮后，微信客户端将调起扫一扫工具，完成扫码操作后，将扫码的结果传给开发者，同时收起扫一扫工具，然后弹出“消息接收中”提示框，随后可能会收到开发者下发的消息。<br />
            /// 5、pic_sysphoto：弹出系统拍照发图用户点击按钮后，微信客户端将调起系统相机，完成拍照操作后，会将拍摄的相片发送给开发者，并推送事件给开发者，同时收起系统相机，随后可能会收到开发者下发的消息。<br />
            /// 6、pic_photo_or_album：弹出拍照或者相册发图用户点击按钮后，微信客户端将弹出选择器供用户选择“拍照”或者“从手机相册选择”。用户选择后即走其他两种流程。<br />
            /// 7、pic_weixin：弹出微信相册发图器用户点击按钮后，微信客户端将调起微信相册，完成选择操作后，将选择的相片发送给开发者的服务器，并推送事件给开发者，同时收起相册，随后可能会收到开发者下发的消息。<br />
            /// 8、location_select：弹出地理位置选择器用户点击按钮后，微信客户端将调起地理位置选择工具，完成选择操作后，将选择的地理位置发送给开发者的服务器，同时收起位置选择工具，随后可能会收到开发者下发的消息。<br />
            /// 9、media_id：下发消息（除文本消息）用户点击media_id类型按钮后，微信服务器会将开发者填写的永久素材id对应的素材下发给用户，永久素材类型可以是图片、音频、视频、图文消息。请注意：永久素材id必须是在“素材管理/新增永久素材”接口上传后获得的合法id。<br />
            /// 10、view_limited：跳转图文消息URL用户点击view_limited类型按钮后，微信客户端将打开开发者在按钮中填写的永久素材id对应的图文消息URL，永久素材类型只支持图文消息。请注意：永久素材id必须是在“素材管理/新增永久素材”接口上传后获得的合法id。<br />
            /// </para>
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 菜单标题，不超过16个字节，子菜单不超过60个字节
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 菜单KEY值，用于消息接口推送，不超过128字节, click等点击类型必须
            /// </summary>
            public string key { get; set; }
            /// <summary>
            /// 网页链接，用户点击菜单可打开链接，不超过1024字节, view类型必须
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// 调用新增永久素材接口返回的合法media_id, media_id类型和view_limited类型必须
            /// </summary>
            public string media_id { get; set; }
            /// <summary>
            /// 二级菜单数组，个数应为1~5个
            /// </summary>
            public Button[] sub_button { get; set; }
        }

        public class MatchRule
        {
            /// <summary>
            /// 用户标签的id，可通过用户标签管理接口获取
            /// </summary>
            public string tag_id { get; set; }
            /// <summary>
            /// 用户分组id，可通过用户分组管理接口获取
            /// </summary>
            public string group_id { get; set; }
            /// <summary>
            /// 性别：男（1）女（2），不填则不做匹配
            /// </summary>
            public string sex { get; set; }
            /// <summary>
            /// 客户端版本，当前只具体到系统型号：IOS(1), Android(2),Others(3)，不填则不做匹配
            /// </summary>
            public string client_platform_type { get; set; }
            /// <summary>
            /// 国家信息，是用户在微信中设置的地区，具体请参考地区信息表
            /// </summary>
            public string country { get; set; }
            /// <summary>
            /// 省份信息，是用户在微信中设置的地区，具体请参考地区信息表
            /// </summary>
            public string province { get; set; }
            /// <summary>
            /// 城市信息，是用户在微信中设置的地区，具体请参考地区信息表
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 语言信息，是用户在微信中设置的语言，具体请参考语言表：
            /// 1、简体中文 "zh_CN" 2、繁体中文TW "zh_TW" 3、繁体中文HK "zh_HK" 4、英文 "en" 5、印尼 "id" 6、马来 "ms" 7、西班牙 "es" 8、韩国 "ko" 9、意大利 "it" 10、日本 "ja" 11、波兰 "pl" 12、葡萄牙 "pt" 13、俄国 "ru" 14、泰文 "th" 15、越南 "vi" 16、阿拉伯语 "ar" 17、北印度 "hi" 18、希伯来 "he" 19、土耳其 "tr" 20、德语 "de" 21、法语 "fr"
            /// </summary>
            public string language { get; set; }
        }

        #region 获取自定义菜单配置接口
        /// <summary>
        /// 获取自定义菜单配置接口
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetCurrentSelfmenuInfoResult> GetCurrentSelfmenuInfo(ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetCurrentSelfmenuInfoResult>("https://api.weixin.qq.com/cgi-bin/get_current_selfmenu_info?$acac$", config);
        }

        public class GetCurrentSelfmenuInfoResult : IApiResult
        {
            /// <summary>
            /// 菜单是否开启，0代表未开启，1代表开启
            /// </summary>
            public int is_menu_open { get; set; }
            /// <summary>
            /// 菜单信息
            /// </summary>
            public Selfmenu_Info selfmenu_info { get; set; }
        }

        public class Selfmenu_Info
        {
            /// <summary>
            /// 菜单按钮
            /// </summary>
            public Selfmenu_Button[] button { get; set; }
        }

        public class Selfmenu_Button : Selfmenu_BasicButtion
        {
            public Sub_Button sub_button { get; set; }
        }

        public class Sub_Button
        {
            public Selfmenu_BasicButtion[] list { get; set; }
        }

        public class Selfmenu_BasicButtion
        {
            /// <summary>
            /// 菜单的类型，公众平台官网上能够设置的菜单类型有view（跳转网页）、text（返回文本，下同）、img、photo、video、voice。使用API设置的则有8种，详见《自定义菜单创建接口》
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 菜单名称
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 对于不同的菜单类型，value的值意义不同。官网上设置的自定义菜单：
            /// Text:保存文字到value； Img、voice：保存mediaID到value； Video：保存视频下载链接到value； News：保存图文消息到news_info，同时保存mediaID到value； View：保存链接到url。
            /// 使用API设置的自定义菜单： click、scancode_push、scancode_waitmsg、pic_sysphoto、pic_photo_or_album、 pic_weixin、location_select：保存值到key；view：保存链接到url
            /// </summary>
            public string key { get; set; }
            /// <summary>
            /// 对于不同的菜单类型，value的值意义不同。官网上设置的自定义菜单：
            /// Text:保存文字到value； Img、voice：保存mediaID到value； Video：保存视频下载链接到value； News：保存图文消息到news_info，同时保存mediaID到value； View：保存链接到url。
            /// 使用API设置的自定义菜单： click、scancode_push、scancode_waitmsg、pic_sysphoto、pic_photo_or_album、 pic_weixin、location_select：保存值到key；view：保存链接到url
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// 对于不同的菜单类型，value的值意义不同。官网上设置的自定义菜单：
            /// Text:保存文字到value； Img、voice：保存mediaID到value； Video：保存视频下载链接到value； News：保存图文消息到news_info，同时保存mediaID到value； View：保存链接到url。
            /// 使用API设置的自定义菜单： click、scancode_push、scancode_waitmsg、pic_sysphoto、pic_photo_or_album、 pic_weixin、location_select：保存值到key；view：保存链接到url
            /// </summary>
            public string value { get; set; }
            /// <summary>
            /// 图文消息的信息, 只有 type 为 news 时才有
            /// </summary>
            public News_Info news_info { get; set; }
        }

        public class News_Info
        {
            public News_Info_Item[] list { get; set; }
        }

        public class News_Info_Item
        {
            /// <summary>
            /// 图文消息的标题
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 作者
            /// </summary>
            public string author { get; set; }
            /// <summary>
            /// 摘要
            /// </summary>
            public string digest { get; set; }
            /// <summary>
            /// 是否显示封面，0为不显示，1为显示
            /// </summary>
            public int show_cover { get; set; }
            /// <summary>
            /// 封面图片的URL
            /// </summary>
            public string cover_url { get; set; }
            /// <summary>
            /// 正文的URL
            /// </summary>
            public string content_url { get; set; }
            /// <summary>
            /// 原文的URL，若置空则无查看原文入口
            /// </summary>
            public string source_url { get; set; }
        }
        #endregion
    }
}
