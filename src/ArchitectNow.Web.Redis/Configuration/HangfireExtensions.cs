using System;
using Hangfire;
using Hangfire.Pro.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Redis.Configuration
{
	static class HangfireExtensions
	{
		public static void ConfigureHangfire(this IServiceCollection services, Func<string> getConnectionString, Action<RedisStorageOptions> configureRedis = null)
		{
			services.AddHangfire(globalConfiguration =>
			{
				var connectionString = getConnectionString();
				var redisStorageOptions = new RedisStorageOptions();
				configureRedis?.Invoke(redisStorageOptions);
				globalConfiguration.UseRedisStorage(connectionString, redisStorageOptions);
			});
		}
	}
}