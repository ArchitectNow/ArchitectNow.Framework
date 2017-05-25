using System;
using System.Linq;
using System.Threading.Tasks;
using ArchitectNow.Web.Models.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace ArchitectNow.Web.Configuration
{
	public static class JwtExtensions
	{
		public static void ConfigureJwt(this IApplicationBuilder app, IConfigurationRoot configurationRoot)
		{
			var jwtAppSettingOptions = configurationRoot.GetSection(nameof(JwtIssuerOptions));
			var signingKey = app.ApplicationServices.GetService<JwtSigningKey>();

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

				ValidateAudience = true,
				ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = signingKey,

				RequireExpirationTime = true,
				ValidateLifetime = true,

				ClockSkew = TimeSpan.Zero
			};

			app.UseJwtBearerAuthentication(new JwtBearerOptions
			{
				AutomaticAuthenticate = true,
				AutomaticChallenge = true,
				TokenValidationParameters = tokenValidationParameters,
				Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var task = Task.Run(() =>
						{
							if (context.Request.Query.TryGetValue("securityToken", out StringValues securityToken))
							{
								context.Token = securityToken.FirstOrDefault();
							}
						});

						return task;
					}
				}
			});

		}
	}
}