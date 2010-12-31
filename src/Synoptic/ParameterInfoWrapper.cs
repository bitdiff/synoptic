using System;
using System.Linq;
using System.Reflection;

namespace ConsoleWrapper.Synoptic
{
    public class ParameterInfoWrapper
    {
        public ParameterInfoWrapper(ParameterInfo parameter)
        {
            Name = parameter.Name;

            var attributes = parameter.GetCustomAttributes(typeof(CommandParameterAttribute), true);
            if (attributes.Length > 0)
            {
                var commandParameter = (CommandParameterAttribute)attributes.First();
                Prototype = commandParameter.Prototype;

                Description = Description.GetNewIfValid(commandParameter.Description);
                Name = Name.GetNewIfValid(commandParameter.Name);
            }

            IsOptionValueRequired = parameter.ParameterType != typeof(bool);
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Prototype { get; private set; }
        public bool IsOptionValueRequired { get; private set; }
        
        public string GetOptionPrototype()
        {
            return PrototypeGenerator.ToOptionPrototype(this);
        }
        
        public string GetOptionPrototypeHelp()
        {
            return GetOptionPrototype() + (IsOptionValueRequired ? "<VALUE>" : String.Empty);
        }
    }
}