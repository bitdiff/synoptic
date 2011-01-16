using System;
using System.Threading;

namespace Synoptic.Service.Demo
{
    internal class ConsoleWriterWorker : IWorker<string>
    {
        public void Run(string message)
        {
            Console.WriteLine("MESSAGE: " + message);
            Thread.Sleep(5000);
            Console.WriteLine("DONE");
        }
    }
}