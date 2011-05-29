using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Synoptic.ConsoleFormat
{
    public class ConsoleFormatter
    {
        public static void Write(ConsoleTable table)
        {
            var maxWidth = table.Width ?? 75;

            foreach (var row in table.Rows)
            {
                var cellWidths = row.CalculateCellWidths(maxWidth);

                while (row.HasMoreLines())
                {
                    for (int i = 0; i < row.Cells.Count(); i++)
                    {
                        var cell = row.Cells.ElementAt(i);

                        var width = cellWidths.ElementAt(i);
                        var words = cell.GetWordsForLine(width);

                        Console.Write("{0,-" + cell.Padding + "}", String.Empty);
                        // Prevent applying style to empty lines caused by other cells wrapping.
//                        Console.Write("{0,-" + width + "}", words);
                        if (words.Length > 0)
                            SetConsoleStyle(cell.Style);

                        Console.Write("{0}", words);
                        Console.ResetColor();

                        Console.Write("{0}", new string(' ', width - words.Length));
                    }

                    Console.WriteLine();
                }
            }
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
                    Console.WriteLine();
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