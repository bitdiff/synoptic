namespace Synoptic.Service
{
    public interface IWindowsServiceConfiguration
    {
        string ServiceName { get; }
        string CommandLineArguments { get; }
        string Description { get; }
        string DisplayName { get; }
    }
}