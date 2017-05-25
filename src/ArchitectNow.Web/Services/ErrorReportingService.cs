using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Mindscape.Raygun4Net;

namespace ArchitectNow.Web.Services
{
	public class ErrorReportingService : IErrorReportingService
	{
		private readonly RaygunClient _client;
		private readonly IHostingEnvironment _hostingEnvironment;

		public ErrorReportingService(RaygunClient client, IHostingEnvironment hostingEnvironment)
		{
			_client = client;
			_hostingEnvironment = hostingEnvironment;
		}

		public virtual Task Send(Exception exception)
		{
			return Send(exception, null);
		}

		public virtual Task Send(Exception exception, IList<string> tags)
		{
			if (tags == null)
			{
				tags = new List<string>();
			}

			tags.Add(_hostingEnvironment.EnvironmentName);

			return _client.SendInBackground(exception, tags);
		}

		public virtual Task Send(Exception exception, IList<string> tags, IDictionary userCustomData)
		{
			if (tags == null)
			{
				tags = new List<string>();
			}
			tags.Add(_hostingEnvironment.EnvironmentName);

			return _client.SendInBackground(exception, tags, userCustomData);
		}
	}
}