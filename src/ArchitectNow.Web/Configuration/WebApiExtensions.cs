using System;
using ArchitectNow.Web.Filters;
using ArchitectNow.Web.Models;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ArchitectNow.Web.Configuration
{
	public static class WebApiExtensions
	{
		public static void ConfigureApi(this IServiceCollection services, FluentValidationOptions fluentValidationOptions, Action<MvcOptions> configureMvc = null)
		{
			/*************************
             * IConfiguration is not available yet
             *************************/

			services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
			services.AddRouting(options => options.LowercaseUrls = true);
			var mvcBuilder = services.AddMvc(o =>
				{
					o.Filters.AddService(typeof(GlobalExceptionFilter));
					o.ModelValidatorProviders.Clear();
					configureMvc?.Invoke(o);
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
				});


			if (fluentValidationOptions.Enabled)
			{
				mvcBuilder.AddFluentValidation(configuration => fluentValidationOptions.Configure?.Invoke(configuration));
			}
		}
	}
}
