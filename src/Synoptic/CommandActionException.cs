using System;

namespace Synoptic
{
    public class CommandActionException : Exception
    {
        public CommandActionException(string message)
            : base(message)
        {
        }
    }
}