using ArchitectNow.Mongo.Db;
using ArchitectNow.Mongo.Options;
using ArchitectNow.Mongo.Services;
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

			builder.RegisterType<DataContextService>().As<IDataContextService<DataContext>>().InstancePerLifetimeScope();

		    builder.Register(context =>
		    {
			    var configurationRoot = context.Resolve<IConfigurationRoot>();
			    var mongoOptions = configurationRoot.GetSection("mongo").Get<MongoOptions>();
				
			    return new OptionsWrapper<MongoOptions>(mongoOptions);
		    }).As<IOptions<MongoOptions>>().SingleInstance();
		}
    }
}
