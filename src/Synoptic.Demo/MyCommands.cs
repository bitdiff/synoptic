using System;

namespace Synoptic.Demo
{
    public class MyCommands
    {
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

        }

        [Command(Name = "ex")]
        public void MyExceptionGenerator()
        {
            throw new ApplicationException("bmc");
        }

        [Command(Description = "this is another test command")]
        public void MyThirdCommand(string param1, int? param2, [CommandParameter(Description = "this is a test parameter")] bool param3)
        {
            Console.WriteLine("MyCommand");
            Console.WriteLine("  param1=" + param1);
            Console.WriteLine("  param2=" + param2.HasValue);
            Console.WriteLine("  param3" + param3);
        }
    }
}