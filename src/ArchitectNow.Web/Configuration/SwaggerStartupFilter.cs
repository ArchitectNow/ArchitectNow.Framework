using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ArchitectNow.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using NJsonSchema;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration;

namespace ArchitectNow.Web.Configuration
{
    public class SwaggerStartupFilter: IStartupFilter
    {
	    protected SwaggerOptions[] Options { get; }

	    public SwaggerStartupFilter(params SwaggerOptions[] options)
	    {
		 	Options = options;   
	    }

	    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
	    {
		    return builder =>
		    {
			    foreach (var option in Options)
			    {
				    var action = option.Configure;

				    var swaggerUiOwinSettings = new SwaggerUiSettings
				    {
					    DefaultPropertyNameHandling = PropertyNameHandling.CamelCase,
					    Title = option.Title,
					    SwaggerRoute = option.SwaggerRoute,
					    SwaggerUiRoute = option.SwaggerUiRoute,
					    UseJsonEditor = true,
					    FlattenInheritanceHierarchy = true,
					    IsAspNetCore = true
				    };
			    
				    foreach (var optionDocumentProcessor in option.DocumentProcessors)
				    {
					    swaggerUiOwinSettings.DocumentProcessors.Add(optionDocumentProcessor);
				    }

				    foreach (var operationProcessor in option.OperationProcessors)
				    {
					    swaggerUiOwinSettings.OperationProcessors.Add(operationProcessor);
				    }
			    
				    action?.Invoke(swaggerUiOwinSettings);
				    if (option.Controllers?.Any() == true)
				    {
					    builder.UseSwaggerUi(option.Controllers, swaggerUiOwinSettings,
						    new SwaggerJsonSchemaGenerator(swaggerUiOwinSettings));
				    }
				    else
				    {
					    builder.UseSwaggerUi(option.ControllerAssembly, swaggerUiOwinSettings);
				    }
			    }
		    };
	    }
    }
}
