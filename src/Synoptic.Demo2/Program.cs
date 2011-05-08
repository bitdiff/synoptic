using System;

namespace Synoptic.Demo2
{
    class Program
    {
        static void Main(string[] args)
        {
            new CommandRunner2()
               .Run(args);
        }
    }

    [Command(Name = "windows-service", Description = "Windows service")]
    public class WindowsServiceCommand
    {
        [CommandAction(Name = "install", Description = "Installs the service")]
        public void Install([CommandParameterAttribute(IsRequired = true)] string input)
        {
            Console.WriteLine("Install to "+ input);
        }

        [CommandAction(Name = "uninstall", Description = "Uninstalls the service")]
        public void Uninstall()
        {
            Console.WriteLine("Uninstall");
        }

        [CommandAction(Name = "console", Description = "Runs the service in console mode")]
        public void Run()
        {
            Console.WriteLine("Console");
        }
    }
}
