using System.Runtime.InteropServices;

namespace WallHaven.Core
{
    public class Os : IOs
    {
        public bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public void Dispose()
        {
            
        }
    }
}
