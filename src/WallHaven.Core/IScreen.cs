using System;
using WallHaven.Core.Models;

namespace WallHaven.Core
{
    public interface IScreen : IDisposable
    {
        Resolution GetResolution();
    }
}
