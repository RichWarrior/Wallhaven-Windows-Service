using System;

namespace WallHaven.Core
{
    public interface IOs : IDisposable
    {
        bool IsWindows { get; }
    }
}
