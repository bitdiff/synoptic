using System;
using System.ServiceProcess;

namespace Synoptic.Service
{
    public class SafeServiceController
    {
        public void Stop(string name)
        {
            var service = new ServiceController(name);
            if (service.Status == ServiceControllerStatus.Stopped)
                return;
            
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 30));
        }

        public void Start(string name)
        {
            var service = new ServiceController(name);
            if (service.Status == ServiceControllerStatus.Running)
                return;

            service.Start();
        }
    }
}