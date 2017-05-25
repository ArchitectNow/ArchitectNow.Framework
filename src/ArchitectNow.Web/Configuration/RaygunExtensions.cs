using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mindscape.Raygun4Net;

namespace ArchitectNow.Web.Configuration
{
    public static class RaygunExtensions
    {
	    public static void ConfigureRaygun(this IServiceCollection services, IConfigurationRoot configurationRoot)
	    {
			services.AddRaygun(configurationRoot);
		}
    }
}
