using System;
using System.Collections.Generic;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.WebApi.Processors.Security;

namespace ArchitectNow.Web.Models
{
    public class SwaggerOptions
    {
	    public readonly string DefaultSwaggerRoute = "/docs/v1/swagger.json";
	    public readonly string DefaultSwaggerUiRoute = "/docs";
	    
	    public string Name { get; set; }
	    public string Version { get; set; }
	    public string Description { get; set; }
		public string Title { get; set; }
	    public string SwaggerRoute { get; set; } = "/docs/v1/swagger.json";
	    public string SwaggerUiRoute { get; set; } = "/docs";
	    public IEnumerable<Type> Controllers { get; set; }
	    public Action<SwaggerUiSettings> Configure { get; set; }
	    public IList<IDocumentProcessor> DocumentProcessors { get; set; } = new List<IDocumentProcessor>();

	    public SwaggerOptions(bool includeAuthorizationHeader = true)
	    {
		    if (includeAuthorizationHeader)
		    {
			    DocumentProcessors.Add(
				    new SecurityDefinitionAppender("Authorization", new SwaggerSecurityScheme
				    {
					    Type = SwaggerSecuritySchemeType.ApiKey,
					    Name = "Authorization",
					    In = SwaggerSecurityApiKeyLocation.Header
				    })
			    );
		    }
	    }
	}
}
