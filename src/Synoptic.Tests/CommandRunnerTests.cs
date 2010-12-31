using System;
using ConsoleWrapper.Synoptic;
using NUnit.Framework;

namespace ConsoleWrapper.Test.Synoptic
{
    [TestFixture]
    public class CommandRunnerTests
    {
        [Test]
        public void should_display_help_for_one_class()
        {
            new CommandRunner().WithCommand<RunnerTest1>().Run(null);
        }
        
        [Test]
        public void should_display_help_for_assembly()
        {
            new CommandRunner().Run(null);
        }

        [Test]
        public void should_handle_bool()
        {
            new CommandRunner().WithCommand<RunnerTest1>().Run(new[] { "CommandWithBool", "--param1" });
            new CommandRunner().WithCommand<RunnerTest1>().Run(new[] { "CommandWithBool" });
        }

        public class RunnerTest1
        {
            [Command]
            public void TestCommand(string param1)
            {
                Console.WriteLine();
                Console.WriteLine("TestCommand");
                Console.WriteLine("  param1={0}", param1);
            }

            [Command]
            public void CommandWithBool(bool param1)
            {
                Console.WriteLine();
                Console.WriteLine("CommandWithBool");
                Console.WriteLine("  param1={0}", param1);
            }
        }
    }
}