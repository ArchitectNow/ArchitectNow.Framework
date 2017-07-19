using System;
using ArchitectNow.Services;
using ArchitectNow.Web.Redis.Configuration;
using Autofac;
using Hangfire.Pro.Redis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArchitectNow.Web.Redis
{
	public abstract class RedisStartup<TStartup> : StartupBase<TStartup>
	{
		protected RedisStartup(IHostingEnvironment env, ILoggerFactory loggerFactory) : base(env, loggerFactory)
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
					services.ConfigureHangfire(GetHangfireConnectionString, ConfigureRedis, ConfigureHangfire);
				}
			};

			return base.ConfigureServicesInternal(services, beforeCreateContainerAction);
		}

		protected virtual void ConfigureRedis(RedisStorageOptions options)
		{
		
		}
		
		protected override void ConfigureAutofac(ContainerBuilder containerBuilder)
		{
			base.ConfigureAutofac(containerBuilder);
			containerBuilder.RegisterModule<WebModule>();
			containerBuilder.RegisterModule<ServicesModule>();
		}
    }
}
