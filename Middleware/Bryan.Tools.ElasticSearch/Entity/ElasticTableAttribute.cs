using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Tools.Elastic
{
    public class ElasticTableAttribute : Attribute
    {
        public string Name { get; set; }

        public ElasticTableAttribute(string name)
        {
            Name = name;
        }
    }

    public static class IndexHelper
    {
        public static string GetIndexName<T>() where T : ElasticEntity
        {
            return typeof(T).GetCustomAttribute<ElasticTableAttribute>()?.Name ?? typeof(T).Name.ToLower();
        }
    }
}
