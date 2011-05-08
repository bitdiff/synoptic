using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Synoptic
{
    internal class CommandSelector
    {
        private readonly List<Command> _commands;

        public CommandSelector(List<Command> commands)
        {
            _commands = commands;
        }

        public Command Select(ref string[] args)
        {
            var arguments = new List<string>(args);
            var firstArg = arguments.First();
            arguments.RemoveAt(0);

            var commands = Match(firstArg);
            args = arguments.ToArray();

            if (commands.Count == 0)
                throw new CommandActionException(String.Format("Command not found: {0}", firstArg));

            if (commands.Count == 1)
                return commands.First();

            var ambiguous = string.Join(", ", commands.Select(c => c.Name).ToArray());
            throw new CommandActionException(String.Format("{0} is ambiguous with commands: {1}", firstArg, ambiguous));
        }

        private List<Command> StartingWith(string query)
        {
            return new List<Command>(_commands.Where(cmd => cmd.Name.StartsWith(query)).OrderBy(cmd => cmd.Name).ToList());
        }

        public List<Command> Match(string query)
        {
            var exact = _commands.FirstOrDefault(cmd => cmd.Name.Equals(query, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
                return new List<Command> { exact };

            return StartingWith(query);
        }
    }

    internal class CommandFinder
    {
        public CommandManifest FindInAssembly(Assembly assembly)
        {
            var manifest = new CommandManifest();
            var commandTypes = ReflectionUtilities.RetrieveTypesWithAttribute<CommandAttribute>(assembly);

            foreach (var type in commandTypes)
            {
                var typeInfo = new CommandTypeWrapper(type);
                manifest.Commands.Add(new Command(typeInfo.Name, typeInfo.Description, typeInfo.LinkedToType));
            }

            return manifest;
        }

//        public CommandActionManifest FindActionsInType(Type type)
//        {
//            var manifest = new CommandManifest();
//            var methods = ReflectionUtilities.RetrieveMethodsWithAttribute<CommandActionAttribute>(type);
//
//            foreach (var method in methods)
//            {
//                manifest.Commands.Add(GetCommand(method));
//            }
//
//            return manifest;
//        }
//
//        private Command GetCommand(MethodInfo methodInfo)
//        {
//            var wrapper = new MethodInfoWrapper(methodInfo);
//            CommandAction commandAction = new CommandAction(wrapper.Name, wrapper.Description, wrapper.LinkedToMethod);
//
//            return commandAction;
//        }
    }
}