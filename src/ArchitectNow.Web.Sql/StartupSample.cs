using System;
using System.Reflection;
using System.Text;
using ArchitectNow.Models.Security;
using ArchitectNow.Services;
using ArchitectNow.Web.Configuration;
using ArchitectNow.Web.Models;
using ArchitectNow.Web.Sql.Configuration;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using Serilog;

namespace ArchitectNow.Web.Sql
{
	public sealed class StartupSample
    {
        private readonly ILogger<StartupSample> _logger;
        private readonly IConfiguration _configuration;

        public StartupSample(ILogger<StartupSample> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation($"{nameof(ConfigureServices)} starting...");
            
            services.ConfigureOptions();
            
            services.ConfigureJwt(_configuration, ConfigureSecurityKey);

            services.ConfigureApi(new FluentValidationOptions {Enabled = true});

            services.ConfigureAutomapper(expression => { });

            services.ConfigureCompression();

            //Register startup filters (order matters)
            services.AddTransient<IStartupFilter, AntiForgeryStartupFilter>();
            
            //last
            services.AddTransient<IStartupFilter, HangfireStartupFilter>();

	        services.ConfigureHangfire( _configuration["redis:connectionString"], configuration => { });
            

            _logger.LogInformation($"{nameof(ConfigureServices)} complete...");
        }

        private JwtSigningKey ConfigureSecurityKey(JwtIssuerOptions issuerOptions)
        {
            var keyString = issuerOptions.Audience;
            var keyBytes = Encoding.Unicode.GetBytes(keyString);
            var signingKey = new JwtSigningKey(keyBytes);
            return signingKey;
        }

        public void Configure(
            IApplicationBuilder app,
            IHostApplicationLifetime appLifetime)
        {
            _logger.LogInformation($"{nameof(Configure)} starting...");

            //Add custom middleware or use IStartupFilter
            // app.UseSwaggerUi3(settings =>
            // {
            //     settings.GeneratorSettings.Title = "API";
            //     settings.GeneratorSettings.Description = "API";
            //     settings.DocumentPath = "/app/docs/v1/swagger.json";
            //     settings.Path = "/app/docs";
            //     settings.GeneratorSettings.DefaultEnumHandling = EnumHandling.String;
            //     settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
            //     settings.GeneratorSettings.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            // });
            //
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            _logger.LogInformation($"{nameof(Configure)} complete...");
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.CreateAutofacContainer();
            builder.RegisterModule<WebModule>();
            builder.RegisterModule<ServicesModule>();
        }
    }
}
