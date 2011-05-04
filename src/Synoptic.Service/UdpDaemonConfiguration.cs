using System.Net;

namespace Synoptic.Service
{
    public class UdpDaemonConfiguration : IUdpDaemonConfiguration
    {
        public UdpDaemonConfiguration(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }

        public IPEndPoint EndPoint { get; set; }
    }
}