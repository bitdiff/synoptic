using System.Collections.Generic;
using NUnit.Framework;

namespace Synoptic.Tests
{

    [TestFixture]
    public class CommandResolverTests
    {
        [Test]
        public void shoud_resolve_command()
        {
            CommandManifest manifest = new CommandFinder().FindInType(typeof(Test1));

            Command command = new CommandResolver().Resolve(manifest, "my-command");

            Assert.IsNotNull(command);
            Assert.That(command.Name, Is.EqualTo("MyCommand"));
        }

        public class Test1
        {
            [Command]
            public void MyCommand()
            {
            }
        }
    }
}
