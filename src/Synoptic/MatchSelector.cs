using System;
using System.Collections.Generic;
using System.Linq;

namespace Synoptic
{
    internal class MatchSelector<T> where T : class
    {
        public IEnumerable<T> PartialMatch(string query, IEnumerable<T> items, Func<T, string> source)
        {
            var exactMatch = items.FirstOrDefault(item => source(item).Equals(query, StringComparison.OrdinalIgnoreCase));
            if (exactMatch != null)
                return new[] { exactMatch };
            
            return items.Where(item => source(item).StartsWith(query)).AsEnumerable();
        }

        public T Match(string query, IEnumerable<T> items, Func<T, string> source)
        {
            return items.FirstOrDefault(item => source(item).Equals(query, StringComparison.OrdinalIgnoreCase));
        }
    }
}