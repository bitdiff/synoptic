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
            Console.WriteLine(Console.WindowWidth);
            ProgressBar progress = new ProgressBar();
            for (int i = 0; i < 101; i++)
            {
                progress.Update(i);
                System.Threading.Thread.Sleep(75);
            }

            return;
            new CommandRunner().WithMiddleware(new MyFirstFilter(), new MyLastFilter())
               .Run(args);
        }
    }

    [Command(Name = "windows-service", Description = "Windows service")]
    public class WindowsServiceCommand
    {
        [CommandAction(IsDefault = true, Name = "install", Description = "Installs the service")]
        public void Install([CommandParameterAttribute(IsRequired = true)] string input)
        {
            Console.WriteLine("Install to " + input);
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
