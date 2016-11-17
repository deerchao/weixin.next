using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Json.Net
{
    public class JsonParser : IJsonParser
    {
        public IJsonValue Parse(string text)
        {
            return new JsonValue(JObject.Parse(text));
        }

        public T Build<T>(IJsonValue value)
        {
            var v = (JsonValue) value;
            var obj = v.Obj;
            return obj.ToObject<T>();
        }

        public string ToString(object target)
        {
            return JsonConvert.SerializeObject(target, Formatting.None);
        }
    }
}