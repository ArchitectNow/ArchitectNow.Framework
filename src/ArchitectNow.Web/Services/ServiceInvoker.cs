using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ArchitectNow.Web.Services
{
	class ServiceInvoker : IServiceInvoker
    {
        private readonly IExceptionResultBuilder _exceptionResultBuilder;

        public ServiceInvoker(IExceptionResultBuilder exceptionResultBuilder)
        {
            _exceptionResultBuilder = exceptionResultBuilder;
        }

        public virtual async Task<IActionResult> AsyncOk<TResult>(Func<Task<TResult>> serviceCall)
        {
            try
            {
                var result = await serviceCall();

                return new OkObjectResult(result);
            }
            catch (Exception exception)
            {
                return _exceptionResultBuilder.Build(exception);
            }
        }

        public virtual async Task<IActionResult> AsyncOk(Func<Task> serviceCall)
        {
            try
            {
                await serviceCall();

                return new OkResult();
            }
            catch (Exception exception)
            {
                return _exceptionResultBuilder.Build(exception);
            }
        }

        public virtual async Task<IActionResult> AsyncOkNoContent(Func<Task> serviceCall)
        {
            try
            {
                await serviceCall();

                return new NoContentResult();
            }
            catch (Exception exception)
            {
                return _exceptionResultBuilder.Build(exception);
            }
        }

        public virtual async Task<IActionResult> AsyncOkNotFound<TResult>(Func<Task<TResult>> serviceCall)
        {
            try
            {
                var result = await serviceCall();

                if (EqualityComparer<TResult>.Default.Equals(result, default(TResult)))
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(result);
            }
            catch (Exception exception)
            {
                return _exceptionResultBuilder.Build(exception);
            }
        }
    }
}
