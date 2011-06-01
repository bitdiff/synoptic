using System;
using Synoptic.ConsoleFormat;

namespace Synoptic.Exceptions
{
    public abstract class CommandParseExceptionBase : Exception
    {
        public abstract void Render();
        
        public ConsoleFormatter ConsoleFormatter
        {
            get
            {
                return new ConsoleFormatter(ConsoleWriter.Error);    
            }
        }
    }
}