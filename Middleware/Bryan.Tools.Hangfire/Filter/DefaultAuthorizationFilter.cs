using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Tools.Hangfire
{
    internal class DefaultAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext dashboardContext)
        {
            return true;
        }
    }
}
