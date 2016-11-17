using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Api
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// 模板消息
    /// </summary>
    public static class Template
    {
        #region 发送模板消息

        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="touser">接收消息的用户 openid</param>
        /// <param name="template_id">模板id</param>
        /// <param name="parameters">用于提供模板参数, 例: new { keyword1 = "xxx", keyword2 = new Template.Parameter("yyy", "#173177") } </param>
        /// <param name="first">模板 first 参数</param>
        /// <param name="remark">模板 remark 参数</param>
        /// <param name="url">接收者点击模板消息后要打开的网址</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<SendTemplateResult> Send(string touser, string template_id, object parameters, Parameter first = null, Parameter remark = null, string url = null, ApiConfig config = null)
        {
            return ApiHelper.PostResult<SendTemplateResult>("https://api.weixin.qq.com/cgi-bin/message/template/send?$acac$", new { touser, template_id, url, data = BuildParameterObject(parameters, first, remark) }, config);
        }

        private static object BuildParameterObject(object parameters, Parameter first, Parameter remark)
        {
            var properties = GetParameterProperties(parameters.GetType());

            var result = new Dictionary<string, Parameter>();
            foreach (var property in properties)
            {
                var v = GetPropertyValue(parameters, property);
                if (v == null)
                    continue;

                var p = v as Parameter;
                if (p != null)
                {
                    result.Add(property.Name, p);
                    continue;
                }

                var s = v as string;
                if (s != null)
                {
                    result.Add(property.Name, new Parameter(s));
                    continue;
                }

                throw new ArgumentException($"{nameof(parameters)} 的属性 {property.Name} 不是 {nameof(Parameter)} 或 string 类型(${v.GetType().FullName})", nameof(parameters));
            }

            if (!result.ContainsKey("first") && first != null)
                result.Add("first", first);

            if (!result.ContainsKey("remark") && remark != null)
                result.Add("remark", remark);

            return result;
        }

        #region Reflection related stuff
        private static readonly ConcurrentDictionary<PropertyInfo, Func<object, object>> _getters = new ConcurrentDictionary<PropertyInfo, Func<object, object>>();
        private static readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _properties = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

        private static object GetPropertyValue(object target, PropertyInfo property)
        {
            var getter = _getters.GetOrAdd(property, BuildPropertyGetter);
            return getter(target);
        }

        private static IEnumerable<PropertyInfo> GetParameterProperties(Type type)
        {
            return _properties.GetOrAdd(type, t => GetPublicReadableProperties(t.GetTypeInfo()));
        }

        private static IEnumerable<PropertyInfo> GetPublicReadableProperties(TypeInfo tinfo)
        {
            var overrided = new HashSet<MethodInfo>();

            foreach (var property in tinfo.DeclaredProperties)
            {
                if (!property.CanRead)
                    continue;

                if (property.GetMethod.IsStatic)
                    continue;

                if (!property.GetMethod.IsPublic)
                    continue;

                var method = property.GetMethod;
                var baseMethod = method.GetRuntimeBaseDefinition();
                if (!method.Equals(baseMethod))
                    overrided.Add(baseMethod);

                if (overrided.Contains(method))
                    continue;

                yield return property;
            }

            var basetype = tinfo.BaseType;
            while (basetype != null)
            {
                var baseinfo = basetype.GetTypeInfo();
                foreach (var property in baseinfo.DeclaredProperties)
                {
                    if (!property.CanRead)
                        continue;

                    if (property.GetMethod.IsStatic)
                        continue;

                    if (!property.GetMethod.IsPublic)
                        continue;

                    var method = property.GetMethod;
                    var baseMethod = method.GetRuntimeBaseDefinition();
                    if (!method.Equals(baseMethod))
                        overrided.Add(baseMethod);

                    if (overrided.Contains(method))
                        continue;

                    yield return property;
                }

                basetype = baseinfo.BaseType;
            }
        }

        private static Func<object, object> BuildPropertyGetter(PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof(object), "x");
            // ReSharper disable once AssignNullToNotNullAttribute
            var convertedParameter = Expression.Convert(parameter, property.DeclaringType);
            var readProperty = Expression.Property(convertedParameter, property);
            var convertResult = Expression.Convert(readProperty, typeof(object));
            var expression = Expression.Lambda<Func<object, object>>(convertResult, parameter);
            return expression.Compile();
        }
        #endregion

        public class Parameter
        {
            public Parameter(string value = null, string color = "#173177")
            {
                this.value = value;
                this.color = color;
            }

            public string value { get; set; }
            public string color { get; set; }

            public static implicit operator Parameter(string value)
            {
                return new Parameter(value);
            }
        }

        public class SendTemplateResult : IApiResult
        {
            /// <summary>
            /// 消息id
            /// </summary>
            public long msgid { get; set; }
        }
        #endregion

        #region 设置所属行业

        /// <summary>
        /// 设置所属行业, 每月可修改行业1次，账号仅可使用所属行业中相关的模板
        ///<list type="table">
        ///<listheader><term>代码</term><description>主行业/副行业</description></listheader>
        ///<item><term>1</term><description>IT科技/互联网/电子商务</description></item>
        ///<item><term>2</term><description>IT科技/IT软件与服务</description></item>
        ///<item><term>3</term><description>IT科技/IT硬件与设备</description></item>
        ///<item><term>4</term><description>IT科技/电子技术</description></item>
        ///<item><term>5</term><description>IT科技/通信与运营商</description></item>
        ///<item><term>6</term><description>IT科技/网络游戏</description></item>
        ///<item><term>7</term><description>金融业/银行</description></item>
        ///<item><term>8</term><description>金融业/基金|理财|信托</description></item>
        ///<item><term>9</term><description>金融业/保险</description></item>
        ///<item><term>10</term><description>餐饮/餐饮</description></item>
        ///<item><term>11</term><description>酒店旅游/酒店</description></item>
        ///<item><term>12</term><description>酒店旅游/旅游</description></item>
        ///<item><term>13</term><description>运输与仓储/快递</description></item>
        ///<item><term>14</term><description>运输与仓储/物流</description></item>
        ///<item><term>15</term><description>运输与仓储/仓储</description></item>
        ///<item><term>16</term><description>教育/培训</description></item>
        ///<item><term>17</term><description>教育/院校</description></item>
        ///<item><term>18</term><description>政府与公共事业/学术科研</description></item>
        ///<item><term>19</term><description>政府与公共事业/交警</description></item>
        ///<item><term>20</term><description>政府与公共事业/博物馆</description></item>
        ///<item><term>21</term><description>政府与公共事业/公共事业|非盈利机构</description></item>
        ///<item><term>22</term><description>医药护理/医药医疗</description></item>
        ///<item><term>23</term><description>医药护理/护理美容</description></item>
        ///<item><term>24</term><description>医药护理/保健与卫生</description></item>
        ///<item><term>25</term><description>交通工具/汽车相关</description></item>
        ///<item><term>26</term><description>交通工具/摩托车相关</description></item>
        ///<item><term>27</term><description>交通工具/火车相关</description></item>
        ///<item><term>28</term><description>交通工具/飞机相关</description></item>
        ///<item><term>29</term><description>房地产/建筑</description></item>
        ///<item><term>30</term><description>房地产/物业</description></item>
        ///<item><term>31</term><description>消费品/消费品</description></item>
        ///<item><term>32</term><description>商业服务/法律</description></item>
        ///<item><term>33</term><description>商业服务/会展</description></item>
        ///<item><term>34</term><description>商业服务/中介服务</description></item>
        ///<item><term>35</term><description>商业服务/认证</description></item>
        ///<item><term>36</term><description>商业服务/审计</description></item>
        ///<item><term>37</term><description>文体娱乐/传媒</description></item>
        ///<item><term>38</term><description>文体娱乐/体育</description></item>
        ///<item><term>39</term><description>文体娱乐/娱乐休闲</description></item>
        ///<item><term>40</term><description>印刷/印刷</description></item>
        ///<item><term>41</term><description>其它/其它</description></item>
        ///</list>
        /// </summary>
        /// <param name="industry_id1">公众号模板消息所属行业编号</param>
        /// <param name="industry_id2">公众号模板消息所属行业编号</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task SetIndustry(string industry_id1, string industry_id2, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/template/api_set_industry?$acac$", new { industry_id1, industry_id2 }, config);
        }
        #endregion

        #region 获取设置的行业信息

        /// <summary>
        /// 获取设置的行业信息
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetIndustryResult> GetIndustry(ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetIndustryResult>("https://api.weixin.qq.com/cgi-bin/template/get_industry?$acac$", config);
        }

        public class GetIndustryResult : IApiResult
        {
            /// <summary>
            /// 帐号设置的主营行业
            /// </summary>
            public Industry primary_industry { get; set; }
            /// <summary>
            /// 帐号设置的副营行业
            /// </summary>
            public Industry secondary_industry { get; set; }
        }

        public class Industry
        {
            /// <summary>
            /// 主行业
            /// </summary>
            public string first_class { get; set; }
            /// <summary>
            /// 副行业
            /// </summary>
            public string second_class { get; set; }
        }
        #endregion

        #region 获得已添加模板列表

        /// <summary>
        /// 获取已添加至帐号下所有模板列表
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<GetAllPrivateTemplateResult> GetAllPrivateTemplate(ApiConfig config = null)
        {
            return ApiHelper.GetResult<GetAllPrivateTemplateResult>("https://api.weixin.qq.com/cgi-bin/template/get_all_private_template?$acac$", config);
        }

        public class GetAllPrivateTemplateResult : IApiResult
        {
            public TemplateRecord[] template_list { get; set; }
        }

        public class TemplateRecord
        {
            /// <summary>
            /// 模板ID
            /// </summary>
            public string template_id { get; set; }
            /// <summary>
            /// 模板标题
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 模板所属行业的一级行业
            /// </summary>
            public string primary_industry { get; set; }
            /// <summary>
            /// 模板所属行业的二级行业
            /// </summary>
            public string deputy_industry { get; set; }
            /// <summary>
            /// 模板内容
            /// </summary>
            public string content { get; set; }
            /// <summary>
            /// 模板示例
            /// </summary>
            public string example { get; set; }
        }

        #endregion

        #region 获得模板ID

        /// <summary>
        /// 获得模板ID(需要先从行业模板库选择模板到帐号后台)
        /// </summary>
        /// <param name="template_id_short">模板库中模板的编号，有“TM**”和“OPENTMTM**”等形式</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<ApiAddTemplateResult> ApiAddTemplate(string template_id_short, ApiConfig config = null)
        {
            return ApiHelper.PostResult<ApiAddTemplateResult>("https://api.weixin.qq.com/cgi-bin/template/api_add_template?$acac$", new { template_id_short }, config);
        }

        public class ApiAddTemplateResult : IApiResult
        {
            /// <summary>
            /// 模板ID
            /// </summary>
            public string template_id { get; set; }
        }

        #endregion

        #region 删除模板

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="template_id">模板ID</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task DeletePrivateTemplate(string template_id, ApiConfig config = null)
        {
            return ApiHelper.PostVoid("https://api.weixin.qq.com/cgi-bin/template/del_private_template?$acac$", new { template_id }, config);
        }
        #endregion
    }
}
