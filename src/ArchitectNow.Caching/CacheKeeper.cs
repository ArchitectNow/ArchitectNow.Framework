using System;
using CacheManager.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ArchitectNow.Caching
{
    class CacheKeeper<T> : ICacheKeeper<T>
    {
        private readonly ICacheManager<T> _distributed;
        private readonly ICacheManager<T> _inMemory;
        private bool _distributedEnabled = true;

        public CacheKeeper(ILogger<CacheKeeper<T>> log, IConnectionMultiplexer multiplexer)
        {
            
            multiplexer.ConnectionFailed += (sender, args) =>
            {
                _distributedEnabled = false;

                log.LogDebug("Connection failed, disabling redis...");
            };

            multiplexer.ConnectionRestored += (sender, args) =>
            {
                _distributedEnabled = true;

                log.LogDebug("Connection restored, redis is back...");
            };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.None
            };

            _distributed = CacheFactory.Build<T>(
                s =>
                {
                    s
                        .WithJsonSerializer(jsonSerializerSettings, jsonSerializerSettings)
                        .WithDictionaryHandle()
                        .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(30))
                        .And
                        .WithRedisConfiguration("redis", multiplexer)
                        .WithRedisCacheHandle("redis");
                });

            _inMemory = CacheFactory.Build<T>(
                s => s
                    .WithDictionaryHandle()
                    .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(5)));
        }

        public ICacheManager<T> GetCacheManager()
        {
            if (_distributedEnabled)
            {
                return _distributed;
            }

            return _inMemory;
        }
    }
}