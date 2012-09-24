using System;

namespace Synoptic
{
    internal class ActivatorCommandDependencyResolver : ICommandDependencyResolver
    {
        public object Resolve(Type serviceType)
        {
            return Activator.CreateInstance(serviceType);
        }
    }
}