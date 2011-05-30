using System;
using Synoptic.Demo.Services;

namespace Synoptic.Demo.Commands
{
    [Command(Name = "didemo", Description = "Shows how to use DI to inject services into commands.")]
    internal class ServiceDemoCommand
    {
        private readonly IMyService _myService;
        private readonly IMyService2 _myService2;

        public ServiceDemoCommand(IMyService myService, IMyService2 myService2)
        {
            _myService = myService;
            _myService2 = myService2;
        }

        [CommandAction]
        public void RunMyServices(string message)
        {
            Console.WriteLine("RunMyServices");
            Console.WriteLine("  message=" + message);
            Console.WriteLine("  _myService.Hello=" + _myService.Hello(message));
            Console.WriteLine("  _myService2.Hello2=" + _myService2.Hello2(message));
        }
    }
}