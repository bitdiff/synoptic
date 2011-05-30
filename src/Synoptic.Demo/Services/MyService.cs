namespace Synoptic.Demo.Services
{
    internal class MyService : IMyService
    {
        public string Hello(string message)
        {
            return "Hello " + message;
        }
    }
}