using System;
using StructureMap;

namespace Synoptic.Demo
{
    public class StructureMapCommandDependencyResolver : ICommandDependencyResolver
    {
        private readonly IContainer _container;

        public StructureMapCommandDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public object Resolve(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }
    }
}