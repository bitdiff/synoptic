using System;

namespace Synoptic.Exceptions
{
    public class CommandActionException : Exception
    {
        public CommandActionException(string message)
            : base(message)
        {
        }
    }
}