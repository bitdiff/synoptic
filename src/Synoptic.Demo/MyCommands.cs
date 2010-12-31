using System;

namespace Synoptic.Demo
{
    internal class MyCommands
    {
        private readonly IMyService _myService;
        private readonly IMyService2 _myService2;

        public MyCommands(IMyService myService, IMyService2 myService2)
        {
            _myService = myService;
            _myService2 = myService2;
        }

        [Command]
        public void RunMyServices(string message)
        {
            Console.WriteLine("RunMyServices");
            Console.WriteLine("  message=" + message);
            Console.WriteLine("  _myService.Hello=" + _myService.Hello(message));
            Console.WriteLine("  _myService2.Hello2=" + _myService2.Hello2(message));
        }

        [Command]
        public void MyCommand(string param1)
        {
            Console.WriteLine("MyCommand");
            Console.WriteLine("  param1=" + param1);
        }

        [Command]
        public void MyCommand2(string camelParamOne)
        {
            Console.WriteLine("MyCommand2");
            Console.WriteLine("  param1=" + camelParamOne);
        }

        [Command]
        public void MyFourthCommand(Uri myUri)
        {
            Console.WriteLine("MyFourthCommand");
            Console.WriteLine("  myUri=" + myUri);
        }

        [Command(Name = "ex")]
        public void MyExceptionGenerator()
        {
            throw new ApplicationException("MyExceptionGenerator");
        }

        [Command]
        public void MyCommandException()
        {
            throw new CommandException("This goes to console.");
        }

        [Command(Description = "this is another test command")]
        public void MyThirdCommand([CommandParameter(DefaultValue = "defaultforparam1")] string param1, int param2, [CommandParameter(Description = "this is a test parameter")] bool param3)
        {
            Console.WriteLine("MyThirdCommand");
            Console.WriteLine("  param1=" + param1);
            Console.WriteLine("  param2=" + param2);
            Console.WriteLine("  param3=" + param3);
        }
    }
}