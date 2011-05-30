using System;

namespace Synoptic.Demo.Commands
{
    [Command(Name = "third", Description = "Demonstrates some of the command action attributes.")]
    internal class ThirdCommand
    {
        [CommandAction(Description = "A test command that demonstrates command parameters.")]
        public void Run([CommandParameter(DefaultValue = "defaultforparam1")] string param1, int param2, [CommandParameter(Description = "this is a test parameter")] bool param3)
        {
            Console.WriteLine("MyThirdCommand");
            Console.WriteLine("  param1=" + param1);
            Console.WriteLine("  param2=" + param2);
            Console.WriteLine("  param3=" + param3);
        }
    }
}