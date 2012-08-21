using System;
using System.Net.Sockets;

namespace Synoptic.Service
{
    public class SocketReceiveErrorEventArgs : EventArgs
    {
        private readonly int _errorCode;
        private readonly SocketAsyncOperation _lastOperation;

        public SocketReceiveErrorEventArgs(int errorCode, SocketAsyncOperation lastOperation)
        {
            _errorCode = errorCode;
            _lastOperation = lastOperation;
        }

        public int ErrorCode
        {
            get { return _errorCode; }
        }

        public SocketAsyncOperation LastOperation
        {
            get { return _lastOperation; }
        }
    }
}