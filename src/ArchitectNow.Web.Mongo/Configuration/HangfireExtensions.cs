using System;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Mongo.Configuration
{
	static class HangfireExtensions
	{
		public static void ConfigureHangfire(this IServiceCollection services, string connectionString, string databaseName, Action<IGlobalConfiguration> setupAction = null)
		{
			services.AddHangfire(globalConfiguration =>
			{
				setupAction?.Invoke(globalConfiguration);
			});
		}
	}
}