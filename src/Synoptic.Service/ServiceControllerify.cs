using System.ServiceProcess;
using System.Threading;

namespace Synoptic.Service
{
    public class ServiceControllerify
    {
        public void Stop(string name)
        {
            var service = new ServiceController(name);
            service.Stop();

            var count = 0;

            while (service.Status != ServiceControllerStatus.Stopped)
            {
                service.Refresh();
                Thread.Sleep(100);
                if (count++ > 20) throw new TimeoutException();
            }
        }

        public void Start(string name)
        {
            new ServiceController(name).Start();
        }
    }
}