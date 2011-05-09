using System.Collections.Generic;

namespace Synoptic
{
    public class Request
    {
        private readonly IDictionary<string, object> _context = new Dictionary<string, object>();

        public Request(params string[] arguments)
        {
            Arguments = arguments;
        }

        public IDictionary<string, object> Context
        {
            get { return _context; }
        }

        public string[] Arguments { get; set; }
    }
}