using System;

namespace Synoptic.Service
{
    public class WindowsServiceEventArgs : EventArgs
    {
        private readonly string _serviceName;

        public WindowsServiceEventArgs(string serviceName)
        {
            _serviceName = serviceName;
        }

        public string ServiceName
        {
            get { return _serviceName; }
        }
    }
}