using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArchitectNow.Web.Configuration
{
    public class AssetStartupFilter : IStartupFilter
    {
        private readonly ILogger<AssetStartupFilter> _logger;

        public AssetStartupFilter(ILogger<AssetStartupFilter> logger)
        {
            _logger = logger;
        }
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {    
                _logger.LogInformation($"Configure End: {nameof(AssetStartupFilter)}");

                var configuration = builder.ApplicationServices.GetService<IConfiguration>();
                builder.UseFileServer();

                var uploadsPath = configuration["uploadsPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                builder.UseStaticFiles();
                
                next(builder);
                _logger.LogInformation($"Configure End: {nameof(AssetStartupFilter)}");
            };
        }
    }
}