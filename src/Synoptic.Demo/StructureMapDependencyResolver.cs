using System;
using Ninject;
using StructureMap;

namespace Synoptic.Demo
{
    public class StructureMapDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public StructureMapDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public object Resolve(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }
    }

    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object Resolve(Type serviceType)
        {
            return _kernel.Get(serviceType);
        }
    }
}