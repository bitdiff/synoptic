using System;

namespace Synoptic
{
    public interface IDependencyResolver
    {
        object Resolve(Type serviceType);
    }
}