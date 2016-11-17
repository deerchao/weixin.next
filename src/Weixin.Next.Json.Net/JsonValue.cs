using Newtonsoft.Json.Linq;
using Weixin.Next.MP.Api;

namespace Weixin.Next.Json.Net
{
    public class JsonValue : IJsonValue
    {
        private readonly JObject _obj;

        public JsonValue(JObject obj)
        {
            _obj = obj;
        }

        public JObject Obj
        {
            get { return _obj; }
        }

        public bool HasField(string name)
        {
            return Obj.Property(name) != null;
        }

        public T FieldValue<T>(string name)
        {
            return Obj.Value<T>(name);
        }
    }
}
