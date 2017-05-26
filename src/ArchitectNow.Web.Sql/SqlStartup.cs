using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ArchitectNow.Web.Sql.Configuration;

namespace ArchitectNow.Web.Sql
{
	public abstract class SqlStartup<TStartup> : StartupBase<TStartup>
	{
		protected SqlStartup(IHostingEnvironment env, ILoggerFactory loggerFactory) : base(env, loggerFactory)
		{
		}

		protected override IServiceProvider ConfigureServicesInternal(IServiceCollection services, Action<IServiceCollection> beforeCreateContainerAction = null)
		{
			if (beforeCreateContainerAction == null)
			{
				beforeCreateContainerAction = collection => { };
			}

			beforeCreateContainerAction += collection =>
			{
				if (Features.UseHangfire)
				{
					services.ConfigureHangfire(GetHangfireConnectionString, ConfigureHangfire);
				}
			};

			return base.ConfigureServicesInternal(services, beforeCreateContainerAction);
		}
	}
}