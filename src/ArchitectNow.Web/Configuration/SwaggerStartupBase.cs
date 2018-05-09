using System;
using ArchitectNow.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace ArchitectNow.Web.Configuration
{
    public abstract class SwaggerStartupBase<TSettings, TSwaggerSettings> : IStartupFilter where TSettings : SwaggerOptionsBase
    {
        protected abstract void ConfigureSettings(TSwaggerSettings settings, TSettings option);
        public abstract Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next);
    }
}