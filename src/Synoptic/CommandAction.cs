using System.Collections.Generic;
using System.Reflection;

namespace Synoptic
{
    public class CommandAction
    {
        private readonly List<ParameterInfoWrapper> _parameters = new List<ParameterInfoWrapper>();

        public CommandAction(string name, string description, MethodInfo linkedToMethod)
        {
            Name = name;
            Description = description;
            LinkedToMethod = linkedToMethod;

            foreach (var parameter in linkedToMethod.GetParameters())
            {
                _parameters.Add(new ParameterInfoWrapper(parameter));
            }
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public MethodInfo LinkedToMethod { get; private set; }
        public List<ParameterInfoWrapper> Parameters { get { return _parameters; } }
    }
}