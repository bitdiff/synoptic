using System;

namespace Synoptic.Pipeline
{
    public interface IFilter<T, TOut>
    {
        TOut Process(T context, Func<T, TOut> executeNext);
    }
}