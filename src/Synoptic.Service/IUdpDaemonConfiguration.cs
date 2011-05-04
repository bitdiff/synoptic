using System.Net;

namespace Synoptic.Service
{
    public interface IUdpDaemonConfiguration
    {
        IPEndPoint EndPoint { get; }
    }
}