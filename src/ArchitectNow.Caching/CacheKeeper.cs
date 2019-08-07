using System;
using CacheManager.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ArchitectNow.Caching
{
    class CacheKeeper<T> : ICacheKeeper<T>
    {
        private readonly ILogger<CacheKeeper<T>> _log;
        private readonly ICacheManager<T> _distributed;
        private readonly ICacheManager<T> _inMemory;
        private bool _distributedEnabled = true;
        private readonly RedisOptions _redisOptions;

        public CacheKeeper(ILogger<CacheKeeper<T>> log, IOptions<RedisOptions> redisOptions, IOptions<CachingOptions> cachingOptions)
        {
            _log = log;
            _redisOptions = redisOptions.Value;
        
            if (!cachingOptions.Value.Enabled)
            {
                _distributedEnabled = false;
                _inMemory = CacheFactory.Build<T>(s => s.WithHandle(typeof(NoOpCacheHandler<>), Guid.NewGuid().ToString("N")));
                return;
            }

            var inMemoryExpirationInSeconds = cachingOptions.Value.InMemoryExpirationInSeconds < 0
                ? CachingConstants.InMemoryDefaultExpirationInSeconds
                : cachingOptions.Value.InMemoryExpirationInSeconds;

            var redisExpirationInSeconds = redisOptions.Value.ExpirationInSeconds < 0
                ? CachingConstants.RedisDefaultExpirationInSeconds
                : redisOptions.Value.ExpirationInSeconds;

            _inMemory = CacheFactory.Build<T>(
                s => s
                    .WithDictionaryHandle()
                    .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(inMemoryExpirationInSeconds)));

            var multiplexer = Create();

            if (multiplexer == null)
            {
                _distributedEnabled = false;
                return;
            }

            multiplexer.ConnectionFailed += (sender, args) =>
            {
                _distributedEnabled = false;

                _log.LogDebug("Connection failed, disabling redis...");
            };

            multiplexer.ConnectionRestored += (sender, args) =>
            {
                _distributedEnabled = true;

                _log.LogDebug("Connection restored, redis is back...");
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
                        .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(inMemoryExpirationInSeconds))
                        .And
                        .WithRedisConfiguration("redis", multiplexer)
                        .WithRedisCacheHandle("redis")
                        .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(redisExpirationInSeconds));
                });
        }

        public ICacheManager<T> GetCacheManager()
        {
            if (_distributedEnabled && _distributed != null)
            {
                return _distributed;
            }

            return _inMemory;
        }

        private IConnectionMultiplexer Create()
        {
            var isEnabled = _redisOptions.Enabled;

            if (!isEnabled)
            {
                return null;
            }

            var connectionString = _redisOptions.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Missing redis connection string.");
            }

            var configurationOptions = ConfigurationOptions.Parse(connectionString);

            try
            {
                return ConnectionMultiplexer.Connect(configurationOptions);
            }
            catch (Exception exception)
            {
                _log.LogError(exception.Message);
                return null;
            }
        }
    }
}