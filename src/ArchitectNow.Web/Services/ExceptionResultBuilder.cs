using System;
using ArchitectNow.Models.Exceptions;
using Autofac.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ArchitectNow.Web.Services
{
    public class ExceptionResultBuilder : IExceptionResultBuilder
    {
	    private readonly IHostingEnvironment _hostingEnvironment;

        public ExceptionResultBuilder(IHostingEnvironment hostingEnvironment)
        {
	        _hostingEnvironment = hostingEnvironment;
        }
        public ObjectResult Build(Exception exception)
        {
            var stackTrace = "No stack trace available";

	        if (!string.Equals(_hostingEnvironment.EnvironmentName, "Production", StringComparison.OrdinalIgnoreCase))
            {
                stackTrace = exception.GetBaseException().StackTrace;
            }
            var statusCode = 500;
            object content = null;
            var message = exception.GetBaseException().Message;

            var dependencyResolutionException = exception as DependencyResolutionException;
            if (dependencyResolutionException != null)
            {
                message = $"Dependency Exception: Please ensure that classes implement the interface: {message}";
            }

            var apiException = exception as ApiException;

            if (apiException != null)
            {
                statusCode = (int)apiException.StatusCode;
                content = apiException.GetContent();
                if (!string.IsNullOrEmpty(apiException.Message))
                {
                    message = apiException.GetBaseException().Message;
                }
                stackTrace = null;
            }
			
            dynamic response = new
            {
                Message = message,
                Content = content,
                StackTrace = stackTrace
            };
            
            var objectResult = new ObjectResult(content ?? message)
            {
	            StatusCode = statusCode
            };

            return objectResult;
        }
    }
}