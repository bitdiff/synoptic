using System;
using System.Collections.Generic;
using Mono.Options;
using Synoptic.ConsoleUtilities;
using Synoptic.Pipeline;

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
            new CommandRunner().WithArgumentFilters(new MyFirstFilter(), new MyLastFilter())
               .Run(args);
        }
    }

    [Command(Name="this is a really long name", Description = "This is another command. Testing the wrapping of the description")]
    public class SomeOtherCommand
    {
        
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
    
    [Middleware(First = true)]
    public class MyFirstFilter : IFilter<Request, Response>
    {
        public Response Process(Request request, Func<Request,Response> executeNext)
        {
            List<string> result = new OptionSet()
                .Add("master|m=", "Master name", m => request.Context["masterName"] = m).Parse(request.Arguments);
            
            request.Arguments = result.ToArray();
            return executeNext(request).Append("ssssssssss");
            //return new Response("ow");
        }
    }

    [Middleware(Last = true)]
    public class MyLastFilter : IFilter<Request,Response>
    {
        public Response Process(Request request, Func<Request,Response> executeNext)
        {
            if (request.Context.ContainsKey("masterName"))
                Console.WriteLine(request.Context["masterName"]);
            return new Response("last");
        }
    }
}
