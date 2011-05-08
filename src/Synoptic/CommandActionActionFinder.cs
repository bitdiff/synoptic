using System;
using System.Reflection;

namespace Synoptic
{
    internal class CommandActionActionFinder : ICommandActionFinder
    {
        public CommandActionManifest FindInAssembly(Assembly assembly)
        {
            var manifest = new CommandActionManifest();

            foreach (var type in assembly.GetTypes())
            {
                manifest.Commands.AddRange(FindInType(type).Commands);
            }

            return manifest;
        }

        public CommandActionManifest FindInType(Type type)
        {
            var manifest = new CommandActionManifest();
            var methods = ReflectionUtilities.RetrieveMethodsWithAttribute<CommandActionAttribute>(type);

            foreach (var method in methods)
            {
                manifest.Commands.Add(GetCommand(method));
            }

            return manifest;
        }

        private CommandAction GetCommand(MethodInfo methodInfo)
        {
            var wrapper = new MethodInfoWrapper(methodInfo);
            CommandAction commandAction = new CommandAction(wrapper.Name, wrapper.Description, wrapper.LinkedToMethod);

            return commandAction;
        }
    }
}