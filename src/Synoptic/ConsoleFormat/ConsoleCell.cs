using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Synoptic.ConsoleFormat
{
    public class ConsoleCell
    {
        private readonly ConsoleStyle _style = new ConsoleStyle();
        private readonly Regex _wordExpression = new Regex(@"(\S*(\s)?)");
        private Match _currentMatch;
        private string _text;

        internal int Padding { get; set; }
        internal int? Width { get; set; }

        public ConsoleCell(string text)
            : this()
        {
            Text = text;
        }

        public ConsoleCell(string format, params string[] args)
            : this(String.Format(format,args)) { }

        public ConsoleCell()
        {
            Padding = 3;
        }

        public ConsoleCell WithPadding(int padding)
        {
            Padding = padding;
            return this;
        }

        public ConsoleCell WithWidth(int width)
        {
            Width = width;
            return this;
        }

        public ConsoleCell WithForegroundColor(ConsoleColor color)
        {
            Style.ForegroundColor = color;
            return this;
        }

        public ConsoleCell WithBackgroundColor(ConsoleColor color)
        {
            Style.BackgroundColor = color;
            return this;
        }

        public ConsoleStyle Style { get { return _style; } }
        public ConsoleRow Row { get; set; }

        public string Text
        {
            get { return _text; }
            set
            {
                _currentMatch = value == null ? Match.Empty : _wordExpression.Match(value);

                HasMoreWords = _currentMatch.Success;
                _text = value;
            }
        }

        internal string GetWordsForLine(int width)
        {
            int currentLineLength = 0;
            StringBuilder output = new StringBuilder();

            while (_currentMatch.Success)
            {
                string word = _currentMatch.Value;

                // Word is too long to fit on a single line.
                if (word.Length > width)
                {
                    // No space needed on the end of a line.
                    if (word.EndsWith(" ") && word.Length == width)
                        word = word.Substring(0, width);
                    else
                    {
                        // Truncate.
                        word = word.Substring(0, width - 2) + "-";
                    }
                }

                currentLineLength += word.Length;
                if (currentLineLength > width)
                    break;

                _currentMatch = _currentMatch.NextMatch();
                HasMoreWords = _currentMatch.Success;

                output.Append(word);
            }

            return output.ToString();
        }

        internal bool HasMoreWords { get; private set; }
    }
}