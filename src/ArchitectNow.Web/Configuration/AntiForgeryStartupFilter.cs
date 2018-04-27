using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Configuration
{
	public class AntiForgeryStartupFilter: IStartupFilter
	{
		public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
		{
			return builder =>
			{
				var antiforgery = builder.ApplicationServices.GetService<IAntiforgery>();
				builder.Use(n => context =>
				{
					if (string.Equals(context.Request.Path.Value, "/", StringComparison.OrdinalIgnoreCase) ||
					    string.Equals(context.Request.Path.Value, "/index.html", StringComparison.OrdinalIgnoreCase))
					{
						var tokens = antiforgery.GetAndStoreTokens(context);
						context.Response.Cookies.Append("X-XSRF-TOKEN", tokens.RequestToken,
							new CookieOptions { HttpOnly = false });
					}

					return n(context);
				});
				next(builder);
			};
		}
	}
}