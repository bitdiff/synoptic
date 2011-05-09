using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Synoptic
{
    public interface IMiddleware<T, TOut>
    {
        TOut Process(T context, Func<T, TOut> executeNext);
    }

    public class Pipeline<T, TOut>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IList<IMiddleware<T, TOut>> _filters = new List<IMiddleware<T, TOut>>();
        private int _current;

        public Pipeline()
        //            : this(new ActivatorServiceProvider())
        { }

        public Pipeline<T, TOut> Add(Type filterType)
        {
            Add((IMiddleware<T, TOut>)_serviceProvider.GetService(filterType));
            return this;
        }

        public Pipeline<T, TOut> Add<TFilter>() where TFilter : IMiddleware<T, TOut>
        {
            Add(typeof(TFilter));
            return this;
        }

        public int Count
        {
            get { return _filters.Count; }
        }

        public Pipeline<T, TOut> Add(IMiddleware<T, TOut> filter)
        {
            _filters.Add(filter);
            return this;
        }

        public TOut Execute(T input)
        {
            GetNext = () => _current < _filters.Count
                ? x => _filters[_current++].Process(x, GetNext())
                : new Func<T, TOut>(c => { throw new EndOfChainException(); });

            return GetNext().Invoke(input);
        }

        private Func<Func<T, TOut>> GetNext { get; set; }
    }

    public class EndOfChainException : Exception
    {
        public EndOfChainException()
            : base("Next filter does not exist."
            + " Use 'Finally' or a filter that short-circuits before reaching the end of the chain")
        { }
    }

    public static class AssemblyExtensions
    {
        public static Pipeline<T, TOut> AddFiltersIn<T, TOut>(this Pipeline<T, TOut> pipeline, Assembly assembly)
        {
            return AddFiltersIn(pipeline, Assembly.GetCallingAssembly(), t => true);
        }

        public static Pipeline<T, TOut> AddFiltersIn<T, TOut>(this Pipeline<T, TOut> pipeline, string filterNamespace)
        {
            return AddFiltersIn(pipeline, Assembly.GetCallingAssembly(), filterNamespace);
        }

        public static Pipeline<T, TOut> AddFiltersIn<T, TOut>(
            this Pipeline<T, TOut> pipeline,
            Assembly assembly,
            string filterNamespace)
        {
            return AddFiltersIn(pipeline, assembly, t => t.Namespace == filterNamespace);
        }

        public static Pipeline<T, TOut> AddFiltersIn<T, TOut>(
            this Pipeline<T, TOut> pipeline,
            Assembly assembly,
            Func<Type, bool> predicate)
        {
            var filterTypes = assembly
                .GetTypes()
                .Where(t => typeof(IMiddleware<T, TOut>).IsAssignableFrom(t));

            foreach (var filterType in filterTypes.Where(predicate))
            {
                pipeline.Add(filterType);
            }

            return pipeline;
        }
    }

    public static class PipelineExtensions
    {
        /// <summary>
        /// Convenience method to add a short-circuiting filter to the end of the chain
        /// </summary>
        public static Pipeline<T, TOut> Finally<T, TOut>(this Pipeline<T, TOut> pipeline, Func<T, TOut> func)
        {
            pipeline.Add(new ShortCircuit<T, TOut>(func));
            return pipeline;
        }

        private class ShortCircuit<T, TOut> : IMiddleware<T, TOut>
        {
            private readonly Func<T, TOut> _func;

            public ShortCircuit(Func<T, TOut> func)
            {
                this._func = func;
            }

            public TOut Process(T context, Func<T, TOut> executeNext)
            {
                return _func.Invoke(context);
            }
        }
    }
}