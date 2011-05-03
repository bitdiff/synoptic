using System;
using System.Threading;

namespace Synoptic.Service
{
    public class LoopyDaemon : IDaemon
    {
        private readonly Action<DaemonEvent> _action;
        private readonly ManualResetEvent _reset = new ManualResetEvent(false);
        private Thread _thread;
        public event EventHandler<EventArgs> Stopped = (s, e) => { };

        public void OnStop(EventArgs e)
        {
            var handle = Stopped;
            if (handle != null)
                handle(this, e);
        }

        public LoopyDaemon(Action<DaemonEvent> action)
        {
            _action = action;
        }

        public void Start()
        {
            var daemonEvent = new DaemonEvent
                               {
                                   Interval = 1000
                               };

            _thread = new Thread(() =>
                                     {
                                         while (true)
                                         {
                                             _action(daemonEvent);
                                             if (WaitHandle.WaitAny(new[] { _reset }, daemonEvent.Interval) == 0)
                                                 break;
                                         }
                                     });

            _thread.Start();
        }

        public void Stop()
        {
            _reset.Set();
            OnStop(new EventArgs());
            _thread.Join();
        }
    }
}