using System.Linq;
using NUnit.Framework;

namespace Synoptic.Tests
{
    [TestFixture]
    public class CommandActionFinderTests
    {
        [Test]
        public void should_find_actions_in_commands()
        {
            var command = new CommandFinder().FindInType(typeof(CommandActionFinderTestCommand));
            Assert.That(command, Is.Not.Null);

            var actions = new CommandActionFinder().FindInCommand(command);
            Assert.That(actions.Count(), Is.EqualTo(2));
        }

        [Command]
        internal class CommandActionFinderTestCommand
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