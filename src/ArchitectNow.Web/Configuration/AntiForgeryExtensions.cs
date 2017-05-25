using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ArchitectNow.Web.Configuration
{
	public static class AntiForgeryExtensions
	{
		public static void ConfigureAntiForgery(this IApplicationBuilder app, IAntiforgery antiforgery)
		{
			app.Use(next => context =>
			{
				if (string.Equals(context.Request.Path.Value, "/", StringComparison.OrdinalIgnoreCase) ||
				    string.Equals(context.Request.Path.Value, "/index.html", StringComparison.OrdinalIgnoreCase))
				{
					var tokens = antiforgery.GetAndStoreTokens(context);
					context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
						new CookieOptions { HttpOnly = false });
				}

				return next(context);
			});
		}
	}
}