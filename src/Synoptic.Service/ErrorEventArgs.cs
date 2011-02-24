using System;

namespace Synoptic.Service
{
    public class ErrorEventArgs : EventArgs
    {
        private readonly Exception _exception;

        public ErrorEventArgs(Exception exception)
        {
            _exception = exception;
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }
}