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

        public int Padding { get; set; }
        public int? Width { get; set; }

        public ConsoleCell(string text)
            : this()
        {
            Text = text;
        }

        public ConsoleCell()
        {
            Padding = 3;
        }

        public ConsoleStyle Style { get { return _style; } }
        public ConsoleRow Row { get; set; }

        public string Text
        {
            get { return _text; }
            set
            {
                _currentMatch = _wordExpression.Match(value);
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