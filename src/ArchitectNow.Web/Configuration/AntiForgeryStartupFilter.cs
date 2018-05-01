using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArchitectNow.Web.Configuration
{
    public class AntiForgeryStartupFilter : IStartupFilter
    {
        private readonly ILogger<AntiForgeryStartupFilter> _logger;

        public AntiForgeryStartupFilter(ILogger<AntiForgeryStartupFilter> logger)
        {
            _logger = logger;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                _logger.LogInformation($"Configure Start: {nameof(AntiForgeryStartupFilter)}");

                var antiforgery = builder.ApplicationServices.GetService<IAntiforgery>();
                builder.Use(n => context =>
                {
                    if (string.Equals(context.Request.Path.Value, "/", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(context.Request.Path.Value, "/index.html", StringComparison.OrdinalIgnoreCase))
                    {
                        var tokens = antiforgery.GetAndStoreTokens(context);
                        context.Response.Cookies.Append("X-XSRF-TOKEN", tokens.RequestToken,
                            new CookieOptions {HttpOnly = false});
                    }

                    return n(context);
                });
                next(builder);
                _logger.LogInformation($"Configure End: {nameof(AntiForgeryStartupFilter)}");
            };
        }
    }
}