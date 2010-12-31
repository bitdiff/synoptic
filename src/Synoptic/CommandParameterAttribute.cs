using System;

namespace Synoptic
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class CommandParameterAttribute : Attribute
    {
        public CommandParameterAttribute()
        {
        }

        public CommandParameterAttribute(string prototype)
        {
            Prototype = prototype;  
        }

        public CommandParameterAttribute(string prototype, string description)
        {
            Prototype = prototype;
            Description = description;
        }

        public string Prototype { get; set; }
        public string Description { get; set; }
        public object DefaultValue { get; set; }
    }
}