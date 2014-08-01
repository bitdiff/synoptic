using System;
using System.Configuration;
using System.Net;
using System.Threading;

namespace Synoptic.Service
{
    public class UdpPollingPreempter : IPollingPreempter
    {
        private readonly IDaemonLogger _logger;
        private UdpServer _server;
        private readonly int? _preemptOnPort;
        public event EventHandler ShouldPreempt;

        public void OnError(Exception exception, CancellationToken cancellationToken)
        {
            _logger.Error(LogTag, exception);

            Stop();
            Listen(cancellationToken);
        }

        private const string LogTag = "udppolling.daemon";

        public UdpPollingPreempter(IDaemonLogger logger)
        {
            _logger = logger;
            _preemptOnPort = GetPreemptPort();
        }

        public void Listen(CancellationToken cancellationToken)
        {
            if (!_preemptOnPort.HasValue)
            {
                return;
            }

            var localEndPoint = new IPEndPoint(IPAddress.Any, _preemptOnPort.Value);

            _server = new UdpServer();
            _server.Start(localEndPoint, cancellationToken);

            _server.ReceiveError += (s, e) => _logger.Info(LogTag, "Socket error {0} during {1}.", e.ErrorCode, e.LastOperation);
            _server.MessageReceived += (s, e) => Preempt(e);

            _logger.Info(LogTag, "Preempt poll udp listener configured on port {0}.", _preemptOnPort.Value);
        }

        private int? GetPreemptPort()
        {
            int port;
            var isValid = Int32.TryParse(ConfigurationManager.AppSettings["preemptOnPort"], out port);

            return isValid ? port : new int?();
        }

        public void Preempt(UdpMessageReceivedEventArgs messageReceivedEventArgs)
        {
            try
            {
                OnPreempted();
            }
            catch (Exception exception)
            {
                _logger.Error(LogTag, exception);
            }
        }

        protected virtual void OnPreempted()
        {
            if (ShouldPreempt != null)
            {
                _logger.Debug(LogTag, "Received preempt message.");
                ShouldPreempt(this, null);
            }
        }

        public void Stop()
        {
            if (_server != null)
            {
                _server.Dispose();
            }

            _logger.Info(LogTag, "Preempt poll udp listener stopped.", _preemptOnPort);
        }
    }
}