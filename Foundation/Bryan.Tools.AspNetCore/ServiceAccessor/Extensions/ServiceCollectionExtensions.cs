using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using Tools.AspNetCore;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddServiceAccessor(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddSingleton<IStartupFilter, AccessorStartupFilter>();

        return services;
    }

}
