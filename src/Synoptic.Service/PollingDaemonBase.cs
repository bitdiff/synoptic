using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Synoptic.Service
{
    public abstract class PollingDaemonBase : IServiceDaemon
    {
        private readonly IDaemonLogger _logger;
        private readonly TimeSpan _interval;
        private readonly IPollingPreempter _preempter;

        private CancellationTokenSource _tokenSource;
        private readonly List<Task> _tasks = new List<Task>();

        private readonly ManualResetEventSlim _preemptIntervalEvent = new ManualResetEventSlim(false);

        protected PollingDaemonBase(IDaemonLogger logger, TimeSpan interval, IPollingPreempter preempter = null)
        {
            _logger = logger;
            _interval = interval;
            _preempter = preempter;
        }

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();
            var cancellationToken = _tokenSource.Token;

            var task = new Task(() => Execute(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning);

            ConfigureTaskForErrors(task, OnError, cancellationToken);

            _tasks.Add(task);

            task.Start(TaskScheduler.Current);

            if (_preempter != null)
            {
                StartPreempter(cancellationToken);
            }

            _logger.Debug(LogTag, "Started.");

            OnStarted();
        }

        public virtual void OnStarted() { }

        private void StartPreempter(CancellationToken cancellationToken)
        {
            var preemptTask = new Task(() =>
            {
                _preempter.ShouldPreempt += Preempted;
                _preempter.Listen(cancellationToken);

                cancellationToken.WaitHandle.WaitOne();
            }, cancellationToken, TaskCreationOptions.LongRunning);

            ConfigureTaskForErrors(preemptTask, _preempter.OnError, cancellationToken);

            _tasks.Add(preemptTask);

            preemptTask.Start(TaskScheduler.Current);
        }

        private void ConfigureTaskForErrors(Task task, Action<Exception, CancellationToken> errorHandler, CancellationToken cancellationToken)
        {
            task.ContinueWith(t => errorHandler(t.Exception, cancellationToken),
                TaskContinuationOptions.OnlyOnFaulted |
                TaskContinuationOptions.ExecuteSynchronously);
        }

        void Preempted(object sender, EventArgs e)
        {
            _preemptIntervalEvent.Set();
        }

        public void Stop()
        {
            _tokenSource.Cancel();

            try
            {
                if (_preempter != null)
                {
                    _logger.Debug(LogTag, "Waiting for preempter to stop...");
                    _preempter.Stop();
                }

                _logger.Debug(LogTag, "Waiting for task(s) to end...");

                Task.WaitAll(_tasks.ToArray(), _tokenSource.Token);
            }
            catch (OperationCanceledException) { }
            catch (AggregateException e)
            {
                foreach (var exception in e.WithoutCancellations().InnerExceptions)
                {
                    _logger.Error(LogTag, exception);
                }
            }

            OnStopped();

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
                    OnError(e, ct);
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

        public abstract void OnError(Exception e, CancellationToken cancellationToken);
        public abstract void Run(CancellationTokenSource cts);
        public abstract void OnStopped();
        public abstract string LogTag { get; }
    }
}