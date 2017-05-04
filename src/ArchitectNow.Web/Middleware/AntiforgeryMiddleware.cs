using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace ArchitectNow.Web.Middleware
{
	public class AntiforgeryMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IAntiforgery _antiforgery;

		public AntiforgeryMiddleware(RequestDelegate next, IAntiforgery antiforgery)
		{
			_next = next;
			_antiforgery = antiforgery;
		}

		public Task Invoke(HttpContext context, Func<HttpContext, bool> checkForPath = null)
		{
			if (checkForPath == null)
			{
				checkForPath = httpContext => string.Equals(context.Request.Path.Value, "/", StringComparison.OrdinalIgnoreCase) ||
				                              string.Equals(context.Request.Path.Value, "/index.html",
					                              StringComparison.OrdinalIgnoreCase);
			}

			if (checkForPath(context))
			{
				var tokens = _antiforgery.GetAndStoreTokens(context);
				context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
					new CookieOptions { HttpOnly = false });
			}

			return _next(context);
		}
	}
}