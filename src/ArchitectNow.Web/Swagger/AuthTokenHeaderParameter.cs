using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ArchitectNow.Web.Swagger
{
    public class AuthorizationHeaderParameter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
	        var descriptor = context.ApiDescription.ActionDescriptor;
	        var hasAllowAnonymousAttribute = descriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
	        if (hasAllowAnonymousAttribute)
	        {
		        return;
	        }

	        if (operation.Parameters == null)
	        {
		        operation.Parameters = new List<IParameter>();
	        }

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Authorization",
                In = "header",
                Type = "string",
                Required = false
            });
        }
    }
}
