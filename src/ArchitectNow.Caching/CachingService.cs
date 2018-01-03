using System;
using System.Threading.Tasks;
using CacheManager.Core;

namespace ArchitectNow.Caching
{
    class CachingService : ICachingService
    {
        private const string Region = "caching";
        private readonly ICacheMangerFactory _cacheMangerFactory;

        public CachingService(ICacheMangerFactory cacheMangerFactory)
        {
            _cacheMangerFactory = cacheMangerFactory;
        }

        public T AddOrUpdate<T>(string key, T value)
        {
            var cacheManager = GetCacheManager<T>();

            return cacheManager.AddOrUpdate(key, value, _ => value);
        }

        public T GetOrAdd<T>(string key, Func<string, T> valueFactory)
        {
            var cacheManager = GetCacheManager<T>();
             
            cacheManager.TryGetOrAdd(key, Region, (k, r) => valueFactory(key), out var result);

            return result;
        }
        
        public bool Remove<T>(string key)
        {
            var cacheManager = GetCacheManager<T>();
            return cacheManager.Remove(key, Region);
        }

        public void Clear(string region)
        {
            var cacheManager = GetCacheManager<object>();
            cacheManager.ClearRegion(region);
        }

        public void ClearAll()
        {
            Clear(Region);
        }

        public Task<T> AddOrUpdateAsync<T>(string key, T value)
        {
            return AddOrUpdate(key, value).AsResult();
        }
        
        public Task<T> GetOrAddAsync<T>(string key, Func<string, T> valueFactory)
        {
            return GetOrAdd(key, valueFactory).AsResult();
        }
        
        public Task<bool> RemoveAsync<T>(string key)
        {
            return Remove<T>(key).AsResult();
        }

        public Task ClearAsync(string region)
        {
            return Task.Run(() => Clear(region));
        }

        public Task ClearAllAsync()
        {
            return Task.Run(()=> ClearAll());
        }

        private ICacheManager<T> GetCacheManager<T>()
        {
            return _cacheMangerFactory.Resolve<T>();
        }
    }
}