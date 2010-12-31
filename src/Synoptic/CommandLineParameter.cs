namespace ConsoleWrapper.Synoptic
{
    public class CommandLineParameter
    {
        public CommandLineParameter(string name,
                                    string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }
    }
}