namespace Synoptic.Service
{
    public class ComboDaemon : IDaemon
    {
        private readonly IDaemon[] _daemons;

        public ComboDaemon(params IDaemon[] daemons)
        {
            _daemons = daemons;
        }

        public void Start()
        {
            foreach(var d in _daemons)
            {
                d.Start();
            }
        }

        public void Stop()
        {
            foreach (var d in _daemons)
            {
                d.Stop();
            }
        }
    }
}