using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Synoptic
{
    internal static class ReflectionUtilities
    {
        public static IEnumerable<MethodInfo> RetrieveMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static IEnumerable<MethodInfo> RetrieveMethodsWithAttribute<TAttribute>(Type type) where TAttribute : Attribute
        {
            var methods = RetrieveMethods(type);

            foreach (MethodInfo method in methods)
            {
                var attribute = Attribute.GetCustomAttribute(method, typeof(TAttribute), false) as TAttribute;
                if(attribute != null)
                    yield return method;
            }
        }

        public static IEnumerable<Type> RetrieveTypesWithAttribute<TAttribute>(Assembly assembly) 
            where TAttribute : Attribute
        {
            return assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(TAttribute), true).Length > 0);
        }
    }
}
