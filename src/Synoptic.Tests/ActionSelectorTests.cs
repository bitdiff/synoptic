using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Synoptic.Exceptions;

namespace Synoptic.Tests
{
    [TestFixture]
    public class ActionSelectorTests
    {
        private IEnumerable<CommandAction> GetTestActions()
        {
            var command = new CommandFinder().FindInType(typeof(ActionSelectorTestCommand));
            return new CommandActionFinder().FindInCommand(command);
        }

        [Test]
        public void should_select_action_when_single_action_name_matches()
        {
            var actions = GetTestActions();
            var commandAction = new ActionSelector().Select("my-action", null, actions);
            Assert.That(commandAction.Name, Is.EqualTo("my-action"));
        }

        [Test]
        [ExpectedException(typeof(CommandActionNotFoundException))]
        public void should_throw_execption_when_action_name_does_not_match()
        {
            var actions = GetTestActions();
            new ActionSelector().Select("not-exist", null, actions);
        }

        [Test]
        public void should_populate_exception_when_action_name_partially_matches_available_actions()
        {
            var actions = GetTestActions();

            try
            {
                new ActionSelector().Select("my", null, actions);
                Assert.Fail();
            }
            catch (CommandActionNotFoundException exception)
            {
                Assert.That(exception.PossibleActions.Count.Equals(2));
                Assert.That(exception.AvailableActions.Count, Is.EqualTo(actions.Count()));
            }
        }

        [Command]
        internal class ActionSelectorTestCommand
        {
            [CommandAction]
            public void MyAction() { }

            [CommandAction]
            public void MyOtherAction() { }
        }
    }
}
