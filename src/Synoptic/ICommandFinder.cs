using System;
using System.Reflection;

namespace Synoptic
{
    internal interface ICommandFinder
    {
        CommandManifest FindInAssembly(Assembly assembly);
        CommandManifest FindInType(Type type);
    }
}