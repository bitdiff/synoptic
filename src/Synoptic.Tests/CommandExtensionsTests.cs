using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Synoptic.Tests
{
    [TestFixture]
    public class CommandExtensionsTests
    {
        [Test]
        public void shoud_run_command()
        {
            CommandManifest manifest = new CommandFinder().FindInType(typeof(Test1));
            var parseResult = new CommandLineParseResult(new List<CommandLineParameter> { new CommandLineParameter("param1", "testParam1Value") }, new[] { "my-command" });
            var commandName = CommandNameExtractor.Extract(new[] { parseResult });

            Command command = new CommandResolver().Resolve(manifest, commandName);

            string testResult = "fail";
            Test1.TestAction = s => testResult = s;

            command.Run(new ActivatorDependencyResolver(), parseResult);

            Assert.That(testResult, Is.EqualTo("testParam1Value"));
        }

        public class Test1
        {
            public static Action<string> TestAction = s => { };
            [Command]
            public void MyCommand(string param1)
            {
                TestAction(param1);
            }
        }
    }
}