using Autofac;

namespace ArchitectNow.Web.Configuration
{
	public static class AutofacExtensions
	{
		public static void CreateAutofacContainer(this ContainerBuilder container, params Module[] modules)
		{
			container.RegisterModule<WebModule>();
			foreach (var module in modules)
			{
				container.RegisterModule(module);
			}
		}
	}
}