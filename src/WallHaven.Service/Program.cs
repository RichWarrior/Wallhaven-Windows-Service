using System;
using System.IO;
using Topshelf;
using WallHaven.Core;
using WallHaven.Logger;
using WallHaven.Logger.Exceptions;
using _Logger = WallHaven.Logger.Logger;
using _Timer = System.Threading.Timer;

namespace WallHaven.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = new _Logger();
            try
            {
                IOs os = new Os();
                if (!os.IsWindows)
                {
                    logger.Log(new NotSupportedPlatform());
                    return;
                }
                Settings.LogPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                ServiceInfo.GetServiceInfo(Directory.GetCurrentDirectory());
                HostFactory.Run(hostConfig =>
                {
                    hostConfig.Service<WallHavenService>(serviceConfig =>
                    {
                        serviceConfig.ConstructUsing(() => new WallHavenService());
                        serviceConfig.WhenStarted(s => s.Start());
                        serviceConfig.WhenStopped(s => s.Stop());
                    });
                    hostConfig.RunAsLocalSystem();
                    hostConfig.SetServiceName("Wallhaven Wallpaper Service");
                    hostConfig.SetDisplayName("Wallhaven Wallpaper Service");
                    hostConfig.SetDescription("Wallhaven wallpaper service for windows");
                });
            }
            catch (Exception ex)
            {
                if (ex is BaseException)
                    logger.Log(ex.Message);
            }

        }
    }

    public class WallHavenService
    {

        _Timer timer;
        object state;

        IWallpaper Wallpaper;
        IScreen Screen;

        public WallHavenService()
        {
            Wallpaper = new Wallpaper();
            Screen = new Screen();
            ServiceInfo.Resolution = Screen.GetResolution();
        }

        public void Start()
        {
            if (timer == null)
            {
                state = new object();
                timer = new _Timer(DoWork, state, (int)TimeSpan.Zero.TotalMilliseconds, (int)TimeSpan.FromSeconds(10).TotalMilliseconds);
            }
        }


        public void Stop()
        {

        }

        private void DoWork(object state)
        {
            lock (Wallpaper)
            {
                Wallpaper.ChangeWallpaper();
            }
        }

    }
}
