using Autofac;
using Microsoft.Extensions.Configuration;

namespace ArchitectNow.Caching
{
    public class CachingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CachingRegion>().As<ICachingRegion>().SingleInstance();
            builder.RegisterType<CachingService>().As<ICachingService>().SingleInstance();
            builder.RegisterType<CacheMangerFactory>().As<ICacheMangerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(CacheKeeper<>)).As(typeof(ICacheKeeper<>)).SingleInstance();

            builder.Register(context => context.Resolve<IConfigurationRoot>().CreateOptions<RedisOptions>("redis")).AsSelf().SingleInstance();
            
            builder.Register(context => context.Resolve<IConfigurationRoot>().CreateOptions<CachingOptions>("caching")).AsSelf().SingleInstance();
            
        }
    }
}