using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ArchitectNow.Models.Options
{
    public static class OptionsExtensions
    {
	    public static IOptions<TOptions> CreateOptions<TOptions>(this IConfigurationRoot configurationRoot, string section) where TOptions : class, new()
	    {
		    var options = configurationRoot.GetSection(section).Get<TOptions>();

		    return new OptionsWrapper<TOptions>(options);
		}
    }
}
