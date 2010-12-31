namespace ConsoleWrapper.Synoptic
{
    public interface ICommandResolver
    {
        Command Resolve(CommandManifest manifest, string commandName);
    }
}