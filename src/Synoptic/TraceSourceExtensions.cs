using System.Diagnostics;

namespace Synoptic
{
    public static class TraceSourceExtensions
    {
        public static void Error(this TraceSource traceSource, string format, params object[] args)
        {
            Write(traceSource, TraceEventType.Error, format, args);
        }

        public static void Critical(this TraceSource traceSource, string format, params object[] args)
        {
            Write(traceSource, TraceEventType.Critical, format, args);
        }

        public static void Warning(this TraceSource traceSource, string format, params object[] args)
        {
            Write(traceSource, TraceEventType.Warning, format, args);
        }

        public static void Information(this TraceSource traceSource, string format, params object[] args)
        {
            Write(traceSource, TraceEventType.Information, format, args);
        }

        public static void Resume(this TraceSource traceSource, string format, params object[] args)
        {
            Write(traceSource, TraceEventType.Resume, format, args);
        }

        public static void Start(this TraceSource traceSource, string format, params object[] args)
        {
            Write(traceSource, TraceEventType.Start, format, args);
        }

        public static void Stop(this TraceSource traceSource, string format, params object[] args)
        {
            Write(traceSource, TraceEventType.Stop, format, args);
        }

        public static void Suspend(this TraceSource traceSource, string format, params object[] args)
        {
            Write(traceSource, TraceEventType.Suspend, format, args);
        }

        public static void Transfer(this TraceSource traceSource, string format, params object[] args)
        {
            Write(traceSource, TraceEventType.Transfer, format, args);
        }

        public static void Verbose(this TraceSource traceSource, string format, params object[] args)
        {
            Write(traceSource, TraceEventType.Verbose, format, args);
        }

        private static void Write(TraceSource traceSource, TraceEventType traceEventType, string format, params object[] args)
        {
            traceSource.TraceEvent(traceEventType, 1, format, args);
        }
    }
}