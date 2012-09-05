using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Synoptic.Service
{
    public abstract class PollingDaemonBase : IDaemon
    {
        private readonly IDaemonLogger _logger;
        private readonly TimeSpan _interval;
        private readonly int? _preemptOnPort;
        private const string LogTag = "polling_daemon";

        private CancellationTokenSource _tokenSource;
        private readonly List<Task> _tasks = new List<Task>();

        private readonly ManualResetEventSlim _preemptIntervalEvent = new ManualResetEventSlim(false);
        private UdpServer _server;

        protected PollingDaemonBase(IDaemonLogger logger, TimeSpan interval, int? preemptOnPort = null)
        {
            _logger = logger;
            _interval = interval;
            _preemptOnPort = preemptOnPort;
        }

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            _tasks.Add(Task.Factory.StartNew(() => Execute(token),
                                             token,
                                             TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent,
                                             TaskScheduler.Current)
                           .ContinueWith(t => OnError(t.Exception),
                                         TaskContinuationOptions.OnlyOnFaulted));

            if (_preemptOnPort.HasValue)
                _tasks.Add(Task.Factory.StartNew(() => PreemptInterval(token),
                                                 token,
                                                 TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent,
                                                 TaskScheduler.Current)
                               .ContinueWith(t => OnError(t.Exception),
                                             TaskContinuationOptions.OnlyOnFaulted));

        }

        protected virtual bool ShouldPreempt(string message)
        {
            return true;
        }

        private void PreemptInterval(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            ConfigureUdp();
        }

        private void ConfigureUdp()
        {
            if (!_preemptOnPort.HasValue)
                return;

            var localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _preemptOnPort.Value);

            _server = new UdpServer();
            _server.Start(localEndPoint, _tokenSource.Token);

            _server.ReceiveError += (s, e) => _logger.Info(LogTag, "Socket error {0} during {1}.", e.ErrorCode, e.LastOperation);
            _server.MessageReceived += (s, e) =>
                                           {
                                               if (ShouldPreempt(e.Message))
                                                   _preemptIntervalEvent.Set();
                                           };
        }

        public void Stop()
        {
            _tokenSource.Cancel();

            _logger.Debug(LogTag, "Waiting to end...");

            try
            {
                Task.WaitAll(_tasks.ToArray(), _tokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.Debug(LogTag, "Task cancelled.");
            }

            if (_server != null)
            {
                _server.Dispose();
                _logger.Debug(LogTag, "Server disposed.");
            }

            _logger.Debug(LogTag, "Stopped.");
        }

        private void Execute(CancellationToken ct)
        {
            for (; ; )
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    Run(_tokenSource);
                }
                catch (Exception e)
                {
                    OnError(e);
                }

                var r = WaitHandle.WaitAny(new[] { ct.WaitHandle, _preemptIntervalEvent.WaitHandle }, _interval);

                // Cancelled.
                if (r == 0)
                    break;

                // Pre-empted.
                if (r == 1)
                    _preemptIntervalEvent.Reset();
            }
        }

        public abstract void OnError(Exception e);
        public abstract void Run(CancellationTokenSource cts);
    }
}