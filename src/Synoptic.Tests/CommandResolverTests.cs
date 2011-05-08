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
            CommandActionManifest actionManifest = new CommandActionActionFinder().FindInType(typeof(Test1));

            CommandAction commandAction = new CommandResolver().Resolve(actionManifest, "my-command");

            Assert.IsNotNull(commandAction);
            Assert.That(commandAction.Name, Is.EqualTo("MyCommand"));
        }

        public class Test1
        {
            [CommandAction]
            public void MyCommand()
            {
            }
        }
    }
}
