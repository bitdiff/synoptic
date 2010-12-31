using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConsoleWrapper.Synoptic
{
    public static class ReflectionUtilities
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
    }
}
