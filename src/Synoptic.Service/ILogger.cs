using System;

namespace Synoptic.Service
{
    public interface ILogger
    {
        void LogInfo(string name, string message, params object[] args);
        void LogException(string name, Exception exception, string message, params object[] args);
    }
}