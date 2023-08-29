using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 查询配置项Section
        /// </summary>
        public static T GetSection<T>(this IServiceCollection services, string sectionName) where T : class
        {
            using ServiceProvider provider = services.BuildServiceProvider();
            IConfigurationSection section = (provider.GetRequiredService<IConfiguration>() ?? throw new ArgumentNullException("IConfiguration")).GetSection(sectionName);
            if (!section.Exists())
            {
                throw new Exception($"Config file not exist {sectionName} section.");
            }
            T option = section.Get<T>();
            if (option == null)
            {
                throw new Exception($"Get {sectionName} option from config file failed.");
            }
            return option;
        }
    }
}
