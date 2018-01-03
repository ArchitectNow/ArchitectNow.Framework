using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Configuration
{
    public class AssetStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                var configurationRoot = builder.ApplicationServices.GetService<IConfigurationRoot>();
                builder.UseFileServer();

                var uploadsPath = configurationRoot["uploadsPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                builder.UseStaticFiles();
            };
        }
    }
}