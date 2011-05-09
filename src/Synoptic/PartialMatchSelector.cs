using System;
using System.Collections.Generic;
using System.Linq;

namespace Synoptic
{
    internal class PartialMatchSelector<T> where T : class
    {
        public IEnumerable<T> Match(string query, IEnumerable<T> items, Func<T, string> source)
        {
            var exactMatch = items.FirstOrDefault(item => source(item).Equals(query, StringComparison.OrdinalIgnoreCase));
            if (exactMatch != null)
                return new[] { exactMatch };
            
            return items.Where(item => source(item).StartsWith(query)).AsEnumerable();
        }
    }
}