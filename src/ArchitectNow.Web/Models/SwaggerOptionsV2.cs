using System;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;
using NSwag.SwaggerGeneration.WebApi;

namespace ArchitectNow.Web.Models
{
	public class SwaggerOptionsV2 : SwaggerOptionsBase
	{
		public Action<SwaggerUiSettings<WebApiToSwaggerGeneratorSettings>> Configure { get; set; }

		public SwaggerOptionsV2(bool includeAuthorizationHeader = true)
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
