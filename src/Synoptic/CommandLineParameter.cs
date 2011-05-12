namespace Synoptic
{
    internal class CommandLineParameter
    {
        public CommandLineParameter(string name,
                                    string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public object Value { get; private set; }
    }
}