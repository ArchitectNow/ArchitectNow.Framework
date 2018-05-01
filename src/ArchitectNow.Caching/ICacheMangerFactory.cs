using CacheManager.Core;

namespace ArchitectNow.Caching
{
    public interface ICacheMangerFactory
    {
        ICacheManager<T> Resolve<T>();
    }
}