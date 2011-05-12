using System;

namespace Synoptic
{
    internal class Command
    {
        public Command(string name, string description, Type linkedToType)
        {
            Name = name;
            Description = description;
            LinkedToType = linkedToType;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Type LinkedToType { get; private set; }
    }
}