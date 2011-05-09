using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Synoptic
{
    internal class CommandActionFinder : ICommandActionFinder
    {
        public IEnumerable<CommandAction> FindInCommand(Command command)
        {
            return FindInType(command.LinkedToType);
        }

        private IEnumerable<CommandAction> FindInType(Type type)
        {
            var methods = ReflectionUtilities.RetrieveMethodsWithAttribute<CommandActionAttribute>(type);
            return methods.Select(GetCommand);
        }

        private CommandAction GetCommand(MethodInfo methodInfo)
        {
            var wrapper = new MethodInfoWrapper(methodInfo);
            CommandAction commandAction = new CommandAction(wrapper.Name, wrapper.Description, wrapper.LinkedToMethod, wrapper.IsDefault);

            return commandAction;
        }
    }
}