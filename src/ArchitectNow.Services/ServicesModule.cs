using ArchitectNow.Services.Caching;
using ArchitectNow.Services.Options;
using Autofac;

namespace ArchitectNow.Services
{
	public class ServicesModule : Module
    {
	    protected override void Load(ContainerBuilder builder)
	    {
		    builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces().PreserveExistingDefaults();
		    builder.RegisterType<BaseCacheService<CachingOptions>>().As<ICacheService>().SingleInstance().PreserveExistingDefaults();
	    }
    }
}
