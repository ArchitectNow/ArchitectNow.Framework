using System;
using Hangfire;
using Hangfire.Pro.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Redis.Configuration
{
	static class HangfireExtensions
	{
		public static void ConfigureHangfire(this IServiceCollection services, string connectionString, Action<RedisStorageOptions> configureRedis = null, Action<IGlobalConfiguration> configureHangfire = null)
		{
			services.AddHangfire(globalConfiguration =>
			{
				var redisStorageOptions = new RedisStorageOptions();
				
				configureRedis?.Invoke(redisStorageOptions);
				configureHangfire?.Invoke(globalConfiguration);
				globalConfiguration.UseRedisStorage(connectionString, redisStorageOptions);
			});
		}
	}
}