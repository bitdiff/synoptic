using System;
using System.Net;

namespace Synoptic.Service
{
    public interface ISkinnyUdpServerConfiguration
    {
        IPEndPoint EndPoint { get; }
    }
}