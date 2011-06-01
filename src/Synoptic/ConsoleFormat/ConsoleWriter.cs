using System;
using System.IO;

namespace Synoptic.ConsoleFormat
{
    public class ConsoleWriter : IConsoleWriter
    {
        private readonly TextWriter _writer;

        public ConsoleWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public static ConsoleWriter Default
        {
            get
            {
                return new ConsoleWriter(Console.Out);
            }
        }

        public static ConsoleWriter Error
        {
            get
            {
                return new ConsoleWriter(Console.Error);
            }
        }

        public void WriteLine()
        {
            _writer.WriteLine();
        }

        public void Write(string format, params string[] args)
        {
            _writer.Write(format, args);
        }

        public void SetStyle(ConsoleStyle style)
        {
            if (style.ForegroundColor.HasValue)
                Console.ForegroundColor = style.ForegroundColor.Value;
            if (style.BackgroundColor.HasValue)
                Console.BackgroundColor = style.BackgroundColor.Value;
        }

        public void ResetStyle()
        {
            Console.ResetColor();
        }

        public int GetWidth()
        {
            return Console.WindowWidth;
        }

        public int GetCursorColumnPosition()
        {
            return Console.CursorLeft;
        }
    }
}