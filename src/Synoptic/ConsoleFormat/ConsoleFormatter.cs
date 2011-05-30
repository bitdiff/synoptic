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

                        if (words.Length > 0)
                            SetConsoleStyle(cell.Style);
                        Console.Write("{0}", words);
                        Console.ResetColor();

                        Console.Write("{0}", new string(' ', width - words.Length));
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

    public static class ConsoleTableExtensions
    {
//        public static ConsoleRow AddRow(this ConsoleTable table)
//        {
//            if (table == null)
//                throw new ArgumentNullException("table");
//
//            ConsoleRow row = new ConsoleRow();
//            table.Rows.AddRow(row);
//            return row;
//        }


        public static ConsoleCell AddCell(this ConsoleRow row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            ConsoleCell cell = new ConsoleCell();
            row.AddCell(cell);

            return cell;
        }

        //        public static class GenericWebControlExtensions
        //        {
        //            public static T SetFontSize<T>(this T control, FontUnit fontSize) where T : WebControl
        //            {
        //                control.Font.Size = fontSize;
        //                return control;
        //            }
        //
        //            public static T SetFontBold<T>(this T control, bool isbold) where T : WebControl
        //            {
        //                control.Font.Bold = isbold;
        //                return control;
        //            }
        //        }
    }
}