using Microsoft.AspNetCore.Builder;

namespace ArchitectNow.Web.Middleware
{
	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseAntiforgery(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<AntiforgeryMiddleware>();
		}
	}
}