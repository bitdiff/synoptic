using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Synoptic.ConsoleFormat;

namespace Synoptic.Tests
{
    [TestFixture]
    public class CommandRunnerTests
    {
        [Test]
        public void should_display_help_for_one_class()
        {
            ConsoleTable t  = new ConsoleTable();
            var row = new ConsoleRow();
            var cell = new ConsoleCell()
                           {
                               Text = "sad sa dasd a dasd ad a dsa das d sad"
                           };

//            row.AddCell(cell);
//            t.AddRow(row);

            ConsoleFormatter.Write(t);
            
//            new CommandRunner().WithCommandsFromType<RunnerTest>().Run(null);
        }

        [Test]
        public void should_turn_command_names_to_hyphened_notation()
        {
            bool hasRun = false;
            RunnerTest.TestAction = (m, a) =>
                                        {
                                            hasRun = true;
                                            Assert.That(m.Name, Is.EqualTo("MultipleParamsToHyphen"));
                                            Assert.That(a.Length, Is.EqualTo(3));
                                            Assert.That(a[0], Is.EqualTo("one"));
                                            Assert.That(a[1], Is.EqualTo("two"));
                                            Assert.That(a[2], Is.EqualTo("three"));
                                        };

//            new CommandRunner().WithCommandsFromType<RunnerTest>().Run(new[] {
//                "multiple-params-to-hyphen",
//                "--param-one=one", 
//                "--param-two=two",
//                "--param-three=three"
//            });
//
//            Assert.That(hasRun);
        }

        [Test]
        public void should_display_help_for_assembly()
        {
            new CommandRunner().Run(null);
        }

        [Test]
        public void should_allow_custom_command_set_creation()
        {
//            new CommandRunner().WithCommandSet<RunnerTest>(p => new RunnerTest(p)).Run(new[] { "CommandWithBool", "--param1", "someotherparam" });
        }

        [Test]
        public void should_allow_args_pre_processor()
        {
//            new CommandRunner().WithArgsPreProcessor(args =>
//                                                         {
//                                                             Console.WriteLine(args.Count());
//                                                             Console.WriteLine("Took first param out: " + args[0]);
//                                                             return args.Skip(1).ToArray();
//                                                         }).Run(new[] { "CommandWithBool", "some first param", "--param1", "someotherparam" });
        }

        [Test]
        public void should_handle_bool()
        {
//            new CommandRunner().WithCommandsFromType<RunnerTest>().Run(new[] { "CommandWithBool", "--param1" });
//            new CommandRunner().WithCommandsFromType<RunnerTest>().Run(new[] { "CommandWithBool" });
        }

        internal class RunnerTest
        {
            public volatile static Action<MethodBase, object[]> TestAction = (m, a) => { };

            public RunnerTest(CommandLineParseResult commandLineParseResult)
            {
            }

            public RunnerTest()
            {
            }

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
                MethodBase caller = stackTrace.GetFrames().Skip(1).First().GetMethod();

                Console.WriteLine();
                Console.WriteLine("Command: {0}", caller.Name);
                int i = 0;
                foreach (var parameterInfo in caller.GetParameters())
                    Console.WriteLine("  {0}={1}", parameterInfo.Name, args[i++]);

                TestAction(caller, args);
            }
        }
    }
}