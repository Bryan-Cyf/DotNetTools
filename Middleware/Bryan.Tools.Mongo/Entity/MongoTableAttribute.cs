using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Tools.Mongo
{
    public class MongoTableAttribute : Attribute
    {
        public string Name { get; set; }

        public MongoTableAttribute(string name)
        {
            Name = name;
        }
    }

    public static class CollectionHelper
    {
        public static string GetCollectionName<T>() where T : MongoEntity
        {
            return typeof(T).GetCustomAttribute<MongoTableAttribute>()?.Name ?? typeof(T).Name.ToLower();
        }
    }
}
