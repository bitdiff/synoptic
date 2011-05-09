using System;
using System.Collections.Generic;

namespace Synoptic.Pipeline
{
    public class Pipeline<T, TOut>
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly IList<IFilter<T, TOut>> _filters = new List<IFilter<T, TOut>>();
        private int _current;

        public Pipeline(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public Pipeline<T, TOut> Add(Type filterType)
        {
            Add((IFilter<T, TOut>)_dependencyResolver.Resolve(filterType));
            return this;
        }

        public Pipeline<T, TOut> Add<TFilter>() where TFilter : IFilter<T, TOut>
        {
            Add(typeof(TFilter));
            return this;
        }

        public int Count
        {
            get { return _filters.Count; }
        }

        public Pipeline<T, TOut> Add(IFilter<T, TOut> filter)
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
}