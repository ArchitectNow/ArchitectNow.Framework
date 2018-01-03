using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace ArchitectNow.Caching
{
    public class CachingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<CachingService>().As<ICachingService>().SingleInstance();
            builder.RegisterType<CacheMangerFactory>().As<ICacheMangerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(CacheKeeper<>)).As(typeof(ICacheKeeper<>)).SingleInstance();

            builder.Register(context =>
            {
                var configurationRoot = context.Resolve<IConfigurationRoot>();
                
                var connectionString = configurationRoot["redis:connectionString"];
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Missing redis connection string.");
                }
                var configurationOptions = ConfigurationOptions.Parse(connectionString);

                return ConnectionMultiplexer.Connect(configurationOptions);
            }).As<IConnectionMultiplexer>().SingleInstance();
        }
    }
}