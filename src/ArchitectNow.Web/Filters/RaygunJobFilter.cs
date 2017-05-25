using ArchitectNow.Web.Services;
using Hangfire.Common;
using Hangfire.States;

namespace ArchitectNow.Web.Filters
{
    public class RaygunJobFilter: JobFilterAttribute, IElectStateFilter
    {
        private readonly IErrorReportingService _errorReportingService;

        public RaygunJobFilter(IErrorReportingService errorReportingService)
        {
            _errorReportingService = errorReportingService;
        }

        public void OnStateElection(ElectStateContext context)
        {
            var failedState = context.CandidateState as FailedState;

            if (failedState != null)
            {
                _errorReportingService.Send(failedState.Exception);
            }
        }
    }
}
