using System;
using System.Linq;

namespace Synoptic
{
    internal class CommandTypeWrapper
    {
        public CommandTypeWrapper(Type type)
        {
            LinkedToType = type;
            
            var attributes = type.GetCustomAttributes(typeof(CommandAttribute), true);
            if (attributes.Length > 0)
            {
                var commandAttribute = (CommandAttribute)attributes.First();

                Description = Description.GetNewIfValid(commandAttribute.Description);
                Name = Name.GetNewIfValid(commandAttribute.Name);
            }
        }

        public string Name { get; set; }
        public Type LinkedToType { get; set; }
        public string Description { get; set; }
    }
}