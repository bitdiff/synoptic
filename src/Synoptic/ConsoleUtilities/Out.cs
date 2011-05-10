using System;
using System.Text.RegularExpressions;

namespace Synoptic.ConsoleUtilities
{
    public static class Out
    {
        public static void WordWrap(string val, int indent, string prefix)
        {
            int max = Console.WindowWidth;
            string pad = new string(' ', indent) + prefix;

            Regex r = new Regex(@"([\w\.\:\-]*(\s)?)");
            Match words = r.Match(val);

            int count = Console.CursorLeft;
            
            Console.Write(count == 0 ? pad : prefix);
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