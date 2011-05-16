using System;
using System.Text.RegularExpressions;

namespace Synoptic.ConsoleUtilities
{
    public static class ConsoleFormatter
    {
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
    }
}