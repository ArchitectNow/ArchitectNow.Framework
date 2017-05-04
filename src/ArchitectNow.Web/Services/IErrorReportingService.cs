using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArchitectNow.Web.Services
{
	public interface IErrorReportingService
	{
		Task Send(Exception exception);
		Task Send(Exception exception, IList<string> tags);
		Task Send(Exception exception, IList<string> tags, IDictionary userCustomData);
	}
}