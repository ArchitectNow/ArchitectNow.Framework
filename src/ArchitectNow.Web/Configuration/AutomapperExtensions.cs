using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ArchitectNow.Web.Configuration
{
    public static class AutomapperExtensions
    {
        public static void ConfigureAutomapper(this IServiceCollection services, Action<IMapperConfigurationExpression> action)
        {                    
            services.AddAutoMapper(action);
        }
    }
}
