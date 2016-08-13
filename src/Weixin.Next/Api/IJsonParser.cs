namespace Weixin.Next.Api
{
    public interface IJsonParser
    {
        IJsonValue Parse(string text);
        T Build<T>(IJsonValue value);

        IJsonValue Generate(object target);
        string ToString(IJsonValue value);
    }

    public interface IJsonValue
    {
        bool HasField(string name);
        IJsonValue Field(string name);
        T As<T>();
    }
}