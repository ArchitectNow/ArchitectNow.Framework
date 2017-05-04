using System;
using System.Collections.Generic;
using System.IO;
using ArchitectNow.Web.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ArchitectNow.Web.Configuration
{
    public static class SwaggerExtensions
    {
	    public static void ConfigureSwagger(this IServiceCollection services, string name, Info info, Action<SwaggerGenOptions> setupAction= null)
	    {
		    services.ConfigureSwaggerGen(options =>
		    {
			    options.SwaggerDoc(name, info);
			    options.CustomSchemaIds(type => type.FullName);
			    options.DescribeStringEnumsInCamelCase();
				
			    setupAction?.Invoke(options);
		    });
	    }

	    public static void ConfigureSwagger(this IApplicationBuilder app, string description, Action<SwaggerUIOptions> setupAction = null)
	    {
		    app.UseSwagger(c =>
		    {
			    c.RouteTemplate = "docs/{documentName}/swagger.json";
		    });

		    app.UseSwaggerUI(options =>
		    {
			    options.SwaggerEndpoint("/docs/v1/swagger.json", description);

			    options.RoutePrefix = "docs";
			    //options.SwaggerEndpoint("/docs/v1/swagger.json", "V1 Docs");
			    options.EnabledValidator();
			    options.BooleanValues(new object[] { 0, 1 });
			    options.DocExpansion("list");
			    options.InjectOnCompleteJavaScript("/swagger-ui/on-complete.js");
			    options.InjectOnFailureJavaScript("/swagger-ui/on-failure.js");
			    options.SupportedSubmitMethods(new[] { "get", "post", "put", "patch", "delete" });
			    options.ShowRequestHeaders();
			    options.ShowJsonEditor();

				setupAction?.Invoke(options);
		    });
	    }
	}
}
