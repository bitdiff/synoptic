using System;
using System.Threading;

namespace Synoptic.Service.Demo
{
    class MyDaemon : IDaemon
    {
        private const string Tag = "MyDaemon";
        private readonly ILogger _log;
        private readonly ManualResetEvent _resetEvent;
        private Thread _thread;

        public MyDaemon(ILogger log, ManualResetEvent resetEvent)
        {
            _log = log;
            _resetEvent = resetEvent;
        }

        public void Start()
        {
            _resetEvent.Reset();

            _log.LogInfo(Tag, "Starting..");

            _thread = new Thread(() =>
            {
                while (true)
                {
                    _log.LogInfo(Tag, "working... ({0})", DateTime.UtcNow);
                    if (WaitHandle.WaitAny(new[] { _resetEvent }, 3000) == 0)
                    {
                        _log.LogInfo(Tag, "Ending work..");
                        break;
                    }
                }
            });

            _thread.Start();
        }

        public void Stop()
        {
            _resetEvent.Set();
            _log.LogInfo(Tag, "Waiting for work to finish..");

            if (_thread != null)
                _thread.Join();

            _log.LogInfo(Tag, "Stopping..");
        }
    }
}