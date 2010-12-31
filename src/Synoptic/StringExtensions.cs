using System;
using System.Collections.Generic;

namespace ConsoleWrapper.Synoptic
{
    internal static class StringExtensions
    {
        internal static bool EqualEnough(this string str1, string str2)
        {
            if (str1 == null || str2 == null)
                return false;

            return str1.Replace("-", "").Replace("_", "").ToLowerInvariant() ==
                   str2.Replace("-", "").Replace("_", "").ToLowerInvariant();
        }

        public static string GetNewIfValid(this string originalString, string newString)
        {
            return String.IsNullOrEmpty(newString) ? originalString : newString;
        }

        public static string ToHyphened(this string value)
        {
            var cs = new List<char>();
            var firstCharPassed = false;
            foreach (var c in value)
            {
                if (Char.IsUpper(c) && firstCharPassed) cs.Add('-');
                cs.Add(c);
                firstCharPassed = true;
            }

            return new string(cs.ToArray()).ToLower();
        }
    }
}