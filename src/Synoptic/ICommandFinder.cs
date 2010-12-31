using System;
using System.Reflection;

namespace ConsoleWrapper.Synoptic
{
    public interface ICommandFinder
    {
        CommandManifest FindInAssembly(Assembly assembly);
        CommandManifest FindInType(Type type);
    }
}