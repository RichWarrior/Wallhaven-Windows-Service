using System;

namespace WallHaven.Logger
{
    public interface ILogger : IDisposable
    {
        void Log(string message);
        void Log(BaseException ex);
    }
}
