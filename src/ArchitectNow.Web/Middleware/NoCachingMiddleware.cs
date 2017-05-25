using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ArchitectNow.Web.Middleware
{
    public class NoCachingMiddleware
    {
        private readonly RequestDelegate _next;

        public NoCachingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                context.Response.Headers.Add("Expires", "-1");
            }

            await _next(context);
        }
    }
}
