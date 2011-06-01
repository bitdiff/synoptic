using System;

namespace Synoptic.Demo.Commands
{
    [Command(Name = "windows-service", Description = "Allows the configuration of a windows service.")]
    public class WindowsServiceCommand
    {
        [CommandAction(Name = "install", Description = "Installs the service")]
        public void Install([CommandParameterAttribute(IsRequired = true)] string input)
        {
            Console.WriteLine("Install to " + input);
        }

        [CommandAction(Name = "uninstall", Description = "Uninstalls the service")]
        public void Uninstall([CommandParameterAttribute(Description = "Another input field", IsRequired = true)] string input2, [CommandParameterAttribute(Description = "Yet another input field", IsRequired = true)] string input3)
        {
            Console.WriteLine("Uninstall");
        }

        [CommandAction(Name = "run", Description = "Runs the service in console mode")]
        public void Run()
        {
            Console.WriteLine("Console");
        }
    }
}