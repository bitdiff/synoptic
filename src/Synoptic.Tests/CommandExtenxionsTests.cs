using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Synoptic.Tests
{
    [TestFixture]
    public class CommandExtenxionsTests
    {
        [Test]
        public void shoud_run_command()
        {
            CommandManifest manifest = new CommandFinder().FindInType(typeof(Test1));
            var parseResult = new CommandLineParseResult(new List<CommandLineParameter> { new CommandLineParameter("param1", "testParam1Value"), new CommandLineParameter("ctorParam", "ctorParamValue") }, new[] { "my-command" });
            var commandName = CommandNameExtractor.Extract(new[] { parseResult });

            Command command = new CommandResolver().Resolve(manifest, commandName);

            string testResult = "fail";
            Test1.TestAction = s => testResult = s;

            command.Run(parseResult);

            Assert.That(testResult, Is.EqualTo("ctorParamValuetestParam1Value"));
        }

        [Test]
        public void shoud_run_command_with_non_primative_parameters()
        {
            CommandManifest manifest = new CommandFinder().FindInType(typeof(Test1));

            var parseResult = new CommandLineParseResult(new List<CommandLineParameter>
                                                             {
                                                                 new CommandLineParameter("ctorParam", "ctorParamValue"),
                                                                 new CommandLineParameter("param1", "testParam1Value"),
                                                                 new CommandLineParameter("param2", "2"),
                                                                 new CommandLineParameter("MoreParam1", "MoreParam1Value"),
                                                                 new CommandLineParameter("MoreParam2", "4"),
                                                             }, new[] { "MyCommandWithMoreParams" });

            var commandName = CommandNameExtractor.Extract(new[] { parseResult });
            Command command = new CommandResolver().Resolve(manifest, commandName);

            string testResult = "fail";
            Test1.TestAction = s => testResult = s;

            command.Run(parseResult);

            Assert.That(testResult, Is.EqualTo("ctorParamValuetestParam1Value2MoreParam1Value4"));
        }

        public class Test1
        {
            private readonly string _ctorParam;

            public Test1(string ctorParam)
            {
                _ctorParam = ctorParam;
            }

            public static Action<string> TestAction = s => { };
            [Command]
            public void MyCommand(string param1)
            {
                TestAction(_ctorParam + param1);
            }

            [Command]
            public void MyCommandWithMoreParams(string param1, int param2, MoreParams moreParams)
            {
                TestAction(_ctorParam + param1 + param2 + moreParams.MoreParam1 + moreParams.MoreParam2);
            }

            //            [Command]
            //            public void MyCommandWithMoreParams(string param1, int param2)
            //            {
            //                TestAction(_ctorParam + param1 + param2 );
            //            }
        }

        public class MoreParams
        {
            public string MoreParam1 { get; private set; }
            public int MoreParam2 { get; private set; }

            public MoreParams(string moreParam1, int moreParam2)
            {
                MoreParam1 = moreParam1;
                MoreParam2 = moreParam2;
            }
        }
    }
}