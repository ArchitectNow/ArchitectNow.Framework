using System;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;
using NSwag.SwaggerGeneration.WebApi;

namespace ArchitectNow.Web.Models
{
    public class SwaggerOptionsV3: SwaggerOptionsBase
    {
        public Action<SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>> Configure { get; set; }
	    
        public SwaggerOptionsV3(bool includeAuthorizationHeader = true)
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