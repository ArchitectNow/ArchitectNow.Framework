using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Autofac;
using Autofac.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ArchitectNow.Web.Models.Security;
using ArchitectNow.Web.Filters;

namespace ArchitectNow.Web
{
    public class ApiModule : Module
    {
        private int _depth;

        protected override void Load(ContainerBuilder builder)
        {
            var assembly = ThisAssembly;
            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();

			builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

            builder.RegisterType<GlobalExceptionFilter>().AsSelf().InstancePerLifetimeScope();
			
	        builder.Register(context =>
		        {
			        var physicalAddress = NetworkInterface.GetAllNetworkInterfaces().First().GetPhysicalAddress();
			        var keyString = $"{physicalAddress}";
			        var keyBytes = Encoding.Unicode.GetBytes(keyString);
			        var signingKey = new JwtSigningKey(keyBytes);
			        return signingKey;
		        })
		        .AsSelf()
		        .SingleInstance();

	        builder.Register(context =>
	        {
		        var configurationRoot = context.Resolve<IConfigurationRoot>();
		        var issuerOptions = configurationRoot.GetSection("jwtIssuerOptions").Get<JwtIssuerOptions>();

		        var key = context.Resolve<JwtSigningKey>();

		        issuerOptions.SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
		        
		        return new OptionsWrapper<JwtIssuerOptions>(issuerOptions);
	        }).As<IOptions<JwtIssuerOptions>>().SingleInstance();
		}

		protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry,
                                                              IComponentRegistration registration)
        {
            registration.Preparing += RegistrationOnPreparing;
            registration.Activating += RegistrationOnActivating;
            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        private string GetPrefix()
        {
            return new string('-', _depth * 2);
        }

        private void RegistrationOnPreparing(object sender, PreparingEventArgs preparingEventArgs)
        {
            Console.WriteLine("{0}Resolving  {1}", GetPrefix(), preparingEventArgs.Component.Activator.LimitType);
            _depth++;
        }

        private void RegistrationOnActivating(object sender, ActivatingEventArgs<object> activatingEventArgs)
        {
            _depth--;
            Console.WriteLine("{0}Activating {1}", GetPrefix(), activatingEventArgs.Component.Activator.LimitType);
        }
    }
}