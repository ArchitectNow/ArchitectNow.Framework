using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ArchitectNow.Caching
{
    static class OptionsExtensions
    {
        public static IOptions<TOptions> CreateOptions<TOptions>(this IConfiguration configuration, string section) where TOptions : class, new()
        {
            return new OptionsWrapper<TOptions>(configuration.GetSection(section).Get<TOptions>());
        }
    }
}