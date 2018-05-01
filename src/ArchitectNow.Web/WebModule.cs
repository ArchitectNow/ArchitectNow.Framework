using ArchitectNow.Models.Security;
using ArchitectNow.Web.Filters;
using ArchitectNow.Web.Services;
using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ArchitectNow.Web
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceInvoker>().As<IServiceInvoker>().InstancePerLifetimeScope();
            builder.RegisterType<ExceptionResultBuilder>().As<IExceptionResultBuilder>().InstancePerLifetimeScope();
            
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

            builder.RegisterType<GlobalExceptionFilter>().AsSelf().InstancePerLifetimeScope();            
        }
    }
}