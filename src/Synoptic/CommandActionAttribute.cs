using System;

namespace Synoptic
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CommandActionAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsDefault { get; set; }
    }
}