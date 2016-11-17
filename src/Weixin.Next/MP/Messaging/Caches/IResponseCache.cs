using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Weixin.Next.MP.Messaging.Caches
{
    /// <summary>
    /// <para>用于缓存处理完成后的处理结果, 应保证线程安全</para>
    /// </summary>
    public interface IResponseCache
    {
        Task Add(string key, string response);

        Task<string> Get(string key, bool remove);

        Task Remove(string key);
    }

    /// <summary>
    /// 不进行缓存
    /// </summary>
    public class NullResponseCache : IResponseCache
    {
        private static readonly Task<string> _nullTask = Task.FromResult((string)null);

        public Task Add(string key, string response)
        {
            return _nullTask;
        }

        public Task<string> Get(string key, bool remove)
        {
            return _nullTask;
        }

        public Task Remove(string key)
        {
            return _nullTask;
        }
    }

    public sealed class ResponseCache : IResponseCache, IDisposable
    {
        private static readonly Task<string> _nullTask = Task.FromResult((string)null);

        private Dictionary<string, string> _workingDict = new Dictionary<string, string>();
        private Dictionary<string, string> _backupDict = new Dictionary<string, string>();

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly Timer _exchangeTimer;

        public ResponseCache()
        {
            _exchangeTimer = new Timer(state => ClearOldData(), null, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15));
        }

        public Task Add(string key, string response)
        {
            _lock.EnterWriteLock();

            try
            {
                _backupDict.Remove(key);
                _workingDict[key] = response;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return _nullTask;
        }

        public Task<string> Get(string key, bool remove)
        {
            string result;
            if (remove)
            {
                _lock.EnterWriteLock();

                try
                {
                    if (_workingDict.TryGetValue(key, out result))
                        _workingDict.Remove(key);
                    else if (_backupDict.TryGetValue(key, out result))
                        _backupDict.Remove(key);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            else
            {
                _lock.EnterReadLock();

                try
                {
                    if (!_workingDict.TryGetValue(key, out result))
                    {
                        _backupDict.TryGetValue(key, out result);
                    }
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }

            return result == null
                ? _nullTask
                : Task.FromResult(result);
        }

        public Task Remove(string key)
        {
            _lock.EnterWriteLock();

            try
            {
                _workingDict.Remove(key);
                _backupDict.Remove(key);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            return _nullTask;
        }

        /// <summary>
        /// <para>每 15 秒清空备用字典, 同时将当前字典与备用字典交换</para>
        /// <para>效果是每项数据最少缓存 15 秒, 最多缓存 30 秒</para>
        /// </summary>
        private void ClearOldData()
        {
            _lock.EnterWriteLock();

            try
            {
                _backupDict.Clear();

                var t = _workingDict;
                _workingDict = _backupDict;
                _backupDict = t;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            _exchangeTimer.Dispose();
        }
    }
}