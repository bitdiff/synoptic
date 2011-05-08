using System;

namespace Synoptic
{
    public class Command
    {
//        private readonly List<ParameterInfoWrapper> _parameters = new List<ParameterInfoWrapper>();

        public Command(string name, string description, Type linkedToType)
        {
            Name = name;
            Description = description;
            LinkedToType = linkedToType;

//            foreach (var parameter in linkedToMethod.GetParameters())
//            {
//                _parameters.Add(new ParameterInfoWrapper(parameter));
//            }
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Type LinkedToType { get; private set; }
//        public List<ParameterInfoWrapper> Parameters { get { return _parameters; } }
    }
}