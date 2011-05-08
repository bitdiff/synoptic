using System;
using System.Collections.Generic;

namespace Synoptic
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CommandActionAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }    
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

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

    public class MiddlewareAttribute : Attribute
    {
        public bool First { get; set; }
        public bool Last { get; set; }
    }
}