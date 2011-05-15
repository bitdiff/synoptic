using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Synoptic.ConsoleUtilities
{
    public static class Out
    {
        public static void WordWrap(string val, int indent, string prefix)
        {
            StringBuilder output = new StringBuilder();

            int max = Console.WindowWidth;
            string padding = new string(' ', indent) + prefix;

            Regex r = new Regex(@"([\w\.\:\-\=\|\[\]]*(\s)?)");
            Match words = r.Match(val);

            int count = Console.CursorLeft;

            output.Append(count == 0 ? padding : prefix);
            count = Console.CursorLeft;

            while (words.Success)
            {
                string word = words.Value;
                count += word.Length;
                if (count >= max - 1)
                {
                    output.AppendLine();
                    output.Append(padding);
                    count = word.Length + padding.Length;
                }
                output.Append(word);
                words = words.NextMatch();
            }

            Console.Write(output.ToString());
        }

        public static void WordWrap(string val, int indent)
        {
            WordWrap(val, indent, string.Empty);
        }

        public static void WordWrap(string val)
        {
            WordWrap(val, 0);
        }
    }
}