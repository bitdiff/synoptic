using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Synoptic.Service
{
    public class UdpServer : IDisposable
    {
        private Socket _socket;
        private CancellationToken _token;
        private const int BufferSize = 256;
        public event EventHandler<SocketReceiveErrorEventArgs> ReceiveError;
        public event EventHandler<UdpMessageReceivedEventArgs> MessageReceived;

        private void OnReceiveError(SocketReceiveErrorEventArgs e)
        {
            var handler = ReceiveError;

            if (handler != null)
                handler(this, e);
        }

        private void OnMessageReceived(UdpMessageReceivedEventArgs e)
        {
            var handler = MessageReceived;

            if (handler != null)
                handler(this, e);
        }

        public void Start(IPEndPoint localEndPoint, CancellationToken token)
        {
            _token = token;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(localEndPoint);

            EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

            var args = new SocketAsyncEventArgs { RemoteEndPoint = remote };

            args.SetBuffer(new byte[BufferSize], 0, BufferSize);
            args.Completed += CompletedAsync;

            _socket.ReceiveMessageFromAsync(args);
        }

        private void CompletedAsync(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0)
            {
                if (e.SocketError == SocketError.Success)
                {
                    var message = Encoding.ASCII.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                    OnMessageReceived(new UdpMessageReceivedEventArgs(message));
                }
                else
                {
                    OnReceiveError(new SocketReceiveErrorEventArgs((Int32)e.SocketError, e.LastOperation));
                }
            }

            try
            {
                // Don't accept next message if cancellation is signaled.
                if (!_token.IsCancellationRequested)
                    _socket.ReceiveMessageFromAsync(e);
            }
            // Caused if socket is closed.
            catch (ObjectDisposedException)
            {
            }
        }

        public void Dispose()
        {
            if (_socket != null)
                _socket.Dispose();
        }
    }
}