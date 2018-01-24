using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ArchitectNow.Caching
{
    static class OptionsExtensions
    {
        public static IOptions<TOptions> CreateOptions<TOptions>(this IConfigurationRoot configurationRoot, string section) where TOptions : class, new()
        {
            return new OptionsWrapper<TOptions>(configurationRoot.GetSection(section).Get<TOptions>());
        }
    }
}