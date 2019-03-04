using System.Collections.Generic;
using System.Linq;
using ArchitectNow.Web.Models;
using Microsoft.AspNetCore.Builder.Internal;
using NJsonSchema;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.WebApi;

namespace ArchitectNow.Web.Configuration
{
    public static class SwaggerExtensions
    {
        public static void ConfigureSwaggerUi3(this ApplicationBuilder builder,
            IEnumerable<SwaggerOptions<SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>>> options)
        {
            foreach (var option in options)
            {
                if (option.Controllers?.Any() == true)
                {
                    builder.UseSwaggerUi3(option.Controllers, settings => { ConfigureSettings(settings, option); });
                }
                else
                {
                    builder.UseSwaggerUi3(option.ControllerAssembly, settings => { ConfigureSettings(settings, option); });
                }
            }
        }

        public static void ConfigureSwaggerUi(this ApplicationBuilder builder,
            IEnumerable<SwaggerOptions<SwaggerUiSettings<WebApiToSwaggerGeneratorSettings>>> options)
        {
            foreach (var option in options)
            {
                if (option.Controllers?.Any() == true)
                {
                    builder.UseSwaggerUi(option.Controllers, settings => { ConfigureSettings(settings, option); });
                }
                else
                {
                    builder.UseSwaggerUi(option.ControllerAssembly, settings => { ConfigureSettings(settings, option); });
                }
            }
        }

        public static void ConfigureSwaggerReDoc(this ApplicationBuilder builder,
            IEnumerable<SwaggerOptions<SwaggerReDocSettings<WebApiToSwaggerGeneratorSettings>>> options)
        {
            foreach (var option in options)
            {
                if (option.Controllers?.Any() == true)
                {
                    builder.UseSwaggerReDoc(option.Controllers, settings => { ConfigureSettings(settings, option); });
                }
                else
                {
                    builder.UseSwaggerReDoc(option.ControllerAssembly, settings => { ConfigureSettings(settings, option); });
                }
            }
        }

        private static void ConfigureSettings<T>(T settings, SwaggerOptions<T> option) where T : SwaggerUiSettingsBase<WebApiToSwaggerGeneratorSettings>
        {
            settings.DocumentPath = option.SwaggerRoute;
            settings.Path = option.SwaggerUiRoute;
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