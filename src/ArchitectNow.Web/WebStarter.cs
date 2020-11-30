using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace ArchitectNow.Web
{
    public class WebStarter<TStartup> where TStartup : class
    {
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public int Run(string[] args, Action<LoggerConfiguration> configureLogger)
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var logPath = Path.Combine(baseDir, "logs");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo
                .RollingFile($@"{logPath}\{{Date}}.txt", retainedFileCountLimit: 10, shared: true)
                .WriteTo.Console();
            
            configureLogger?.Invoke(loggerConfiguration);
            
            Log.Logger = loggerConfiguration
                .CreateLogger();

            try
            {
                BuildWebHost(args)
                    .Run();
                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Site terminated");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private IHost BuildWebHost(string[] args)
        {
            var webHostBuilder = CreateWebHostBuilder(args);
            return webHostBuilder
                .Build();
        }

        protected virtual IHostBuilder CreateWebHostBuilder(string[] args)
        {
            //pulls in environment from cli args or env vars
            var configBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            var configuration = configBuilder.Build();

            //https://github.com/aspnet/MetaPackages/blob/633cb681493c0958a9d215624c173db29e20c23d/src/Microsoft.AspNetCore/WebHost.cs

            // Using generic Host here because it allows us to inject a custom service provider for DI
            var webHostBuilder = Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseStartup<TStartup>()
                        .UseConfiguration(configuration)
                        .UseSerilog(Log.Logger);
                });
            return webHostBuilder;
        }
    }
}