using System;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace ArchitectNow.Web.Configuration
{
	public class HangfireStartupFilter: IStartupFilter
	{
		protected virtual string HangfireDashboardUrl { get; } = "/hangfire";
		protected virtual bool UseHangfireServer { get; } = false;
		
		public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
		{
			return builder =>
			{
				if (UseHangfireServer)
				{
					builder.UseHangfireServer();
				}
			
				builder.UseHangfireDashboard(HangfireDashboardUrl, ConfigureDashboard());
				
				next(builder);
			};
		}

		protected virtual DashboardOptions ConfigureDashboard()
		{
			return new DashboardOptions();
		}
	}
}