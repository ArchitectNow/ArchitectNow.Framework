using System;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Sql.Configuration
{
	static class HangfireExtensions
	{
		public static void ConfigureHangfire(this IServiceCollection services, Func<string> getConnectionString, Action<IGlobalConfiguration> setupAction = null)
		{
			services.AddHangfire(globalConfiguration =>
			{
				var connectionString = getConnectionString();
				globalConfiguration.UseSqlServerStorage(connectionString);
				setupAction?.Invoke(globalConfiguration);
			});
		}
	}
}