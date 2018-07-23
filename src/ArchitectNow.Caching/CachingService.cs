using System;
using System.Threading.Tasks;
using CacheManager.Core;

namespace ArchitectNow.Caching
{
    class CachingService : ICachingService
    {
        private readonly string _region;
        private readonly ICacheMangerFactory _cacheMangerFactory;

        public CachingService(ICacheMangerFactory cacheMangerFactory, ICachingRegion cachingRegion)
        {
            _cacheMangerFactory = cacheMangerFactory;
            _region = cachingRegion.Region;
        }

        public T Get<T>(string key)
        {
            var cacheManager = GetCacheManager<T>();

            var result = cacheManager.Get(key, _region);

            return result;
        }

        public bool Add<T>(CacheItem<T> value)
        {
            var cacheManager = GetCacheManager<T>();

            var result = cacheManager.Add(value);

            return result;
        }

        public bool Add<T>(string key, T value)
        {
            var cacheManager = GetCacheManager<T>();

            var result = cacheManager.Add(key, value, _region);

            return result;
        }

        public CacheItem<T> GetOrAdd<T>(string key, Func<string, CacheItem<T>> valueFactory)
        {
            var cacheManager = GetCacheManager<T>();

            cacheManager.TryGetOrAdd(key, _region, (k, r) => valueFactory(key), out var result);

            return result;
        }

        public T GetOrAdd<T>(string key, Func<string, T> valueFactory)
        {
            var cacheManager = GetCacheManager<T>();

            cacheManager.TryGetOrAdd(key, _region, (k, r) => valueFactory(key), out var result);

            return result;
        }

        public T Update<T>(string key, Func<T, T> updateValue)
        {
            var cacheManager = GetCacheManager<T>();
            return cacheManager.Update(key, _region, updateValue);
        }


        public T AddOrUpdate<T>(string key, T addValue, Func<T, T> updateValue)
        {
            var cacheManager = GetCacheManager<T>();

            return cacheManager.AddOrUpdate(key, _region, addValue, updateValue);
        }

        public void Put<T>(string key, T value)
        {
            var cacheManager = GetCacheManager<T>();

            cacheManager.Put(key, value, _region);
        }

        public void Expire<T>(string key, ExpirationMode expirationMode, TimeSpan timeout)
        {
            var cacheManager = GetCacheManager<T>();

            cacheManager.Expire(key, _region, expirationMode, timeout);
        }

        public bool Remove<T>(string key)
        {
            var cacheManager = GetCacheManager<T>();
            return cacheManager.Remove(key, _region);
        }

        public void Clear(string region)
        {
            var cacheManager = GetCacheManager<object>();
            cacheManager.ClearRegion(region);
        }

        public void ClearAll()
        {
            Clear(_region);
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Get<T>(key).AsResult();
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<string, T> valueFactory)
        {
            return GetOrAdd(key, valueFactory).AsResult();
        }

        public Task<CacheItem<T>> GetOrAddAsync<T>(string key, Func<string, CacheItem<T>> valueFactory)
        {
            return GetOrAdd(key, valueFactory).AsResult();
        }

        public Task<bool> AddAsync<T>(string key, T value)
        {
            return Add(key, value).AsResult();
        }

        public Task<bool> RemoveAsync<T>(string key)
        {
            return Remove<T>(key).AsResult();
        }
                
        public Task<T> UpdateAsync<T>(string key, Func<T, T> updateValue)
        {
            var cacheManager = GetCacheManager<T>();
            return cacheManager.Update(key, _region, updateValue).AsResult();
        }


        public Task<T> AddOrUpdateAsync<T>(string key, T addValue, Func<T, T> updateValue)
        {
            var cacheManager = GetCacheManager<T>();

            return cacheManager.AddOrUpdate(key, _region, addValue, updateValue).AsResult();
        }

        public Task PutAsync<T>(string key, T value)
        {
            var cacheManager = GetCacheManager<T>();

            cacheManager.Put(key, value, _region);
            return Task.Run(() => { });
        }

        public Task ExpireAsync<T>(string key, ExpirationMode expirationMode, TimeSpan timeout)
        {
            var cacheManager = GetCacheManager<T>();

            cacheManager.Expire(key, _region, expirationMode, timeout);

            return Task.Run(() => { });
        }

        public Task ClearAsync(string region)
        {
            return Task.Run(() => Clear(region));
        }

        public Task ClearAllAsync()
        {
            return Task.Run(() => ClearAll());
        }

        private ICacheManager<T> GetCacheManager<T>()
        {
            return _cacheMangerFactory.Resolve<T>();
        }
    }
}