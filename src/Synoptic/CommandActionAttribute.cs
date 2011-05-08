using System;

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
}