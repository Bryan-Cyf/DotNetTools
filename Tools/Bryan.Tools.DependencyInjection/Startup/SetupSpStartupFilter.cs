using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Tools.Dependency
{
    internal class SetupSpStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return x =>
            {
                DiHelper._serviceProvider ??= x.ApplicationServices;
                next(x);
            };
        }
    }
}
