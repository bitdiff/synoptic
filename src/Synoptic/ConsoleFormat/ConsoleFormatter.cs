using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Synoptic.ConsoleFormat
{
    public class ConsoleFormatter
    {
        public static void Write(ConsoleTable table)
        {
            var maxWidth = table.Width ?? Console.WindowWidth;
            maxWidth -= 1;

            foreach (var row in table.Rows)
            {
                var cellTextWidths = row.CalculateCellTextWidths(maxWidth).ToList();

                while (row.HasMoreLines())
                {
                    foreach (var cell in row.Cells)
                    {
                        var index = row.Cells.IndexOf(cell);
                        var textWidth = cellTextWidths[index];
                        var words = cell.GetWordsForLine(textWidth).TrimEnd();

                        if (words.Length == 0)
                        {
                            Console.Write("{0}", new string(' ', textWidth + cell.Padding));
                            continue;
                        }

                        Console.Write("{0,-" + cell.Padding + "}", String.Empty);

                        SetConsoleStyle(cell.Style);
                        Console.Write("{0}", words);
                        Console.ResetColor();

                        Console.Write("{0}", new string(' ', textWidth - words.Length));
                    }

                    WriteLine();
                }
            }
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static void Write(int indent, string format, params string[] args)
        {
            int max = Console.WindowWidth;
            string pad = new string(' ', indent);

            Regex regex = new Regex(@"(\S*(\s)?)");
            Match words = regex.Match(String.Format(format, args));

            int count = Console.CursorLeft;

            Console.Write(count == 0 ? pad : String.Empty);
            count = Console.CursorLeft;

            while (words.Success)
            {
                string word = words.Value;
                count += word.Length;
                if (count >= max - 1)
                {
                    WriteLine();
                    Console.Write(pad);
                    count = word.Length + pad.Length;
                }
                Console.Write(word);
                words = words.NextMatch();
            }
        }

        public static void Write(string format, params string[] args)
        {
            Write(0, format, args);
        }

        private static void SetConsoleStyle(ConsoleStyle style)
        {
            if (style.ForegroundColor.HasValue)
                Console.ForegroundColor = style.ForegroundColor.Value;
            if (style.BackgroundColor.HasValue)
                Console.BackgroundColor = style.BackgroundColor.Value;
        }
    }
}