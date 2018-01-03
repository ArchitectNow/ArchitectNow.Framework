using Autofac;
using CacheManager.Core;

namespace ArchitectNow.Caching
{
    class CacheMangerFactory : ICacheMangerFactory
    {
        private readonly IComponentContext _context;

        public CacheMangerFactory(IComponentContext context)
        {
            _context = context;
        }

        public ICacheManager<T> Resolve<T>()
        {
            var cacheManager = _context.Resolve<ICacheKeeper<T>>().GetCacheManager();

            return cacheManager;
        }
    }
}