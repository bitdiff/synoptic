using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Synoptic.Service
{
    public class SkinnyUdpServer : IDaemon
    {
        public EventHandler<EventArgs> OnStarting = (s, e) => { };
        public EventHandler<EventArgs> OnStarted = (s, e) => { };
        public EventHandler<EventArgs> OnStopping = (s, e) => { };
        public EventHandler<EventArgs> OnStopped = (s, e) => { };
        public EventHandler<ErrorEventArgs> OnError = (s, e) => { };

        private readonly ManualResetEvent _resetEvent;
        private readonly IPEndPoint _ipEndPoint;
        private readonly IWorker<string> _worker;
        private Thread _serviceThread;
        private Socket _udpSock;
        private int _requestCount;

        public SkinnyUdpServer(IWorker<string> worker, ISkinnyUdpServerConfiguration configuration)
        {
            _worker = worker;
            _ipEndPoint = configuration.EndPoint;
            _resetEvent = new ManualResetEvent(false);
        }

        public void Start()
        {
            OnStarting(this, new EventArgs());

            _resetEvent.Reset();
            _serviceThread = new Thread(StartService);
            _serviceThread.Start();

            OnStarted(this, new EventArgs());
        }

        public void Stop()
        {
            if (_serviceThread == null)
                return;

            OnStopping(this, new EventArgs());

            _resetEvent.Set();
            _serviceThread.Join();

            OnStopped(this, new EventArgs());
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

            _udpSock.Close();

            int i = 0;
            while (_requestCount > 0)
            {
                Thread.Sleep(100);
                
                // Hard coded 30 second timeout.
                if (i++ <= 300) continue; 
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

                int msgLen = _udpSock.EndReceiveFrom(iar, ref remoteEndPoint);
                if (msgLen > 0)
                    _worker.Run(Encoding.UTF8.GetString(buf, 0, msgLen));
            }
            catch (Exception e)
            {
                OnError(this, new ErrorEventArgs(e));
            }
            finally
            {
                Interlocked.Decrement(ref _requestCount);
            }
        }
    }
}