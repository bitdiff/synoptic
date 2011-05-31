using NUnit.Framework;

namespace Synoptic.Tests
{
    [TestFixture]
    public class CommandFinderTests
    {
        [Test]
        public void should_find_commands_in_type()
        {
            var command = new CommandFinder().FindInType(typeof(CommandFinderTestCommand));
            Assert.That(command, Is.Not.Null);
            Assert.That(command.Name, Is.EqualTo("command-finder-test-command"));
        }

        [Command]
        internal class CommandFinderTestCommand
        {
            [CommandAction]
            public void MyAction(string param1)
            {
            }
 
            [CommandAction]
            public void MyOtherAction(string param1)
            {
            }
        }
    }
}