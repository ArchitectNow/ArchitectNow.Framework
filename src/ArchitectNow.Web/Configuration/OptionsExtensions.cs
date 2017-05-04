using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Configuration
{
	public static class OptionsExtensions
	{
		public static void ConfigureOptions(this IServiceCollection services)
		{
			services.AddOptions();
		}
	}
}