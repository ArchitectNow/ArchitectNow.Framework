using System;
using System.Threading.Tasks;

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
        T AddOrUpdate<T>(string key, T value);
        Task<T> AddOrUpdateAsync<T>(string key, T value);
    }
}