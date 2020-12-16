using System;
using System.Reflection;
using System.Text;
using ArchitectNow.Models.Security;
using ArchitectNow.Web.Configuration;
using ArchitectNow.Web.Models;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ArchitectNow.Web
{
    public sealed class StartupSample
    {
        private readonly ILogger<StartupSample> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private ILifetimeScope _container;
        
        public StartupSample(ILogger<StartupSample> logger, IConfiguration configuration,  IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation($"{nameof(ConfigureServices)} starting...");

            services.ConfigureOptions();

            services.ConfigureJwt(_configuration, ConfigureSecurityKey);

            services.ConfigureApi(new FluentValidationOptions {Enabled = true}, configureMvcBuilder: builder =>
            {
                builder.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            });

            services.ConfigureAutomapper(expression => { });

            services.ConfigureCompression();

            //Register startup filters (order matters)

            services.AddTransient<IStartupFilter, AntiForgeryStartupFilter>();

            //last
            services.AddTransient<IStartupFilter, HangfireStartupFilter>();

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
            IHostApplicationLifetime applicationLifetime,
            IWebHostEnvironment env)
        {
            _logger.LogInformation($"{nameof(Configure)} starting...");
            // new way to get the container at a runtime
            _container = app.ApplicationServices.GetAutofacRoot();
            
            app.UseStaticFiles();
            app.UseCors("DefaultCors");
            app.UseCookiePolicy();

            app.UseSwaggerUi3(settings =>
            {
                settings.DocumentPath = "/app/docs/v1/swagger.json";
                settings.Path = "/app/docs";
            });

            app.UseRouting();
            app.UseAuthentication().UseAuthorization();

            app.UseEndpoints(ep => ep.MapControllers());
            _logger.LogInformation($"{nameof(Configure)} complete...");
            
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                Log.CloseAndFlush();
                _container.Dispose();
            });
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.CreateAutofacContainer(); // add any modules and DI code here
        }
    }
}