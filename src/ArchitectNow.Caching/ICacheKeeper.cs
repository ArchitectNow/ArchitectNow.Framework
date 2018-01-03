using CacheManager.Core;

namespace ArchitectNow.Caching
{
    public interface ICacheKeeper<T>
    {
        ICacheManager<T> GetCacheManager();
    }
}