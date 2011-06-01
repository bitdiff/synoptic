using System;
using Synoptic.ConsoleFormat;

namespace Synoptic.Tests
{
    public class TestConsoleWriter : IConsoleWriter
    {
        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void Write(string format, params string[] args)
        {
            Console.Write(format, args);
        }

        public void SetStyle(ConsoleStyle style)
        {
        }

        public void ResetStyle()
        {
        }

        public int GetWidth()
        {
            return 80;
        }

        public int GetCursorColumnPosition()
        {
            return 0;
        }
    }
}