using System;
using Mono.Options;
using Synoptic.ConsoleFormat;

namespace Synoptic.Demo2
{
    class Program
    {
        static void Main(string[] args)
        {
//            Console.ResetColor();
//            
//            ConsoleTable t = new ConsoleTable();
//            var row = new ConsoleRow();
//            var cell = new ConsoleCell()
//                           {
//                Text = "111111111111111111111111111111 this is the first column aaa aaa aaa this is still the first column aaa aaa ZZZ ZZZ x x x x x x x x x x x x x x x x x x x x x x x x x x"
//            };
//            var cell2 = new ConsoleCell()
//                            {
//                Text = "this is the second column bbb bbb bbb"
//            };
//            cell2.Style.ForegroundColor = ConsoleColor.Yellow;
//            
//            var cell3 = new ConsoleCell()
//                            {
//                
//                Text = "this is the third column ccc ccc ccc long columns are being tested here especially when there is a middle column that is shorter than the others"
//            };
//
//            cell3.Style.BackgroundColor = ConsoleColor.DarkGreen;
//            row.AddCells(cell, cell2, cell3);
//            t.AddRows(row);
//
//            new ConsoleFormatter().Write(t);
//            return;
            
            
            
            
            
            
            var optionSet = new OptionSet
                                {
                                    { "h|help", "shows help",v => { GlobalOptions.Help = v; } },
                                    { "m|master=", v => { GlobalOptions.Help = v; } },
                                    { "s=", v => { GlobalOptions.Help = v; } },
                                    { "lll|llllll=", v => { GlobalOptions.Help = v; } },
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

    [Command(Name = "this is a really long name", Description = "This is another command. Testing the wrapping of the description.")]
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
