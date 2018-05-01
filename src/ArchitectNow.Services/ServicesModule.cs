using ArchitectNow.Services.Contexts;
using Autofac;

namespace ArchitectNow.Services
{
	public class ServicesModule : Module
    {
	    protected override void Load(ContainerBuilder builder)
	    {
		    builder.RegisterGeneric(typeof(DataContextService<,>)).As(typeof(IDataContextService<>));
	    }
    }
}
