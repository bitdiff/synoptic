namespace Synoptic.Service
{
    public interface IWorker<in T>
    {
        void Run(T message);
    }
}