using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Synoptic.Tests
{
    [TestFixture]
    public class PublicInterfaceTests
    {
        [Test]
        public void should_list_public_interfaces()
        {
            Assembly assembly = Assembly.GetAssembly(typeof (CommandRunner));

            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsPublic) continue;
                if (!type.FullName.StartsWith("Synoptic.")) continue;

                Console.WriteLine(type.FullName);
                Console.WriteLine("{");

                foreach (var memberInfo in type.GetMembers())
                {
                    if (memberInfo.DeclaringType != type) continue;
                    if (memberInfo.MemberType == MemberTypes.Method && (memberInfo.Name.StartsWith("get_") || memberInfo.Name.StartsWith("set_"))) continue;

                    if (memberInfo.MemberType == MemberTypes.Constructor)
                    {
                        var constructorInfo = memberInfo as ConstructorInfo;
                        Console.WriteLine("  {0}({1})", type.Name, GetS(constructorInfo));
                    }
                    else if (memberInfo.MemberType == MemberTypes.Method)
                    {
                        var methodInfo = memberInfo as MethodInfo;

                        string gen = "";
                        string s = string.Join(", ", methodInfo.GetGenericArguments().Select(g => g.Name).ToArray());
                        if (!string.IsNullOrEmpty(s)) gen = "<" + s + ">";

                        Console.WriteLine("  {0} {1}{2}({3})", methodInfo.ReturnType.Name, methodInfo.Name, gen, GetS(methodInfo));
                    }
                    else if (memberInfo.MemberType == MemberTypes.Property)
                    {
                        var propertyInfo = memberInfo as PropertyInfo;
                        
                        string getter = type.GetMethods().Any(m => m.Name == "get_" + memberInfo.Name) ? "get; " : "";
                        string setter = type.GetMethods().Any(m => m.Name == "set_" + memberInfo.Name) ? "set; " : ""; ;
                        Console.WriteLine("  {0} {1} {{ {2}{3}}}", propertyInfo.PropertyType.Name, memberInfo.Name, getter, setter);
                    }
                    else
                    {
                        Console.WriteLine("  {0} [{1}]", memberInfo.Name, memberInfo.MemberType);
                    }
                }

                Console.WriteLine("}");
                Console.WriteLine();
            }
        }

        private string GetS(MethodBase constructorInfo)
        {
            return string.Join(", ", constructorInfo.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name).
                                         ToArray());
        }
    }
}