using System;

namespace ConsoleWrapper.Synoptic
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CommandSetAttribute : Attribute
    {
    }
}