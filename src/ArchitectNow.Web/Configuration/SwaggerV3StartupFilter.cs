using System;
using System.Linq;
using ArchitectNow.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.WebApi;

namespace ArchitectNow.Web.Configuration
{
    public class SwaggerV3StartupFilter : SwaggerStartupBase<SwaggerOptionsV3, SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>>
    {
        private readonly ILogger<SwaggerV3StartupFilter> _logger;
        protected SwaggerOptionsV3[] Options { get; }

        public SwaggerV3StartupFilter(ILogger<SwaggerV3StartupFilter> logger, params SwaggerOptionsV3[] options)
        {
            _logger = logger;
            Options = options;
        }

        public override Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                _logger.LogInformation($"Configure Start: {nameof(SwaggerV3StartupFilter)}");

                foreach (var option in Options)
                {
                    
                    if (option.Controllers?.Any() == true)
                    {
                        builder.UseSwaggerUi3(option.Controllers, settings =>
                        {
                            ConfigureSettings(settings, option);
                        });
                    }
                    else
                    {
                        builder.UseSwaggerUi3(option.ControllerAssembly, settings => { ConfigureSettings(settings, option); });
                    }
                }
                
                next(builder);
                _logger.LogInformation($"Configure End: {nameof(SwaggerV3StartupFilter)}");
            };
        }

        protected override void ConfigureSettings(SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings> settings, SwaggerOptionsV3 option)
        {
            settings.SwaggerRoute = option.SwaggerRoute;
            settings.SwaggerUiRoute = option.SwaggerUiRoute;
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