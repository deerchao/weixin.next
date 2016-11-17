using System.Collections.Concurrent;
using System.Threading.Tasks;
using Weixin.Next.MP.Messaging.Responses;

namespace Weixin.Next.MP.Messaging.Caches
{
    /// <summary>
    /// 代表正在处理中的消息, 应保证线程安全
    /// </summary>
    public interface IExecutionDictionary
    {
        void Add(string key, Task<IResponseMessage> task);

        Task<IResponseMessage> Get(string key, bool remove);

        void Remove(string key);
    }

    /// <summary>
    /// 不执行实际缓存, 效果是不进行消息排重
    /// </summary>
    public class NullExecutionDictionary : IExecutionDictionary
    {
        public void Add(string key, Task<IResponseMessage> task)
        {
        }

        public Task<IResponseMessage> Get(string key, bool remove)
        {
            return null;
        }

        public void Remove(string key)
        {
        }
    }

    /// <summary>
    /// 默认处理消息
    /// </summary>
    public class ExecutionDictionary : IExecutionDictionary
    {
        private readonly ConcurrentDictionary<string, Task<IResponseMessage>> _dictionary = new ConcurrentDictionary<string, Task<IResponseMessage>>();


        public void Add(string key, Task<IResponseMessage> task)
        {
            _dictionary.TryAdd(key, task);
        }

        public Task<IResponseMessage> Get(string key, bool remove)
        {
            Task<IResponseMessage> result;
            if (remove)
            {
                _dictionary.TryRemove(key, out result);
            }
            else
            {
                _dictionary.TryGetValue(key, out result);
            }
            return result;
        }

        public void Remove(string key)
        {
            Task<IResponseMessage> result;
            _dictionary.TryRemove(key, out result);
        }
    }
}