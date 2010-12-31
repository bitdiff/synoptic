using System;

namespace Synoptic
{
    internal class ActivatorDependencyResolver : IDependencyResolver
    {
        public object Resolve(Type serviceType)
        {
            return Activator.CreateInstance(serviceType);
        }
    }
}