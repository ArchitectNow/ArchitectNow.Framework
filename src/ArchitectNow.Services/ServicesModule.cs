using Autofac;

namespace ArchitectNow.Services
{
	public class ServicesModule : Module
    {
	    protected override void Load(ContainerBuilder builder)
	    {
		    builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();
	    }
    }
}
