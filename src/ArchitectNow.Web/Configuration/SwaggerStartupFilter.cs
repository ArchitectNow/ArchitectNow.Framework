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
    public class SwaggerV2StartupFilter : SwaggerStartupBase<SwaggerOptionsV2, SwaggerUiSettings<WebApiToSwaggerGeneratorSettings>>
    {
        private readonly ILogger<SwaggerV2StartupFilter> _logger;
        protected SwaggerOptionsV2[] Options { get; }

        public SwaggerV2StartupFilter(ILogger<SwaggerV2StartupFilter> logger, params SwaggerOptionsV2[] options)
        {
            _logger = logger;
            Options = options;
        }

        public override Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                _logger.LogInformation($"Configure Start: {nameof(SwaggerV2StartupFilter)}");

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
                _logger.LogInformation($"Configure End: {nameof(SwaggerV2StartupFilter)}");
            };
        }

        protected override void ConfigureSettings(SwaggerUiSettings<WebApiToSwaggerGeneratorSettings> settings, SwaggerOptionsV2 optionV2)
        {
            settings.SwaggerRoute = optionV2.SwaggerRoute;
            settings.SwaggerUiRoute = optionV2.SwaggerUiRoute;
            settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
            settings.GeneratorSettings.Title = optionV2.Title;
            settings.GeneratorSettings.FlattenInheritanceHierarchy = true;
            settings.GeneratorSettings.IsAspNetCore = true;
            
            foreach (var optionDocumentProcessor in optionV2.DocumentProcessors)
            {
                settings.GeneratorSettings.DocumentProcessors.Add(optionDocumentProcessor);
            }

            foreach (var operationProcessor in optionV2.OperationProcessors)
            {
                settings.GeneratorSettings.OperationProcessors.Add(operationProcessor);
            }
            var action = optionV2.Configure;
            action?.Invoke(settings);
        }
    }
}