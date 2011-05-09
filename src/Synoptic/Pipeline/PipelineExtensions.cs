using System;

namespace Synoptic.Pipeline
{
    public static class PipelineExtensions
    {
        public static Pipeline<T, TOut> Finally<T, TOut>(this Pipeline<T, TOut> pipeline, Func<T, TOut> func)
        {
            pipeline.Add(new ShortCircuit<T, TOut>(func));
            return pipeline;
        }

        private class ShortCircuit<T, TOut> : IFilter<T, TOut>
        {
            private readonly Func<T, TOut> _func;

            public ShortCircuit(Func<T, TOut> func)
            {
                _func = func;
            }

            public TOut Process(T context, Func<T, TOut> executeNext)
            {
                return _func.Invoke(context);
            }
        }
    }
}