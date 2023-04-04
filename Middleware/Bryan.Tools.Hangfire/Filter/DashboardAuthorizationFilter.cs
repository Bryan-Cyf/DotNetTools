using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Tools.Cache;

namespace Tools.Hangfire
{
    internal class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext dashboardContext)
        {
            bool isValidAuthorize = false;
            var httpContext = dashboardContext.GetHttpContext();

            var token = getTokenFromCookie(httpContext);
            if (!string.IsNullOrWhiteSpace(token))
            {
                //校验token有效性
                isValidAuthorize = check4Valid(token);
            }

            if (!isValidAuthorize)
            {
                var password = httpContext.Request.Query[HangfireConst.PasswordQuery].FirstOrDefault();
                if (password == HangfireConst.Password)
                {
                    var tokenValue = Guid.NewGuid().ToString();
                    httpContext.Response.Cookies.Append(HangfireConst.TokenKey, tokenValue, new CookieOptions() { Expires = DateTimeOffset.Now.AddDays(1) });
                    MemoryCacheHelper.Set(HangfireConst.TokenKey, tokenValue, TimeSpan.FromDays(1));
                    isValidAuthorize = true;
                }
            }

            return isValidAuthorize;
        }

        private string getTokenFromCookie(HttpContext httpContext)
        {
            var token = string.Empty;

            if (httpContext.Request != null && httpContext.Request.Cookies != null)
            {
                httpContext.Request.Cookies.TryGetValue(HangfireConst.TokenKey, out token);
            }

            return token;
        }

        private bool check4Valid(string token)
        {
            bool result;
            try
            {
                var cacheToken = MemoryCacheHelper.GetString(HangfireConst.TokenKey);
                result = cacheToken == token;
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }

}
