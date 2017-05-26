using System;
using System.Reflection;
using ArchitectNow.Web.Models.Options;
using Microsoft.AspNetCore.Builder;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.WebApi.Processors.Security;

namespace ArchitectNow.Web.Configuration
{
    public static class SwaggerExtensions
    {
	    public static void ConfigureSwagger(this IApplicationBuilder app, Assembly assembly, SwaggerOptions options, Action<SwaggerUiOwinSettings> action)
	    {
		    var swaggerUiOwinSettings = new SwaggerUiOwinSettings
		    {
			    DefaultPropertyNameHandling = PropertyNameHandling.CamelCase,
			    Title = options.Title,
			    SwaggerRoute = "/docs/v1/swagger.json",
			    SwaggerUiRoute = "/docs",
			    UseJsonEditor = true,
				FlattenInheritanceHierarchy = true,
				IsAspNetCore = true,
			    DocumentProcessors =
			    {
				    new SecurityDefinitionAppender("Authorization", new SwaggerSecurityScheme
				    {
					    Type = SwaggerSecuritySchemeType.ApiKey,
					    Name = "Authorization",
					    In = SwaggerSecurityApiKeyLocation.Header
				    })
			    }
		    };
			action?.Invoke(swaggerUiOwinSettings);

		    app.UseSwaggerUi(assembly, swaggerUiOwinSettings);
	    }
	}
}
