using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ArchitectNow.Web.Configuration
{
    public class MvcStartupFilter : IStartupFilter
    {
        private readonly ILogger<MvcStartupFilter> _logger;

        public MvcStartupFilter(ILogger<MvcStartupFilter> logger)
        {
            _logger = logger;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                _logger.LogInformation($"Configure End: {nameof(MvcStartupFilter)}");

                builder.UseMvc();
                next(builder);

                _logger.LogInformation($"Configure End: {nameof(MvcStartupFilter)}");
            };
        }
    }
}