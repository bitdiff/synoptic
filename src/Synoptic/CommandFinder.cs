using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Synoptic
{
    internal class CommandFinder
    {
        public IEnumerable<Command> FindInAssembly(Assembly assembly)
        {
            var commandTypes = ReflectionUtilities.RetrieveTypesWithAttribute<CommandAttribute>(assembly);
            return commandTypes.Select(FindInType);
        }

        public Command FindInType(Type type)
        {
            var typeInfo = new CommandTypeWrapper(type);
            return new Command(typeInfo.Name, typeInfo.Description, typeInfo.LinkedToType);
        }
    }
}