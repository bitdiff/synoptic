using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Synoptic.Service
{
    public class SkinnyUdpServer : IDaemon
    {
        private const string LogName = "SkinnyUdpServer";
        private readonly ManualResetEvent _resetEvent;
        private readonly ILogger _logger;
        private readonly IPEndPoint _ipEndPoint;
        private readonly IWorker<string> _worker;
        private Thread _serviceThread;
        private Socket _udpSock;
        private int _requestCount;

        public SkinnyUdpServer(IWorker<string> worker, ILogger logger, ISkinnyUdpServerConfiguration configuration)
        {
            _logger = logger;
            _worker = worker;
            _ipEndPoint = configuration.EndPoint;
            _resetEvent = new ManualResetEvent(false);
        }

        public void Start()
        {
            _logger.LogInfo(LogName, "Starting..");
            
            _resetEvent.Reset();

            _serviceThread = new Thread(StartService);

            _serviceThread.Start();

            _logger.LogInfo(LogName, "Started");
        }

        public void Stop()
        {
            if (_serviceThread == null)
            {
                _logger.LogInfo(LogName, "Cannot stop. Did you forget to call Start before this point?");
                return;
            }

            _logger.LogInfo(LogName, "Stopping..");

            _resetEvent.Set();

            _serviceThread.Join();

            _logger.LogInfo(LogName, "Stopped");
        }

        private void StartService()
        {
            _udpSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _udpSock.Bind(_ipEndPoint);

            EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                var buffer = new byte[256];
                IAsyncResult asyncResult = _udpSock.BeginReceiveFrom(buffer, 0, buffer.Length, 0, ref remote, ReceiveMessage, buffer);

                if (WaitHandle.WaitAny(new[] { asyncResult.AsyncWaitHandle, _resetEvent }) == 1) break;
            }

            _logger.LogInfo(LogName, "Closing socket");

            _udpSock.Close();

            _logger.LogInfo(LogName, "Waiting for requests to be processed");

            int i = 0;
            while (_requestCount > 0)
            {
                Thread.Sleep(100);
                
                if (i++ <= 300) continue; // Hard coded 30 second timeout

                _logger.LogInfo(LogName, "Requests taking too long. Exiting thread");
                break;
            }
        }

        private void ReceiveMessage(IAsyncResult iar)
        {
            Interlocked.Increment(ref _requestCount);
            try
            {
                var buf = (byte[])iar.AsyncState;

                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                int msgLen = 0;

                try
                {
                    msgLen = _udpSock.EndReceiveFrom(iar, ref remoteEndPoint);
                }
                catch (ArgumentNullException e)
                {
                    _logger.LogException(LogName, e, "AsyncResult is null");
                }
                catch (ArgumentException e)
                {
                    _logger.LogException(LogName, e, "AsyncResult was not returned by a call to the BeginReceiveFrom method");
                }
                catch (ObjectDisposedException)
                {
                    _logger.LogInfo(LogName, "The Socket has been closed");
                }
                catch (InvalidOperationException e)
                {
                    _logger.LogException(LogName, e, "EndReceiveFrom was previously called for the asynchronous read");
                }
                catch (SocketException e)
                {
                    _logger.LogException(LogName, e, "An error occurred when attempting to access the socket");
                }

                if (msgLen > 0)
                    _worker.Run(Encoding.UTF8.GetString(buf, 0, msgLen));
            }
            catch (Exception e)
            {
                _logger.LogException(LogName, e, "Unexpected error occurred");
            }
            finally
            {
                Interlocked.Decrement(ref _requestCount);
            }
        }
    }
}