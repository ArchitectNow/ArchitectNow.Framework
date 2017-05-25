using System;
using ArchitectNow.Web.Filters;
using Autofac;
using Hangfire;
using Microsoft.AspNetCore.Builder;

namespace ArchitectNow.Web.Configuration
{
	public static class HangfireExtensions
	{
		public static void ConfigureHangfire(this IApplicationBuilder app, ILifetimeScope lifetimeScope, Action<DashboardOptions> configureDashboardOptions = null )
		{
			var globalConfiguration = GlobalConfiguration.Configuration;

			//globalConfiguration.UseAutofacActivator(lifetimeScope);
			var raygunJobFilter = lifetimeScope.Resolve<RaygunJobFilter>();
			globalConfiguration.UseFilter(raygunJobFilter);
			//globalConfiguration.UseSerilogLogProvider();

			app.UseHangfireServer();

			var options = new DashboardOptions();
			
			configureDashboardOptions?.Invoke(options);

			app.UseHangfireDashboard("/hangfire", options);
		}
	}
}