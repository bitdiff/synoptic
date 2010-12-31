namespace Synoptic
{
    internal interface ICommandResolver
    {
        Command Resolve(CommandManifest manifest, string commandName);
    }
}