using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Synoptic.ConsoleUtilities;

namespace Synoptic.Tests
{
    [TestFixture]
    public class CommandExtensionsTests
    {
        [Test]
        public void should_indent()
        {
            //            StringWriter inner = new StringWriter();
            //            IndentedTextWriter writer = new IndentedTextWriter(Console.Out, new string(' ', 3));

//            var cf = new Cf(Console.Out, 80);

//            cf.AddParagraph(
//                new Paragraph(
//                    @"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
//                    10)).
//                    AddParagraph(
//                    new Paragraph(
//                    @"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
//                    20));

//            cf.Write();
        }

        [Test]
        public void shoud_run_command()
        {
            //            CommandActionManifest actionManifest = new CommandActionFinder().FindInType(typeof(Test1));
            //            var parseResult = new CommandLineParseResult(actionManifest.Commands[0], new List<CommandLineParameter> { new CommandLineParameter("param1", "testParam1Value") }, new[] { "my-command" });
            //
            //            CommandAction commandAction = new CommandResolver().Resolve(actionManifest, "my-command");
            //
            //            string testResult = "fail";
            //            Test1.TestAction = s => testResult = s;
            //
            //            commandAction.Run(new ActivatorDependencyResolver(), parseResult);
            //
            //            Assert.That(testResult, Is.EqualTo("testParam1Value"));
        }

        public class Test1
        {
            public static Action<string> TestAction = s => { };
            [CommandAction]
            public void MyCommand(string param1)
            {
                TestAction(param1);
            }
        }
    }
}