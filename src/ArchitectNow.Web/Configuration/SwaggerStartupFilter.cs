using System;
using System.Linq;
using ArchitectNow.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.WebApi;

namespace ArchitectNow.Web.Configuration
{
    public class SwaggerStartupFilter : IStartupFilter
    {
        private readonly ILogger<SwaggerStartupFilter> _logger;
        protected SwaggerOptions[] Options { get; }

        public SwaggerStartupFilter(ILogger<SwaggerStartupFilter> logger, params SwaggerOptions[] options)
        {
            _logger = logger;
            Options = options;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                _logger.LogInformation($"Configure Start: {nameof(SwaggerStartupFilter)}");

                foreach (var option in Options)
                {
                    
                    if (option.Controllers?.Any() == true)
                    {
                        builder.UseSwaggerUi(option.Controllers, settings =>
                            {
                                ConfigureSettings(settings, option);
                            });
                    }
                    else
                    {
                        builder.UseSwaggerUi(option.ControllerAssembly, settings => { ConfigureSettings(settings, option); });
                    }
                }
                
                next(builder);
                _logger.LogInformation($"Configure End: {nameof(SwaggerStartupFilter)}");
            };
        }

        private void ConfigureSettings(SwaggerUiSettings<WebApiToSwaggerGeneratorSettings> settings, SwaggerOptions option)
        {
            settings.SwaggerRoute = option.SwaggerRoute;
            settings.SwaggerUiRoute = option.SwaggerUiRoute;
            settings.UseJsonEditor = true;
            settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
            settings.GeneratorSettings.Title = option.Title;
            settings.GeneratorSettings.FlattenInheritanceHierarchy = true;
            settings.GeneratorSettings.IsAspNetCore = true;

            foreach (var optionDocumentProcessor in option.DocumentProcessors)
            {
                settings.GeneratorSettings.DocumentProcessors.Add(optionDocumentProcessor);
            }

            foreach (var operationProcessor in option.OperationProcessors)
            {
                settings.GeneratorSettings.OperationProcessors.Add(operationProcessor);
            }
            var action = option.Configure;
            action?.Invoke(settings);
        }
    }
}