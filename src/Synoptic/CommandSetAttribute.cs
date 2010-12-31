using System;

namespace Synoptic
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CommandSetAttribute : Attribute
    {
    }
}