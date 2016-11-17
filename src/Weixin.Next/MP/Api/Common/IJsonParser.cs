namespace Weixin.Next.MP.Api
{
    public interface IJsonParser
    {
        IJsonValue Parse(string text);
        T Build<T>(IJsonValue value);

        string ToString(object target);
    }

    public interface IJsonValue
    {
        bool HasField(string name);
        T FieldValue<T>(string name);
    }
}