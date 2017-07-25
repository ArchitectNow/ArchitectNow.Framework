using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ArchitectNow.Web.Configuration
{
	public static class LoggingExtensions
	{
		private static readonly LoggingLevelSwitch LogEventSwitch = new LoggingLevelSwitch();
		public static void ConfigureLogging(this IServiceCollection services)
		{
			services.AddLogging();
			services.AddSingleton(LogEventSwitch);
		}

		public static void ConfigureLogger(this IHostingEnvironment environment, ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot, Action<LoggerConfiguration> setupEnrichers, Action<LoggerConfiguration> setupAction = null)
		{
			var baseDir = environment.ContentRootPath;
			var logPath = Path.Combine(baseDir, "logs");
			if (!Directory.Exists(logPath))
			{
				Directory.CreateDirectory(logPath);
			}

			LogEventLevel logLevel;
			
			if (!Enum.TryParse(configurationRoot["logging:logLevel:system"], true, out logLevel))
				logLevel = LogEventLevel.Verbose;

			LogEventSwitch.MinimumLevel = logLevel;

			var loggingConfiguration = new LoggerConfiguration()
				.Enrich.FromLogContext();

			setupEnrichers?.Invoke(loggingConfiguration);
			
			loggingConfiguration
				.MinimumLevel.ControlledBy(LogEventSwitch)
				.WriteTo
				.RollingFile($@"{logPath}\{{Date}}.txt", logLevel, retainedFileCountLimit: 10, shared: true)
				.WriteTo
				.ColoredConsole()
				.WriteTo
				.LiterateConsole();

			setupAction?.Invoke(loggingConfiguration);

			var logger = loggingConfiguration.CreateLogger();

			Log.Logger = logger;

			Log.Write(LogEventLevel.Information, "Logging has started");
			
			loggerFactory.AddConsole(configurationRoot.GetSection("Logging"));
			loggerFactory.AddDebug();
			loggerFactory.AddSerilog(logger);
		}
	}
}