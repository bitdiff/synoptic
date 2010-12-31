using System;
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

                Console.WriteLine("public Type [{0}]", type.FullName);
                foreach (var memberInfo in type.GetMembers())
                {
                    if (memberInfo.DeclaringType == type)
                        Console.WriteLine("  public {0} [{1}]", memberInfo.MemberType, memberInfo.Name);
                }
            }
        }
    }
}