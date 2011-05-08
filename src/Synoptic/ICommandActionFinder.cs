using System;
using System.Reflection;

namespace Synoptic
{
    internal interface ICommandActionFinder
    {
        CommandActionManifest FindInAssembly(Assembly assembly);
        CommandActionManifest FindInType(Type type);
    }
}