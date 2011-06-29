using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Synoptic.ConsoleFormat
{
    public class ConsoleFormatter
    {
        private readonly IConsoleWriter _consoleWriter;

        public ConsoleFormatter(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        public void Write(ConsoleTable table)
        {
            var maxWidth = table.Width ?? _consoleWriter.GetWidth();

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

                        _consoleWriter.Write("{0,-" + cell.Padding + "}", String.Empty);

                        _consoleWriter.SetStyle(cell.Style);
                        _consoleWriter.Write("{0}", words);
                        _consoleWriter.ResetStyle();

                        // Filling the line exactly causes it to wrap so no "WriteLine" is necessary.
                        _consoleWriter.Write("{0}", new string(' ', textWidth - words.Length));
                    }
                }
            }
        }

        public void Write(int indent, string format, params object[] args)
        {
            int max = _consoleWriter.GetWidth();
            string pad = new string(' ', indent);

            Regex regex = new Regex(@"(\S*(\s)?)");
            Match words = regex.Match(String.Format(format, args));

            int count = _consoleWriter.GetCursorColumnPosition();

            _consoleWriter.Write(count == 0 ? pad : String.Empty);
            count = _consoleWriter.GetCursorColumnPosition();

            while (words.Success)
            {
                string word = words.Value;
                count += word.Length;
                if (count >= max - 1)
                {
                    _consoleWriter.WriteLine();
                    _consoleWriter.Write(pad);
                    count = word.Length + pad.Length;
                }
                _consoleWriter.Write(word);
                words = words.NextMatch();
            }
        }

        public void Write(string format, params object[] args)
        {
            Write(0, format, args);
        }

        public void WriteLine()
        {
            _consoleWriter.WriteLine();
        }
    }
}