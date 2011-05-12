using System;
using Mono.Options;

namespace Synoptic.Demo2
{
    class Program
    {
        static void Main(string[] args)
        {
            //            Console.WriteLine(Console.WindowWidth);
            //            ProgressBar progress = new ProgressBar();
            //            for (int i = 0; i < 101; i++)
            //            {
            //                progress.Update(i);
            //                System.Threading.Thread.Sleep(75);
            //            }
            //
            //            return;
            var optionSet = new OptionSet
                                {
                                    { "h|help", v => { GlobalOptions.Help = v; } },
                                    { "m|master=", v => { GlobalOptions.Help = v; } },
                                };
            new CommandRunner().WithGlobalOptions(optionSet)
               .Run(args);
        }

        public static MyGlobalOptions GlobalOptions = new MyGlobalOptions();
    }

    public class MyGlobalOptions
    {
        public string Help { get; set; }
    }

    [Command(Name = "this is a really long name", Description = "This is another command. Testing the wrapping of the description")]
    public class SomeOtherCommand
    {
        [CommandAction(IsDefault = true)]
        public void Crank()
        {
            Console.WriteLine("Cranking...");
            Console.WriteLine(Program.GlobalOptions.Help);
        }
    }

    [Command(Name = "windows-service", Description = "Allows the configuration of a windows service.")]
    public class WindowsServiceCommand
    {
        [CommandAction(Name = "install", Description = "Installs the service")]
        public void Install([CommandParameterAttribute(IsRequired = true)] string input)
        {
            Console.WriteLine("Install to " + input);
        }

        [CommandAction(Name = "uninstall", Description = "Uninstalls the service")]
        public void Uninstall()
        {
            Console.WriteLine("Uninstall");
        }

        [CommandAction(IsDefault = true, Name = "run", Description = "Runs the service in console mode")]
        public void Run()
        {
            Console.WriteLine("Console");
        }
    }
}
