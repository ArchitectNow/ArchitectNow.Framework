using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace ArchitectNow.Web.Configuration
{
	public static class LoggingExtensions
	{
		public static void ConfigureLogging(this IServiceCollection services)
		{
			services.AddLogging();
		}

		public static void ConfigureLogger(this IHostingEnvironment environment, ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot)
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
			var logger = new LoggerConfiguration()
				.WriteTo
				.RollingFile($@"{logPath}\{{Date}}.txt", logLevel, retainedFileCountLimit: 10, shared: true)
				.WriteTo
				.ColoredConsole()
				.WriteTo
				.LiterateConsole()
				.CreateLogger();

			Log.Logger = logger;

			Log.Write(LogEventLevel.Information, "Logging has started");

			if (logLevel <= LogEventLevel.Debug)
			{
				//DbInterception.Add(new DatabaseLogger($@"{baseDir}\logs\sql-{DateTimeOffset.UtcNow:yyyyMMdd}.txt"));
			}

			loggerFactory.AddConsole(configurationRoot.GetSection("Logging"));
			loggerFactory.AddDebug();
			loggerFactory.AddSerilog(logger);
		}
	}
}