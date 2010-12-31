using System;
using System.Reflection;

namespace Synoptic
{
    internal class CommandFinder : ICommandFinder
    {
        public CommandManifest FindInAssembly(Assembly assembly)
        {
            var manifest = new CommandManifest();

            foreach (var type in assembly.GetTypes())
            {
                manifest.Commands.AddRange(FindInType(type).Commands);
            }

            return manifest;
        }

        public CommandManifest FindInType(Type type)
        {
            var manifest = new CommandManifest();
            var methods = ReflectionUtilities.RetrieveMethodsWithAttribute<CommandAttribute>(type);

            foreach (var method in methods)
            {
                manifest.Commands.Add(GetCommand(method));
            }

            return manifest;
        }

        private Command GetCommand(MethodInfo methodInfo)
        {
            var wrapper = new MethodInfoWrapper(methodInfo);
            Command command = new Command(wrapper.Name, wrapper.Description, wrapper.LinkedToMethod);

            return command;
        }
    }
}