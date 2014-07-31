using System;

namespace Synoptic.Service
{
    public static class AggregateExceptionExtensions
    {
        public static AggregateException WithoutCancellations(this AggregateException exception)
        {
            try { exception.Handle(e => e is OperationCanceledException); }
            catch (AggregateException ex) { return ex; }
            return new AggregateException();
        }
    }
}