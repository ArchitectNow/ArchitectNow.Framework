using System;
using System.Threading.Tasks;

namespace ArchitectNow.Caching
{
    class NoOpCachingService: ICachingService
    {
        public T GetOrAdd<T>(string key, Func<string, T> valueFactory)
        {
            return valueFactory(key);
        }

        public bool Remove<T>(string key)
        {
            return true;
        }

        public void Clear(string region)
        {
            
        }

        public void ClearAll()
        {
            
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<string, T> valueFactory)
        {
            return GetOrAdd(key, valueFactory).AsResult();
        }

        public Task<bool> RemoveAsync<T>(string key)
        {
            return true.AsResult();
        }

        public Task ClearAsync(string region)
        {
            return Task.Run(() => { });
        }

        public Task ClearAllAsync()
        {
            return Task.Run(() => { });
        }

        public T AddOrUpdate<T>(string key, T value)
        {
            return value;
        }

        public Task<T> AddOrUpdateAsync<T>(string key, T value)
        {
            return AddOrUpdate(key, value).AsResult();
        }
    }
}