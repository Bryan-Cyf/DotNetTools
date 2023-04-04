using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNLog(this IServiceCollection services, string configFileRelativePath = "", LogLevel minLevel = LogLevel.Trace)
        {
            return services.AddLogging(loggingBuilder =>
             {
                 loggingBuilder.ClearProviders();
                 loggingBuilder.SetMinimumLevel(minLevel);
                 loggingBuilder.AddNLog(configFileRelativePath);
             });
        }
    }
}
