using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ArchitectNow.Web.Configuration
{
    public class JwtStartupFilter: IStartupFilter
    {
        private readonly ILogger _logger;
        public JwtStartupFilter(ILogger<JwtStartupFilter> logger)
        {
            _logger = logger;
        }
        
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                _logger.LogInformation($"Configure Start: {nameof(JwtStartupFilter)}");
                builder.UseAuthentication(); 
                next(builder);
                _logger.LogInformation($"Configure End: {nameof(JwtStartupFilter)}");
            };
        }
    }
}