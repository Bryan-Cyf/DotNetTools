using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Tools
{
    public class EnumHelper
    {
        public static List<string> GetList<T>()
        {
            return Enum.GetNames(typeof(T)).ToList();
        }
    }
}
