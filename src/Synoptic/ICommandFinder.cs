using System;
using System.Reflection;

namespace Synoptic
{
    public interface ICommandFinder
    {
        CommandManifest FindInAssembly(Assembly assembly);
        CommandManifest FindInType(Type type);
    }
}