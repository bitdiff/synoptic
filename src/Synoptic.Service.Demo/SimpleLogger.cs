using System;
using System.IO;

namespace Synoptic.Service.Demo
{
    public class SimpleLogger : ILogger
    {
        private static readonly string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

        public void LogInfo(string name, string message, params object[] args)
        {
            var s = string.Format("{0:u} INFO [{1}] {2}\n", DateTime.Now, name, string.Format(message, args));
            Write(s);
        }

        public void LogException(string name, Exception exception, string message, params object[] args)
        {
            var s = string.Format("{0:u} ERROR [{1}] ({2}) {3}\n", DateTime.Now, name, exception.Message, string.Format(message, args));
            Write(s);
        }

        private static void Write(string s)
        {
            File.AppendAllText(LogFile, s);
            Console.Write(s);
        }
    }
}