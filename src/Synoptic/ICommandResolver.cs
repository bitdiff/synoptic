namespace Synoptic
{
    internal interface ICommandResolver
    {
        CommandAction Resolve(CommandActionManifest actionManifest, string commandName);
    }
}