using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectNow.Web.Configuration
{
	public static class AutofacExtensions
	{
		public static IContainer CreateAutofacContainer(this IServiceCollection services, IConfigurationRoot configurationRoot, Action<ContainerBuilder> additionalAction, params Module[] modules)
		{
			var builder = new ContainerBuilder();

			builder.Register(ctx => configurationRoot).As<IConfigurationRoot>();

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