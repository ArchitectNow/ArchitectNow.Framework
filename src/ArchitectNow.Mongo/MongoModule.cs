using ArchitectNow.Models.Options;
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
		    builder.RegisterType<MongoDataContextService>().As<IDataContextService<MongoDataContext>>()
			    .InstancePerLifetimeScope();

		    builder.Register(context => context.Resolve<IConfiguration>().CreateOptions<MongoOptions>("mongo")).As<IOptions<MongoOptions>>().SingleInstance();
		}
    }
}
