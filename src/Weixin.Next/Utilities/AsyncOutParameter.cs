namespace Weixin.Next.Utilities
{
    /// <summary>
    /// async 方法不能用 out 参数, 所以用这个代替, 注意必须在 await 了调用的结果之后才能使用 Value 字段
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncOutParameter<T>
    {
        public T Value { get; set; }
    }

    public static class AsyncOutParameterExtensions
    {
        public static void SetValue<T>(this AsyncOutParameter<T> parameter, T value)
        {
            if (parameter != null)
                parameter.Value = value;
        }
    }
}
