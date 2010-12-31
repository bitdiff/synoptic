using System;

namespace ConsoleWrapper.Synoptic
{
    public class CommandException : Exception
    {
        public CommandException(string message)
            : base(message)
        {
        }
    }
}