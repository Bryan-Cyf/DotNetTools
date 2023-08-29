using Microsoft.AspNetCore.Http;
using System;

namespace Tools.AspNetCore
{
    public static class ServiceProviderExtension
    {
        public static object GetService(this Type serviceType)
        {
            return HttpServiceAccessor.Current.RequestServices.GetService(serviceType);
        }

        public static T GetService<T>(this HttpContext context) where T : class
        {
            return context.RequestServices.GetService(typeof(T)) as T;
        }
    }
}
