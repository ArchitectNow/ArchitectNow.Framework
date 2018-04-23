using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Configuration
{
	public static class AutofacExtensions
	{
		public static IContainer CreateAutofacContainer(this IServiceCollection services, Action<ContainerBuilder> additionalAction, params Module[] modules)
		{
			var builder = new ContainerBuilder();
			
			builder.RegisterModule<WebModule>();
			foreach (var module in modules)
			{
				builder.RegisterModule(module);
			}

			additionalAction?.Invoke(builder);

			builder.Populate(services);
			
			return builder.Build();
		}
	}
}