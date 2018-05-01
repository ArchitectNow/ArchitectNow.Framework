using System;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Mongo.Configuration
{
	public static class HangfireExtensions
	{
		public static void ConfigureHangfire(this IServiceCollection services, string connectionString, string databaseName, Action<IGlobalConfiguration> setupAction = null)
		{
			services.AddHangfire(globalConfiguration =>
			{
				//do not upgrade past 0.5.9
				globalConfiguration.UseMongoStorage(connectionString, databaseName);
				setupAction?.Invoke(globalConfiguration);
			});
		}
	}
}