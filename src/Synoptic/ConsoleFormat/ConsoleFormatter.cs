using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Synoptic.ConsoleFormat
{
    public class ConsoleFormatter
    {
        private static IConsoleWriter _writer = new ConsoleWriter();

        public static void SetWriter(IConsoleWriter writer)
        {
            _writer = writer;
        }

        public static void Write(ConsoleTable table)
        {
            var maxWidth = table.Width ?? _writer.GetWidth();
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
                            _writer.Write("{0}", new string(' ', textWidth + cell.Padding));
                            continue;
                        }

                        _writer.Write("{0,-" + cell.Padding + "}", String.Empty);

                        _writer.SetStyle(cell.Style);
                        _writer.Write("{0}", words);
                        _writer.ResetStyle();

                        _writer.Write("{0}", new string(' ', textWidth - words.Length));
                    }

                    _writer.WriteLine();
                }
            }
        }

        public static void Write(int indent, string format, params string[] args)
        {
            int max = _writer.GetWidth();
            string pad = new string(' ', indent);

            Regex regex = new Regex(@"(\S*(\s)?)");
            Match words = regex.Match(String.Format(format, args));

            int count = _writer.GetCursorColumnPosition();

            _writer.Write(count == 0 ? pad : String.Empty);
            count = _writer.GetCursorColumnPosition();

            while (words.Success)
            {
                string word = words.Value;
                count += word.Length;
                if (count >= max - 1)
                {
                    _writer.WriteLine();
                    _writer.Write(pad);
                    count = word.Length + pad.Length;
                }
                _writer.Write(word);
                words = words.NextMatch();
            }
        }

        public static void Write(string format, params string[] args)
        {
            Write(0, format, args);
        }
    }
}