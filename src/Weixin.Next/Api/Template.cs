using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Weixin.Next.Api
{
    // ReSharper disable InconsistentNaming
    public static class Template
    {
        #region 发送模板消息

        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="touser">接收消息的用户 openid</param>
        /// <param name="template_id">模板id</param>
        /// <param name="parameters"></param>
        /// <param name="first">模板 first 参数</param>
        /// <param name="remark">模板 remark 参数</param>
        /// <param name="url">接收者点击模板消息后要打开的网址</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<SendTemplateResult> Send(string touser, string template_id, object parameters, TemplateParameter first = null, TemplateParameter remark = null, string url = null, ApiConfig config = null)
        {
            return ApiHelper.PostResult<SendTemplateResult>("https://api.weixin.qq.com/cgi-bin/message/template/send?$acac$", new { touser, template_id, url, data = BuildParameterObject(parameters, first, remark) }, config);
        }

        private static object BuildParameterObject(object parameters, TemplateParameter first, TemplateParameter remark)
        {
            var properties = GetParameterProperties(parameters.GetType());

            var result = new Dictionary<string, TemplateParameter>();
            foreach (var property in properties)
            {
                var v = GetPropertyValue(parameters, property);
                if (v == null)
                    continue;

                var p = v as TemplateParameter;
                if (p != null)
                {
                    result.Add(property.Name, p);
                    continue;
                }

                var s = v as string;
                if (s != null)
                {
                    result.Add(property.Name, new TemplateParameter(s));
                    continue;
                }

                throw new ArgumentException($"{nameof(parameters)} 的属性 {property.Name} 不是 {nameof(TemplateParameter)} 或 string 类型(${v.GetType().FullName})", nameof(parameters));
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

        public class TemplateParameter
        {
            public TemplateParameter(string value = null, string color = "#173177")
            {
                this.value = value;
                this.color = color;
            }

            public string value { get; set; }
            public string color { get; set; }

            public static implicit operator TemplateParameter(string value)
            {
                return new TemplateParameter(value);
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
    }
}
