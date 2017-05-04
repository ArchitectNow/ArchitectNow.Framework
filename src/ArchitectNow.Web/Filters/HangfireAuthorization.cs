using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace ArchitectNow.Web.Filters
{
    public class HangfireAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
