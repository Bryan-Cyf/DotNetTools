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
        /// 查询配置项Option
        /// </summary>
        public static T GetOption<T>(this IServiceCollection services, string sectionName) where T : class
        {
            var section = services.GetSection(sectionName);
            T option = section.Get<T>();
            if (option == null)
            {
                throw new Exception($"Get {sectionName} option from config file failed.");
            }
            return option;
        }

        /// <summary>
        /// 查询配置项Section
        /// </summary>
        public static IConfigurationSection GetSection(this IServiceCollection services, string sectionName)
        {
            using ServiceProvider provider = services.BuildServiceProvider();
            IConfigurationSection section = (provider.GetRequiredService<IConfiguration>() ?? throw new ArgumentNullException("IConfiguration")).GetSection(sectionName);
            if (!section.Exists())
            {
                throw new Exception($"Config file not exist {sectionName} section.");
            }
            return section;
        }
    }
}
