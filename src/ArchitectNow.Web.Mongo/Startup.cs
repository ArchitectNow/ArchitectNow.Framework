using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ArchitectNow.Web.Configuration;
using Microsoft.AspNetCore.Antiforgery;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Models = ArchitectNow.Web.Models;

namespace ArchitectNow.Web.Mongo
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
		
		protected abstract Info SwaggerInfo { get; }
		protected abstract Models.Options.SwaggerOptions SwaggerOptions { get; }
		
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		protected IServiceProvider ConfigureServices(IServiceCollection services)
		{
			_logger.LogInformation($"{nameof(ConfigureServices)} starting...");

			services.ConfigureLogging();

			services.ConfigureOptions();

			services.ConfigureApi();

			services.ConfigureCompression();

			services.ConfigureSwagger(SwaggerOptions.Name, SwaggerInfo, SetupSwagger);

			//last
			_applicationContainer = services.CreateAutofacContainer(_configurationRoot,ConfigureAutofac);

			// Create the IServiceProvider based on the container.
			var provider = new AutofacServiceProvider(_applicationContainer);

			_logger.LogInformation($"{nameof(ConfigureServices)} complete...");

			return provider;
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
			env.ConfigureLogger(loggerFactory, _configurationRoot);

			app.ConfigureJwt(_configurationRoot);

			app.ConfigureAntiForgery(antiforgery);

			app.ConfigureAssets(configurationRoot);

			app.ConfigureSwagger(SwaggerOptions.Description);

			app.UseResponseCompression();

			app.UseMvc();

			appLifetime.ApplicationStopped.Register(() => _applicationContainer.Dispose());
		}
	}
}