using System;

namespace Synoptic
{
    public class CommandInvocationException : Exception
    {
        public CommandInvocationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CommandInvocationException(string message) : base(message)
        {
        }
    }
}