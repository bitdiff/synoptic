using System;

namespace Synoptic
{
    public interface ICommandDependencyResolver
    {
        object Resolve(Type serviceType);
    }
}