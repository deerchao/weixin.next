namespace Weixin.Next.MP.Api
{
    /// <summary>
    /// 微信接口返回结果中的 errcode 值
    /// </summary>
    public static class ApiErrorCode
    {
        /// <summary>
        /// 系统繁忙，此时请开发者稍候再试
        /// </summary>
        public const int E_1 = -1;
        /// <summary>
        /// 请求成功
        /// </summary>
        public const int E0 = 0;
        /// <summary>
        /// 获取access_token时AppSecret错误，或者access_token无效。请开发者认真比对AppSecret的正确性，或查看是否正在为恰当的公众号调用接口
        /// </summary>
        public const int E40001 = 40001;
        /// <summary>
        /// 不合法的凭证类型
        /// </summary>
        public const int E40002 = 40002;
        /// <summary>
        /// 不合法的OpenID，请开发者确认OpenID（该用户）是否已关注公众号，或是否是其他公众号的OpenID
        /// </summary>
        public const int E40003 = 40003;
        /// <summary>
        /// 不合法的媒体文件类型
        /// </summary>
        public const int E40004 = 40004;
        /// <summary>
        /// 不合法的文件类型
        /// </summary>
        public const int E40005 = 40005;
        /// <summary>
        /// 不合法的文件大小
        /// </summary>
        public const int E40006 = 40006;
        /// <summary>
        /// 不合法的媒体文件id
        /// </summary>
        public const int E40007 = 40007;
        /// <summary>
        /// 不合法的消息类型
        /// </summary>
        public const int E40008 = 40008;
        /// <summary>
        /// 不合法的图片文件大小
        /// </summary>
        public const int E40009 = 40009;
        /// <summary>
        /// 不合法的语音文件大小
        /// </summary>
        public const int E40010 = 40010;
        /// <summary>
        /// 不合法的视频文件大小
        /// </summary>
        public const int E40011 = 40011;
        /// <summary>
        /// 不合法的缩略图文件大小
        /// </summary>
        public const int E40012 = 40012;
        /// <summary>
        /// 不合法的AppID，请开发者检查AppID的正确性，避免异常字符，注意大小写
        /// </summary>
        public const int E40013 = 40013;
        /// <summary>
        /// 不合法的access_token，请开发者认真比对access_token的有效性（如是否过期），或查看是否正在为恰当的公众号调用接口
        /// </summary>
        public const int E40014 = 40014;
        /// <summary>
        /// 不合法的菜单类型
        /// </summary>
        public const int E40015 = 40015;
        /// <summary>
        /// 不合法的按钮个数
        /// </summary>
        public const int E40016 = 40016;
        /// <summary>
        /// 不合法的按钮个数
        /// </summary>
        public const int E40017 = 40017;
        /// <summary>
        /// 不合法的按钮名字长度
        /// </summary>
        public const int E40018 = 40018;
        /// <summary>
        /// 不合法的按钮KEY长度
        /// </summary>
        public const int E40019 = 40019;
        /// <summary>
        /// 不合法的按钮URL长度
        /// </summary>
        public const int E40020 = 40020;
        /// <summary>
        /// 不合法的菜单版本号
        /// </summary>
        public const int E40021 = 40021;
        /// <summary>
        /// 不合法的子菜单级数
        /// </summary>
        public const int E40022 = 40022;
        /// <summary>
        /// 不合法的子菜单按钮个数
        /// </summary>
        public const int E40023 = 40023;
        /// <summary>
        /// 不合法的子菜单按钮类型
        /// </summary>
        public const int E40024 = 40024;
        /// <summary>
        /// 不合法的子菜单按钮名字长度
        /// </summary>
        public const int E40025 = 40025;
        /// <summary>
        /// 不合法的子菜单按钮KEY长度
        /// </summary>
        public const int E40026 = 40026;
        /// <summary>
        /// 不合法的子菜单按钮URL长度
        /// </summary>
        public const int E40027 = 40027;
        /// <summary>
        /// 不合法的自定义菜单使用用户
        /// </summary>
        public const int E40028 = 40028;
        /// <summary>
        /// 不合法的oauth_code
        /// </summary>
        public const int E40029 = 40029;
        /// <summary>
        /// 不合法的refresh_token
        /// </summary>
        public const int E40030 = 40030;
        /// <summary>
        /// 不合法的openid列表
        /// </summary>
        public const int E40031 = 40031;
        /// <summary>
        /// 不合法的openid列表长度
        /// </summary>
        public const int E40032 = 40032;
        /// <summary>
        /// 不合法的请求字符，不能包含\uxxxx格式的字符
        /// </summary>
        public const int E40033 = 40033;
        /// <summary>
        /// 不合法的参数
        /// </summary>
        public const int E40035 = 40035;
        /// <summary>
        /// 不合法的请求格式
        /// </summary>
        public const int E40038 = 40038;
        /// <summary>
        /// 不合法的URL长度
        /// </summary>
        public const int E40039 = 40039;
        /// <summary>
        /// 不合法的分组id
        /// </summary>
        public const int E40050 = 40050;
        /// <summary>
        /// 分组名字不合法
        /// </summary>
        public const int E40051 = 40051;
        /// <summary>
        /// 分组名字不合法
        /// </summary>
        public const int E40117 = 40117;
        /// <summary>
        /// media_id大小不合法
        /// </summary>
        public const int E40118 = 40118;
        /// <summary>
        /// button类型错误
        /// </summary>
        public const int E40119 = 40119;
        /// <summary>
        /// button类型错误
        /// </summary>
        public const int E40120 = 40120;
        /// <summary>
        /// 不合法的media_id类型
        /// </summary>
        public const int E40121 = 40121;
        /// <summary>
        /// 微信号不合法
        /// </summary>
        public const int E40132 = 40132;
        /// <summary>
        /// 不支持的图片格式
        /// </summary>
        public const int E40137 = 40137;
        /// <summary>
        /// 请勿添加其他公众号的主页链接
        /// </summary>
        public const int E40155 = 40155;
        /// <summary>
        /// 缺少access_token参数
        /// </summary>
        public const int E41001 = 41001;
        /// <summary>
        /// 缺少appid参数
        /// </summary>
        public const int E41002 = 41002;
        /// <summary>
        /// 缺少refresh_token参数
        /// </summary>
        public const int E41003 = 41003;
        /// <summary>
        /// 缺少secret参数
        /// </summary>
        public const int E41004 = 41004;
        /// <summary>
        /// 缺少多媒体文件数据
        /// </summary>
        public const int E41005 = 41005;
        /// <summary>
        /// 缺少media_id参数
        /// </summary>
        public const int E41006 = 41006;
        /// <summary>
        /// 缺少子菜单数据
        /// </summary>
        public const int E41007 = 41007;
        /// <summary>
        /// 缺少oauth code
        /// </summary>
        public const int E41008 = 41008;
        /// <summary>
        /// 缺少openid
        /// </summary>
        public const int E41009 = 41009;
        /// <summary>
        /// access_token超时，请检查access_token的有效期，请参考基础支持-获取access_token中，对access_token的详细机制说明
        /// </summary>
        public const int E42001 = 42001;
        /// <summary>
        /// refresh_token超时
        /// </summary>
        public const int E42002 = 42002;
        /// <summary>
        /// oauth_code超时
        /// </summary>
        public const int E42003 = 42003;
        /// <summary>
        /// 用户修改微信密码，accesstoken和refreshtoken失效，需要重新授权
        /// </summary>
        public const int E42007 = 42007;
        /// <summary>
        /// 需要GET请求
        /// </summary>
        public const int E43001 = 43001;
        /// <summary>
        /// 需要POST请求
        /// </summary>
        public const int E43002 = 43002;
        /// <summary>
        /// 需要HTTPS请求
        /// </summary>
        public const int E43003 = 43003;
        /// <summary>
        /// 需要接收者关注
        /// </summary>
        public const int E43004 = 43004;
        /// <summary>
        /// 需要好友关系
        /// </summary>
        public const int E43005 = 43005;
        /// <summary>
        /// 需要将接收者从黑名单中移除
        /// </summary>
        public const int E43019 = 43019;
        /// <summary>
        /// 多媒体文件为空
        /// </summary>
        public const int E44001 = 44001;
        /// <summary>
        /// POST的数据包为空
        /// </summary>
        public const int E44002 = 44002;
        /// <summary>
        /// 图文消息内容为空
        /// </summary>
        public const int E44003 = 44003;
        /// <summary>
        /// 文本消息内容为空
        /// </summary>
        public const int E44004 = 44004;
        /// <summary>
        /// 多媒体文件大小超过限制
        /// </summary>
        public const int E45001 = 45001;
        /// <summary>
        /// 消息内容超过限制
        /// </summary>
        public const int E45002 = 45002;
        /// <summary>
        /// 标题字段超过限制
        /// </summary>
        public const int E45003 = 45003;
        /// <summary>
        /// 描述字段超过限制
        /// </summary>
        public const int E45004 = 45004;
        /// <summary>
        /// 链接字段超过限制
        /// </summary>
        public const int E45005 = 45005;
        /// <summary>
        /// 图片链接字段超过限制
        /// </summary>
        public const int E45006 = 45006;
        /// <summary>
        /// 语音播放时间超过限制
        /// </summary>
        public const int E45007 = 45007;
        /// <summary>
        /// 图文消息超过限制
        /// </summary>
        public const int E45008 = 45008;
        /// <summary>
        /// 接口调用超过限制
        /// </summary>
        public const int E45009 = 45009;
        /// <summary>
        /// 创建菜单个数超过限制
        /// </summary>
        public const int E45010 = 45010;
        /// <summary>
        /// API调用太频繁，请稍候再试
        /// </summary>
        public const int E45011 = 45011;
        /// <summary>
        /// 回复时间超过限制
        /// </summary>
        public const int E45015 = 45015;
        /// <summary>
        /// 系统分组，不允许修改
        /// </summary>
        public const int E45016 = 45016;
        /// <summary>
        /// 分组名字过长
        /// </summary>
        public const int E45017 = 45017;
        /// <summary>
        /// 分组数量超过上限
        /// </summary>
        public const int E45018 = 45018;
        /// <summary>
        /// 客服接口下行条数超过上限
        /// </summary>
        public const int E45047 = 45047;
        /// <summary>
        /// 不存在媒体数据
        /// </summary>
        public const int E46001 = 46001;
        /// <summary>
        /// 不存在的菜单版本
        /// </summary>
        public const int E46002 = 46002;
        /// <summary>
        /// 不存在的菜单数据
        /// </summary>
        public const int E46003 = 46003;
        /// <summary>
        /// 不存在的用户
        /// </summary>
        public const int E46004 = 46004;
        /// <summary>
        /// 解析JSON/XML内容错误
        /// </summary>
        public const int E47001 = 47001;
        /// <summary>
        /// api功能未授权，请确认公众号已获得该接口，可以在公众平台官网-开发者中心页中查看接口权限
        /// </summary>
        public const int E48001 = 48001;
        /// <summary>
        /// 粉丝拒收消息（粉丝在公众号选项中，关闭了“接收消息”）
        /// </summary>
        public const int E48002 = 48002;
        /// <summary>
        /// api接口被封禁，请登录mp.weixin.qq.com查看详情
        /// </summary>
        public const int E48004 = 48004;
        /// <summary>
        /// api禁止删除被自动回复和自定义菜单引用的素材
        /// </summary>
        public const int E48005 = 48005;
        /// <summary>
        /// api禁止清零调用次数，因为清零次数达到上限
        /// </summary>
        public const int E48006 = 48006;
        /// <summary>
        /// 用户未授权该api
        /// </summary>
        public const int E50001 = 50001;
        /// <summary>
        /// 用户受限，可能是违规后接口被封禁
        /// </summary>
        public const int E50002 = 50002;
        /// <summary>
        /// 参数错误(invalid parameter)
        /// </summary>
        public const int E61451 = 61451;
        /// <summary>
        /// 无效客服账号(invalid kf_account)
        /// </summary>
        public const int E61452 = 61452;
        /// <summary>
        /// 客服帐号已存在(kf_account exsited)
        /// </summary>
        public const int E61453 = 61453;
        /// <summary>
        /// 客服帐号名长度超过限制(仅允许10个英文字符，不包括@及@后的公众号的微信号)(invalid   kf_acount length)
        /// </summary>
        public const int E61454 = 61454;
        /// <summary>
        /// 客服帐号名包含非法字符(仅允许英文+数字)(illegal character in     kf_account)
        /// </summary>
        public const int E61455 = 61455;
        /// <summary>
        /// 客服帐号个数超过限制(10个客服账号)(kf_account count exceeded)
        /// </summary>
        public const int E61456 = 61456;
        /// <summary>
        /// 无效头像文件类型(invalid   file type)
        /// </summary>
        public const int E61457 = 61457;
        /// <summary>
        /// 系统错误(system error)
        /// </summary>
        public const int E61450 = 61450;
        /// <summary>
        /// 日期格式错误
        /// </summary>
        public const int E61500 = 61500;
        /// <summary>
        /// 不存在此menuid对应的个性化菜单
        /// </summary>
        public const int E65301 = 65301;
        /// <summary>
        /// 没有相应的用户
        /// </summary>
        public const int E65302 = 65302;
        /// <summary>
        /// 没有默认菜单，不能创建个性化菜单
        /// </summary>
        public const int E65303 = 65303;
        /// <summary>
        /// MatchRule信息为空
        /// </summary>
        public const int E65304 = 65304;
        /// <summary>
        /// 个性化菜单数量受限
        /// </summary>
        public const int E65305 = 65305;
        /// <summary>
        /// 不支持个性化菜单的帐号
        /// </summary>
        public const int E65306 = 65306;
        /// <summary>
        /// 个性化菜单信息为空
        /// </summary>
        public const int E65307 = 65307;
        /// <summary>
        /// 包含没有响应类型的button
        /// </summary>
        public const int E65308 = 65308;
        /// <summary>
        /// 个性化菜单开关处于关闭状态
        /// </summary>
        public const int E65309 = 65309;
        /// <summary>
        /// 填写了省份或城市信息，国家信息不能为空
        /// </summary>
        public const int E65310 = 65310;
        /// <summary>
        /// 填写了城市信息，省份信息不能为空
        /// </summary>
        public const int E65311 = 65311;
        /// <summary>
        /// 不合法的国家信息
        /// </summary>
        public const int E65312 = 65312;
        /// <summary>
        /// 不合法的省份信息
        /// </summary>
        public const int E65313 = 65313;
        /// <summary>
        /// 不合法的城市信息
        /// </summary>
        public const int E65314 = 65314;
        /// <summary>
        /// 该公众号的菜单设置了过多的域名外跳（最多跳转到3个域名的链接）
        /// </summary>
        public const int E65316 = 65316;
        /// <summary>
        /// 不合法的URL
        /// </summary>
        public const int E65317 = 65317;
        /// <summary>
        /// POST数据参数不合法
        /// </summary>
        public const int E9001001 = 9001001;
        /// <summary>
        /// 远端服务不可用
        /// </summary>
        public const int E9001002 = 9001002;
        /// <summary>
        /// Ticket不合法
        /// </summary>
        public const int E9001003 = 9001003;
        /// <summary>
        /// 获取摇周边用户信息失败
        /// </summary>
        public const int E9001004 = 9001004;
        /// <summary>
        /// 获取商户信息失败
        /// </summary>
        public const int E9001005 = 9001005;
        /// <summary>
        /// 获取OpenID失败
        /// </summary>
        public const int E9001006 = 9001006;
        /// <summary>
        /// 上传文件缺失
        /// </summary>
        public const int E9001007 = 9001007;
        /// <summary>
        /// 上传素材的文件类型不合法
        /// </summary>
        public const int E9001008 = 9001008;
        /// <summary>
        /// 上传素材的文件尺寸不合法
        /// </summary>
        public const int E9001009 = 9001009;
        /// <summary>
        /// 上传失败
        /// </summary>
        public const int E9001010 = 9001010;
        /// <summary>
        /// 帐号不合法
        /// </summary>
        public const int E9001020 = 9001020;
        /// <summary>
        /// 已有设备激活率低于50%，不能新增设备
        /// </summary>
        public const int E9001021 = 9001021;
        /// <summary>
        /// 设备申请数不合法，必须为大于0的数字
        /// </summary>
        public const int E9001022 = 9001022;
        /// <summary>
        /// 已存在审核中的设备ID申请
        /// </summary>
        public const int E9001023 = 9001023;
        /// <summary>
        /// 一次查询设备ID数量不能超过50
        /// </summary>
        public const int E9001024 = 9001024;
        /// <summary>
        /// 设备ID不合法
        /// </summary>
        public const int E9001025 = 9001025;
        /// <summary>
        /// 页面ID不合法
        /// </summary>
        public const int E9001026 = 9001026;
        /// <summary>
        /// 页面参数不合法
        /// </summary>
        public const int E9001027 = 9001027;
        /// <summary>
        /// 一次删除页面ID数量不能超过10
        /// </summary>
        public const int E9001028 = 9001028;
        /// <summary>
        /// 页面已应用在设备中，请先解除应用关系再删除
        /// </summary>
        public const int E9001029 = 9001029;
        /// <summary>
        /// 一次查询页面ID数量不能超过50
        /// </summary>
        public const int E9001030 = 9001030;
        /// <summary>
        /// 时间区间不合法
        /// </summary>
        public const int E9001031 = 9001031;
        /// <summary>
        /// 保存设备与页面的绑定关系参数错误
        /// </summary>
        public const int E9001032 = 9001032;
        /// <summary>
        /// 门店ID不合法
        /// </summary>
        public const int E9001033 = 9001033;
        /// <summary>
        /// 设备备注信息过长
        /// </summary>
        public const int E9001034 = 9001034;
        /// <summary>
        /// 设备申请参数不合法
        /// </summary>
        public const int E9001035 = 9001035;
        /// <summary>
        /// 查询起始值begin不合法
        /// </summary>
        public const int E9001036 = 9001036;
    }
}
