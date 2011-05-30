using System;
using System.Collections.Generic;
using System.Linq;

namespace Synoptic
{
    internal static class StringExtensions
    {
        internal static bool SimilarTo(this string str1, string str2)
        {
            if (str1 == null || str2 == null)
                return false;

            return str1.Replace("-", "").Replace("_", "").ToLowerInvariant() ==
                   str2.Replace("-", "").Replace("_", "").ToLowerInvariant();
        }

        internal static string GetNewIfValid(this string originalString, string newString)
        {
            return String.IsNullOrEmpty(newString) ? originalString : newString;
        }

        internal static string ToHyphened(this string input)
        {
            if (String.IsNullOrEmpty(input))
                return String.Empty;

            var chars = input.ToCharArray();
            var result = new List<char>(chars.Take(1));

            foreach (var c in chars.Skip(1))
            {
                if (Char.IsUpper(c)) result.Add('-');
                result.Add(c);
            }

            return new string(result.ToArray()).Replace(" ", "-").ToLowerInvariant();
        }
    }
}