using System;

namespace Synoptic.Exceptions
{
    public abstract class CommandParseExceptionBase : Exception
    {
        public abstract void Render();
    }
}