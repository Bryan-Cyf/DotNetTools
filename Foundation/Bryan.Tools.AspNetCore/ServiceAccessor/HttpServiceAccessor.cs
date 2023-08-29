using Microsoft.AspNetCore.Http;

namespace Tools.AspNetCore
{
    public static class HttpServiceAccessor
    {
        private static IHttpContextAccessor _accessor;

        public static HttpContext Current => _accessor.HttpContext;

        internal static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }
}
