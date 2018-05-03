using System;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Sql.Configuration
{
	public static class HangfireExtensions
	{
		public static void ConfigureHangfire(this IServiceCollection services, string connectionString, Action<IGlobalConfiguration> configureHangfire = null)
		{
			services.AddHangfire(globalConfiguration =>
			{
				configureHangfire?.Invoke(globalConfiguration);
				globalConfiguration.UseSqlServerStorage(connectionString);
			});
		}
	}
}