using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ArchitectNow.Web.Configuration
{
	public static class ConfigurationExtensions
	{
		public static IConfigurationRoot BuildConfigurationRoot(this IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", true, true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
				.AddEnvironmentVariables();

			return builder.Build();
		}
	}
}