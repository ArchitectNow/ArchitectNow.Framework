﻿using ArchitectNow.Models.Security;
using ArchitectNow.Web.Filters;
using ArchitectNow.Web.Services;
using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var issuerOptions = configuration.GetSection("jwtIssuerOptions").Get<JwtIssuerOptions>();

                var key = context.Resolve<JwtSigningKey>();
                if (key == null)
                {
                    context.Resolve<ILogger<WebModule>>().LogWarning("JwtSigningKey is not defined");
                }
                else
                {
                    issuerOptions.SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                }

                return new OptionsWrapper<JwtIssuerOptions>(issuerOptions);
            }).As<IOptions<JwtIssuerOptions>>().InstancePerLifetimeScope();
        }
    }
}