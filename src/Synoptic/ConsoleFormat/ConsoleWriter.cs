using System;

namespace Synoptic.ConsoleFormat
{
    public class ConsoleWriter
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