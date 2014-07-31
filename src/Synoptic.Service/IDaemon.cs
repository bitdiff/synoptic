using System.Threading;

namespace Synoptic.Service
{
    public interface IDaemon
    {
        void Start(CancellationToken cancellationToken);
        void Stop();
    }
}