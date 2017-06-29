using ArchitectNow.Mongo.Models;
using ArchitectNow.Mongo.Options;
using ArchitectNow.Mongo.Services;
using ArchitectNow.Services.Contexts;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ArchitectNow.Mongo
{
	public class MongoModule: Module
    {
	    protected override void Load(ContainerBuilder builder)
	    {
		    builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();

		    builder.RegisterGeneric(typeof(MongoDataContextService)).As<IDataContextService<MongoDataContext>>()
			    .InstancePerLifetimeScope();
			
		    builder.Register(context =>
		    {
			    var configurationRoot = context.Resolve<IConfigurationRoot>();
			    var mongoOptions = configurationRoot.GetSection("mongo").Get<MongoOptions>();
				
			    return new OptionsWrapper<MongoOptions>(mongoOptions);
		    }).As<IOptions<MongoOptions>>().SingleInstance();
		}
    }
}
