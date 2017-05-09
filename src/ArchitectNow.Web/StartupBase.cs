using System;
using ArchitectNow.Web.Configuration;
using ArchitectNow.Web.Models.Options;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using SwaggerOptions = ArchitectNow.Web.Models.Options.SwaggerOptions;
using AutoMapper;

namespace ArchitectNow.Web
{
	public abstract class StartupBase
	{
		private readonly IConfigurationRoot _configurationRoot;
		private readonly ILogger<StartupBase> _logger;

		protected StartupBase(IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			_configurationRoot = env.BuildConfigurationRoot();
			_logger = loggerFactory.CreateLogger<StartupBase>();
		}
		
		protected abstract Features Features { get; }

		protected abstract Info SwaggerInfo { get; }
		protected abstract SwaggerOptions SwaggerOptions { get; }

		protected IConfigurationRoot ConfigurationRoot => _configurationRoot;
		protected IContainer ApplicationContainer { get; private set; }

		protected abstract string GetHangfireConnectionString();

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		protected virtual IServiceProvider ConfigureServicesInternal(IServiceCollection services, Action<IServiceCollection> beforeCreateContainerAction = null)
		{
			_logger.LogInformation($"{nameof(ConfigureServicesInternal)} starting...");

			services.ConfigureLogging();

			services.ConfigureOptions();

			services.ConfigureApi();

            services.ConfigureAutomapper(ConfigureAutoMapper);

			if (Features.EnableCompression)
			{
				services.ConfigureCompression();
			}

			services.ConfigureSwagger(SwaggerOptions.Name, SwaggerInfo, SetupSwagger);
			
			if (Features.UseRaygun)
			{
				services.ConfigureRaygun(ConfigurationRoot);
			}

			beforeCreateContainerAction?.Invoke(services);

			//last
			ApplicationContainer = services.CreateAutofacContainer(ConfigurationRoot, ConfigureAutofac);

			// Create the IServiceProvider based on the container.
			var provider = new AutofacServiceProvider(ApplicationContainer);

			_logger.LogInformation($"{nameof(ConfigureServicesInternal)} complete...");

			return provider;
		}

		protected virtual void ConfigureHangfire(IGlobalConfiguration globalConfiguration)
		{
			
		}

		protected virtual void ConfigureAutofac(ContainerBuilder containerBuilder)
		{

		}

		protected virtual void SetupSwagger(SwaggerGenOptions swaggerGenOptions)
		{

		}

        protected virtual void ConfigureAutoMapper(IMapperConfigurationExpression configurationExpression)
        {

        }

		protected void ConfigureInternal(
			IApplicationBuilder app,
			IHostingEnvironment env,
			ILoggerFactory loggerFactory,
			IApplicationLifetime appLifetime,
			IAntiforgery antiforgery,
			IConfigurationRoot configurationRoot)
		{
			_logger.LogInformation($"{nameof(ConfigureInternal)} starting...");

			env.ConfigureLogger(loggerFactory, ConfigurationRoot, ConfigureLogging);

			app.ConfigureJwt(ConfigurationRoot);

			app.ConfigureAntiForgery(antiforgery);

			app.ConfigureAssets(configurationRoot);

			app.ConfigureSwagger(SwaggerOptions.Description, ConfigureSwaggerUi);

			if (Features.EnableCompression)
			{
				app.ConfigureCompression();
			}

			app.UseMvc();

			if (Features.UseHangfire)
			{
				app.ConfigureHangfire(ApplicationContainer, ConfigureDashboardOptions);
			}

			appLifetime.ApplicationStopped.Register(() =>
			{
				Log.CloseAndFlush();
				ApplicationContainer.Dispose();
			});

			_logger.LogInformation($"{nameof(ConfigureInternal)} complete...");

		}

		protected virtual void ConfigureLogging(LoggerConfiguration loggerConfiguration)
		{
			
		}

		protected virtual void ConfigureSwaggerUi(SwaggerUIOptions swaggerUiOptions)
		{
			
		}

		protected virtual void ConfigureDashboardOptions(DashboardOptions dashboardOptions)
		{
			
		}
	}
}