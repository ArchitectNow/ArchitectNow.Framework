using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ArchitectNow.Web.Configuration;
using ArchitectNow.Web.Models.Options;
using ArchitectNow.Web.Sql.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Antiforgery;
using Mindscape.Raygun4Net;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Models = ArchitectNow.Web.Models;

namespace ArchitectNow.Web.Sql
{
	public abstract class StartupBase
	{
		private IContainer _applicationContainer;
		private readonly IConfigurationRoot _configurationRoot;
		private readonly ILogger<StartupBase> _logger;

		protected StartupBase(IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			_configurationRoot = env.BuildConfigurationRoot();
			_logger = loggerFactory.CreateLogger<StartupBase>();
		}
		
		protected abstract Features Features { get; }

		protected abstract Info SwaggerInfo { get; }
		protected abstract Models.Options.SwaggerOptions SwaggerOptions { get; }
		protected abstract string GetHangfireConnectionString();

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		protected IServiceProvider ConfigureServicesInternal(IServiceCollection services)
		{
			_logger.LogInformation($"{nameof(ConfigureServicesInternal)} starting...");

			services.ConfigureLogging();

			services.ConfigureOptions();

			services.ConfigureApi();

			if (Features.EnableCompression)
			{
				services.ConfigureCompression();
			}

			services.ConfigureSwagger(SwaggerOptions.Name, SwaggerInfo, SetupSwagger);

			if (Features.UseHangfire)
			{
				services.AddHangfire(GetHangfireConnectionString, ConfigureHangfire);
			}

			if (Features.UseRaygun)
			{
				services.ConfigureRaygun(_configurationRoot);
			}

			//last
			_applicationContainer = services.CreateAutofacContainer(_configurationRoot, ConfigureAutofac);

			// Create the IServiceProvider based on the container.
			var provider = new AutofacServiceProvider(_applicationContainer);

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

		protected void ConfigureInternal(
			IApplicationBuilder app,
			IHostingEnvironment env,
			ILoggerFactory loggerFactory,
			IApplicationLifetime appLifetime,
			IAntiforgery antiforgery,
			IConfigurationRoot configurationRoot)
		{
			_logger.LogInformation($"{nameof(ConfigureInternal)} starting...");

			env.ConfigureLogger(loggerFactory, _configurationRoot);

			app.ConfigureJwt(_configurationRoot);

			app.ConfigureAntiForgery(antiforgery);

			app.ConfigureAssets(configurationRoot);

			app.ConfigureSwagger(SwaggerOptions.Description, ConfigureSwaggerUi);

			app.ConfigureCompression();

			app.UseMvc();

			app.ConfigureHangfire(_applicationContainer, ConfigureDashboardOptions);

			appLifetime.ApplicationStopped.Register(() => _applicationContainer.Dispose());

			_logger.LogInformation($"{nameof(ConfigureInternal)} complete...");

		}

		protected virtual void ConfigureSwaggerUi(SwaggerUIOptions swaggerUiOptions)
		{
			
		}

		protected virtual void ConfigureDashboardOptions(DashboardOptions dashboardOptions)
		{
			
		}
	}
}