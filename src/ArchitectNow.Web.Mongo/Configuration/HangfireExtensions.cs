using System;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Mongo.Configuration
{
	static class HangfireExtensions
	{
		public static void ConfigureHangfire(this IServiceCollection services, Func<string> getConnectionString, string databaseName, Action<IGlobalConfiguration> setupAction = null)
		{
			services.AddHangfire(globalConfiguration =>
			{
				var connectionString = getConnectionString();
				//globalConfiguration.UseMongoStorage(connectionString, databaseName);
				setupAction?.Invoke(globalConfiguration);
			});
		}
	}
}