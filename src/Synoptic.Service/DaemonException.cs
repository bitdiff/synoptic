using System;

namespace Synoptic.Service
{
    public class DaemonException : Exception
    {
        public DaemonException(string message, Exception exception) : base(message, exception) { }
    }
}