using System.Net;

namespace Synoptic.Service
{
    public class SkinnyUdpServerConfiguration : ISkinnyUdpServerConfiguration
    {
        public SkinnyUdpServerConfiguration(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }

        public IPEndPoint EndPoint { get; private set; }
    }
}