using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ArchitectNow.Web.Configuration
{
    public class CompressionStartupFilter : IStartupFilter
    {
        private readonly ILogger<CompressionStartupFilter> _logger;

        public CompressionStartupFilter(ILogger<CompressionStartupFilter> logger)
        {
            _logger = logger;
        }
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder => {
                _logger.LogInformation($"Configure End: {nameof(CompressionStartupFilter)}");

                builder.UseResponseCompression();
                next(builder);
                _logger.LogInformation($"Configure End: {nameof(CompressionStartupFilter)}");
            };
        }
    }
}