using System.IO;
using ArchitectNow.Web.Filters;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ArchitectNow.Web.Configuration
{
	public static class WebApiExtensions
	{
		public static void ConfigureApi(this IServiceCollection services)
		{
			/*************************
             * IConfiguration is not available yet
             *************************/

			services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

			services.AddMvc(o =>
				{
					o.Filters.AddService(typeof(GlobalExceptionFilter));
					o.ModelValidatorProviders.Clear();

					var policy = new AuthorizationPolicyBuilder()
						.RequireAuthenticatedUser()
						.Build();

					o.Filters.Add(new AuthorizeFilter(policy));
				})
				.AddJsonOptions(options =>
				{
					var settings = options.SerializerSettings;

					var camelCasePropertyNamesContractResolver = new CamelCasePropertyNamesContractResolver();

					settings.ContractResolver = camelCasePropertyNamesContractResolver;
					settings.Converters = new JsonConverter[]
					{
						new IsoDateTimeConverter(),
						new StringEnumConverter(true)
					};
				}).AddFluentValidation();
		}

		public static void ConfigureAssets(this IApplicationBuilder app, IConfigurationRoot configurationRoot)
		{
			app.UseFileServer();

			var uploadsPath = configurationRoot["uploadsPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
			if (!Directory.Exists(uploadsPath))
			{
				Directory.CreateDirectory(uploadsPath);
			}
			app.UseStaticFiles();
		}
	}
}
