using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Synoptic.Service;

namespace Synoptic.Tests
{
    [TestFixture]
    public class PublicInterfaceTests
    {
        readonly List<Type> _definedPublicTypes = new List<Type>
                                                    {
                                                        typeof (CommandRunner),
                                                        typeof (CommandInvocationException),
                                                        typeof (IDependencyResolver),
                                                        typeof (CommandException),
                                                        typeof (CommandParameterAttribute),
                                                        typeof (CommandAttribute),
                                                        typeof (IWindowsServiceConfiguration),
                                                        typeof (WindowsServiceConfiguration),
                                                        typeof (WindowsService),
                                                        typeof (CommandLineParseResult),
                                                        typeof (Command),
                                                        typeof (ParameterInfoWrapper),
                                                        typeof (CommandLineParameter),
                                                        typeof (IDaemon),
                                                        typeof (ComboDaemon),
                                                        typeof (LoopyDaemon),
                                                        typeof (SafeServiceController),
                                                        typeof (DaemonEvent),
                                                        typeof (UdpDaemon),
                                                        typeof (IUdpDaemonConfiguration),
                                                        typeof (ErrorEventArgs),
                                                        typeof (WindowsServiceEventArgs),
                                                        typeof (DaemonException),
                                                        typeof (UdpDaemonConfiguration),
                                                        typeof (TraceSourceExtensions)
                                                    };
        [Test]
        public void should_list_public_interfaces()
        {
            var publicTypes = new List<Type>();

            Assembly assembly1 = Assembly.GetAssembly(typeof(CommandRunner));
            Assembly assembly2 = Assembly.GetAssembly(typeof(IDaemon));

            foreach (var type in assembly1.GetTypes().Union(assembly2.GetTypes()))
            {
                if (!type.IsPublic) continue;
                if (!type.FullName.StartsWith("Synoptic.")) continue;

                publicTypes.Add(type);

                Console.WriteLine(GetTypeDefinitionAsString(type));
            }

            var isDefined = new List<Type>();
            foreach (var publicType in publicTypes)
            {
                Assert.That(_definedPublicTypes.Any(t => IsSameType(t, publicType)), "Type is not defined to be public : " + publicType.FullName);

                isDefined.Add(_definedPublicTypes.First(t => IsSameType(t, publicType)));
            }
            Assert.That(isDefined.Count == publicTypes.Count, "Missing types");
        }

        private static bool IsSameType(Type t1, Type t2)
        {
            return Regex.Replace(t1.FullName, @"`\d+.*", "") == Regex.Replace(t2.FullName, @"`\d+.*", "");
        }

        private static string GetArguments(MethodBase constructorInfo)
        {
            return string.Join(", ", constructorInfo.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name).ToArray());
        }

        private static string GetTypeDefinitionAsString(Type type)
        {
            var typeDefinition = new StringBuilder();

            string publicTypeName;
            if (type.IsGenericType)
            {
                var name = Regex.Replace(type.Name, @"`\d+", "");
                string gen = "";
                string s = string.Join(", ", type.GetGenericArguments().Select(g => g.Name).ToArray());
                if (!string.IsNullOrEmpty(s)) gen = "<" + s + ">";
                publicTypeName = string.Format("{0}{1}", name, gen);
            }
            else
            {
                publicTypeName = type.Name;
            }

            typeDefinition.AppendLine(publicTypeName);
            typeDefinition.AppendLine("{");

            foreach (var memberInfo in type.GetMembers())
            {
                if (memberInfo.DeclaringType != type) continue;
                if (memberInfo.MemberType == MemberTypes.Method && (memberInfo.Name.StartsWith("get_") || memberInfo.Name.StartsWith("set_"))) continue;

                if (memberInfo.MemberType == MemberTypes.Constructor)
                {
                    var constructorInfo = memberInfo as ConstructorInfo;
                    typeDefinition.AppendFormat("  {0}({1})\n", type.Name, GetArguments(constructorInfo));
                }
                else if (memberInfo.MemberType == MemberTypes.Method)
                {
                    var methodInfo = memberInfo as MethodInfo;

                    string gen = "";
                    string s = string.Join(", ", methodInfo.GetGenericArguments().Select(g => g.Name).ToArray());
                    if (!string.IsNullOrEmpty(s)) gen = "<" + s + ">";

                    typeDefinition.AppendFormat("  {0} {1}{2}({3})\n", methodInfo.ReturnType.Name, methodInfo.Name, gen, GetArguments(methodInfo));
                }
                else if (memberInfo.MemberType == MemberTypes.Property)
                {
                    var propertyInfo = memberInfo as PropertyInfo;

                    string getter = type.GetMethods().Any(m => m.Name == "get_" + memberInfo.Name) ? "get; " : "";
                    string setter = type.GetMethods().Any(m => m.Name == "set_" + memberInfo.Name) ? "set; " : ""; ;
                    typeDefinition.AppendFormat("  {0} {1} {{ {2}{3}}}\n", propertyInfo.PropertyType.Name, memberInfo.Name, getter, setter);
                }
                else
                {
                    typeDefinition.AppendFormat("  {0} [{1}]\n", memberInfo.Name, memberInfo.MemberType);
                }
            }

            typeDefinition.AppendLine("}");

            return typeDefinition.ToString();
        }
    }
}