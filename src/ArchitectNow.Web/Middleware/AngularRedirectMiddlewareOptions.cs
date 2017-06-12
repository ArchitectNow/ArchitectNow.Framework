using System;
using Microsoft.AspNetCore.Http;

namespace ArchitectNow.Web.Middleware
{
	public class AngularRedirectMiddlewareOptions
	{
		private Func<HttpContext, bool> _checkForPath;

		private readonly Func<HttpContext, bool> _checkForPathDefault;

		public Func<HttpContext, bool> CheckForPath
		{
			get => _checkForPath ?? _checkForPathDefault;
			set => _checkForPath = value;
		}

		public AngularRedirectMiddlewareOptions()
		{
			_checkForPathDefault = httpContext => httpContext.Request.Path.StartsWithSegments("/app") &&
			                                      !httpContext.Request.Path.StartsWithSegments("/hangfire");
		}
	}
}