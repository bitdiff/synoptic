using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Synoptic.Tests
{
    [TestFixture]
    public class CommandRunnerTests
    {
        [SetUp]
        public void setup()
        {
//            ConsoleFormatter.SetWriter(new TestConsoleWriter());
        }

        [Test]
        public void should_turn_command_names_to_hyphened_notation()
        {
            var command = new CommandFinder().FindInType(typeof(CommandRunnerTestClass));
            Assert.That(command.Name, Is.EqualTo("command-runner-test-class"));
        }

        [Test]
        public void should_turn_action_names_to_hyphened_notation()
        {
            bool hasRun = false;

            CommandRunnerTestClass.TestAction = (m, a) =>
                                        {
                                            hasRun = true;
                                            Assert.That(m.Name, Is.EqualTo("MultipleParamsToHyphen"));
                                            Assert.That(a.Length, Is.EqualTo(3));
                                            Assert.That(a[0], Is.EqualTo("one"));
                                            Assert.That(a[1], Is.EqualTo("two"));
                                            Assert.That(a[2], Is.EqualTo("three"));
                                        };

            new CommandRunner().WithCommandsFromType<CommandRunnerTestClass>().Run(new[]
                                                                           {
                                                                               "command-runner-test-class",
                                                                               "multiple-params-to-hyphen",
                                                                               "--param-one=one",
                                                                               "--param-two=two",
                                                                               "--param-three=three"
                                                                           });

            Assert.That(hasRun);
        }

        [Test]
        public void should_display_help_for_assembly()
        {
            new CommandRunner().Run(null);
        }

        [Test]
        public void should_handle_bool()
        {
            new CommandRunner().WithCommandsFromType<CommandRunnerTestClass>().Run(new[] { "CommandWithBool", "--param1" });
            new CommandRunner().WithCommandsFromType<CommandRunnerTestClass>().Run(new[] { "CommandWithBool" });
        }

        [Command]
        internal class CommandRunnerTestClass
        {
            public volatile static Action<MethodBase, object[]> TestAction = (m, a) => { };

            [CommandAction]
            public void TestCommand(string param1)
            {
                Dump(param1);
            }

            [CommandAction]
            public void CommandWithBool(bool param1)
            {
                Dump(param1);
            }

            [CommandAction]
            public void MultipleParamsToHyphen(string paramOne, string paramTwo, string paramThree)
            {
                Dump(paramOne, paramTwo, paramThree);
            }

            private void Dump(params object[] args)
            {
                var stackTrace = new StackTrace();
                var frames = stackTrace.GetFrames();

                if (frames == null)
                {
                    Assert.Fail("Method was not called.");
                }
                else
                {
                    MethodBase caller = frames.Skip(1).First().GetMethod();
                    Console.WriteLine();
                    Console.WriteLine("Command: {0}", caller.Name);
                    int i = 0;
                    foreach (var parameterInfo in caller.GetParameters())
                        Console.WriteLine("  {0}={1}", parameterInfo.Name, args[i++]);

                    TestAction(caller, args);
                    return;
                }
            }
        }
    }
}