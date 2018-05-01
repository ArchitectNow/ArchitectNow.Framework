using System;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ArchitectNow.Web.Configuration
{
	public class HangfireStartupFilter: IStartupFilter
	{
		private readonly ILogger<HangfireStartupFilter> _logger;
		protected virtual string HangfireDashboardUrl { get; } = "/hangfire";
		protected virtual bool UseHangfireServer { get; } = false;

		public HangfireStartupFilter(ILogger<HangfireStartupFilter> logger)
		{
			_logger = logger;
		}
		
		public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
		{
			return builder =>
			{
				_logger.LogInformation($"Configure Start: {nameof(HangfireStartupFilter)}");

				if (UseHangfireServer)
				{
					builder.UseHangfireServer();
				}
			
				builder.UseHangfireDashboard(HangfireDashboardUrl, ConfigureDashboard());
				
				next(builder);
				
				_logger.LogInformation($"Configure End: {nameof(HangfireStartupFilter)}");

			};
		}

		protected virtual DashboardOptions ConfigureDashboard()
		{
			return new DashboardOptions();
		}
	}
}