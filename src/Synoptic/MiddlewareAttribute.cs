using System;

namespace Synoptic
{
    public class MiddlewareAttribute : Attribute
    {
        public bool First { get; set; }
        public bool Last { get; set; }
    }
}