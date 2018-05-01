using CacheManager.Core;
using CacheManager.Core.Internal;
using CacheManager.Core.Logging;

namespace ArchitectNow.Caching
{
    public class NoOpCacheHandler<T> : BaseCacheHandle<T>{
        public NoOpCacheHandler(ICacheManagerConfiguration managerConfiguration, CacheHandleConfiguration configuration, ILoggerFactory loggerFactory) : base(managerConfiguration, configuration)
        {
            Logger = loggerFactory.CreateLogger(this);
        }

        public override void Clear()
        {
            
        }

        public override void ClearRegion(string region)
        {
            
        }

        public override bool Exists(string key)
        {
            return false;
        }

        public override bool Exists(string key, string region)
        {
            return false;
        }

        protected override CacheItem<T> GetCacheItemInternal(string key)
        {
            return null;
        }

        protected override CacheItem<T> GetCacheItemInternal(string key, string region)
        {
            return null;
        }

        protected override bool RemoveInternal(string key)
        {
            return true;
        }

        protected override bool RemoveInternal(string key, string region)
        {
            return true;
        }

        protected override ILogger Logger { get; }
        
        protected override bool AddInternalPrepared(CacheItem<T> item)
        {
            return true;
        }

        protected override void PutInternalPrepared(CacheItem<T> item)
        {
            
        }

        public override int Count { get; } = 0;
    }
}