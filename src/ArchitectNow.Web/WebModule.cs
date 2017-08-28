using System;
using System.Text;
using ArchitectNow.Models.Security;
using ArchitectNow.Web.Filters;
using Autofac;
using Autofac.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mindscape.Raygun4Net;

namespace ArchitectNow.Web
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = ThisAssembly;
            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().PreserveExistingDefaults();

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<RaygunJobFilter>().AsSelf().InstancePerLifetimeScope().PreserveExistingDefaults();

            builder.RegisterType<GlobalExceptionFilter>().AsSelf().InstancePerLifetimeScope()
                .PreserveExistingDefaults();

            builder.Register(context =>
                {
                    var configurationRoot = context.Resolve<IConfigurationRoot>();
                    var issuerOptions = configurationRoot.GetSection("jwtIssuerOptions").Get<JwtIssuerOptions>();

                    var keyString = issuerOptions.Audience;
                    var keyBytes = Encoding.Unicode.GetBytes(keyString);
                    var signingKey = new JwtSigningKey(keyBytes);
                    return signingKey;
                })
                .AsSelf()
                .SingleInstance()
                .PreserveExistingDefaults();

            builder.Register(context =>
            {
                var configurationRoot = context.Resolve<IConfigurationRoot>();
                var issuerOptions = configurationRoot.GetSection("jwtIssuerOptions").Get<JwtIssuerOptions>();

                var key = context.Resolve<JwtSigningKey>();

                issuerOptions.SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                return new OptionsWrapper<JwtIssuerOptions>(issuerOptions);
            }).As<IOptions<JwtIssuerOptions>>().InstancePerLifetimeScope().PreserveExistingDefaults();

            builder.Register(context =>
            {
                var configurationRoot = context.Resolve<IConfigurationRoot>();
                var key = configurationRoot["RaygunSettings:ApiKey"];
                var raygunClient = new RaygunClient(key);
                return raygunClient;
            }).AsSelf().PreserveExistingDefaults();
        }
    }
}