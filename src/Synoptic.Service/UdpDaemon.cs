using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Synoptic.Service
{
    public class UdpDaemon : IDaemon
    {
        private readonly Action<string> _action;
        public event EventHandler<EventArgs> Starting = (s, e) => { };
        public event EventHandler<EventArgs> Started = (s, e) => { };
        public event EventHandler<EventArgs> Stopping = (s, e) => { };
        public event EventHandler<EventArgs> Stopped = (s, e) => { };
        public event EventHandler<ErrorEventArgs> Error = (s, e) => { };
        public event EventHandler<ErrorEventArgs> SocketError = (s, e) => { };

        private void OnEvent(EventHandler<ErrorEventArgs> handle, ErrorEventArgs e)
        {
            if (handle != null)
                handle(this, e);
        }
        
        private void OnEvent(EventHandler<EventArgs> handle, EventArgs e)
        {
            if (handle != null)
                handle(this, e);
        }

        private readonly ManualResetEvent _resetEvent;
        private readonly IPEndPoint _ipEndPoint;
        private Thread _serviceThread;
        private Socket _udpSock;
        private int _requestCount;

        public UdpDaemon(Action<string> action, IUdpDaemonConfiguration configuration)
        {
            _action = action;

            _ipEndPoint = configuration.EndPoint;
            _resetEvent = new ManualResetEvent(false);
        }

        public UdpDaemon(Action<string> action, Action<UdpDaemonConfiguration> configure) : 
            this(action, SetConfiguration(configure))
        {
        }

        private static IUdpDaemonConfiguration SetConfiguration(Action<UdpDaemonConfiguration> configure)
        {
            var configuration = new UdpDaemonConfiguration(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50000));
            configure(configuration);

            return configuration;
        }

        public void Start()
        {
            OnEvent(Starting, new EventArgs());

            _resetEvent.Reset();
            _serviceThread = new Thread(StartService);
            _serviceThread.Start();

            OnEvent(Started, new EventArgs());
        }

        public void Stop()
        {
            if (_serviceThread == null)
                return;

            OnEvent(Stopping, new EventArgs());

            _resetEvent.Set();
            _serviceThread.Join();

            OnEvent(Stopped, new EventArgs());
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
                {
                    _action(Encoding.ASCII.GetString(buf, 0, msgLen));
                }
            }
            catch(ObjectDisposedException e)
            {
                OnEvent(SocketError, new ErrorEventArgs(e));
            }
            catch(SocketException e)
            {
                OnEvent(SocketError, new ErrorEventArgs(e));
            }
            catch (Exception e)
            {
                OnEvent(Error, new ErrorEventArgs(e));
            }
            finally
            {
                Interlocked.Decrement(ref _requestCount);
            }
        }
    }
}