using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace ArchitectNow.Web
{
    public class WebStarter<TStartup> where TStartup : class
    {
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public int Run(string[] args, Action<LoggerConfiguration> configureLogger)
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var logPath = Path.Combine(baseDir, "logs");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo
                .RollingFile($@"{logPath}\{{Date}}.txt", retainedFileCountLimit: 10, shared: true)
                .WriteTo.ColoredConsole();
            
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

        private IWebHost BuildWebHost(string[] args)
        {
            var webHostBuilder = CreateWebHostBuilder(args);
            return webHostBuilder
                .Build();
        }

        protected virtual IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            //pulls in environment from cli args or env vars
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            var configuration = builder.Build();

            //https://github.com/aspnet/MetaPackages/blob/633cb681493c0958a9d215624c173db29e20c23d/src/Microsoft.AspNetCore/WebHost.cs

            var webHostBuilder = WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .UseStartup<TStartup>()
                .UseSerilog(Log.Logger);
            return webHostBuilder;
        }
    }
}