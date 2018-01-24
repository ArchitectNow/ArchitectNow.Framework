using System;
using System.Threading.Tasks;
using CacheManager.Core;

namespace ArchitectNow.Caching
{
    public interface ICachingService
    {
        T GetOrAdd<T>(string key, Func<string, T> valueFactory);
        bool Remove<T>(string key);
        void Clear(string region);
        void ClearAll();

        Task<T> GetOrAddAsync<T>(string key, Func<string, T> valueFactory);
        Task<bool> RemoveAsync<T>(string key);
        Task ClearAsync(string region);
        Task ClearAllAsync();
        T Get<T>(string key);
        bool Add<T>(string key, T value);
        Task<T> GetAsync<T>(string key);
        Task<bool> AddAsync<T>(string key, T value);
        bool Add<T>(CacheItem<T> value);
        CacheItem<T> GetOrAdd<T>(string key, Func<string, CacheItem<T>> valueFactory);
        Task<CacheItem<T>> GetOrAddAsync<T>(string key, Func<string, CacheItem<T>> valueFactory);
    }
}