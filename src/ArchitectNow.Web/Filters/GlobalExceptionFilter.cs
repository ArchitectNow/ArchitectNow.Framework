using System;
using ArchitectNow.Web.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace ArchitectNow.Web.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IExceptionResultBuilder _exceptionResultBuilder;

        public GlobalExceptionFilter(IConfiguration configuration, IExceptionResultBuilder exceptionResultBuilder)
        {
            _configuration = configuration;
            _exceptionResultBuilder = exceptionResultBuilder;
        }

        public void Dispose()
        {

        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            var result = _exceptionResultBuilder.Build(exception);

            context.Result = result;
        }
    }
}
