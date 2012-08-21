using System;

namespace Synoptic.Service
{
    public class UdpMessageReceivedEventArgs : EventArgs
    {
        private readonly string _message;

        public UdpMessageReceivedEventArgs(string message)
        {
            _message = message;
        }

        public string Message
        {
            get { return _message; }
        }
    }
}