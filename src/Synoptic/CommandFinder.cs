using System.Collections.Generic;
using System.Reflection;

namespace Synoptic
{
    internal class CommandFinder
    {
        public IEnumerable<Command> FindInAssembly(Assembly assembly)
        {
            var commandTypes = ReflectionUtilities.RetrieveTypesWithAttribute<CommandAttribute>(assembly);

            foreach (var type in commandTypes)
            {
                var typeInfo = new CommandTypeWrapper(type);
                yield return new Command(typeInfo.Name, typeInfo.Description, typeInfo.LinkedToType);
            }
        }
    }
}