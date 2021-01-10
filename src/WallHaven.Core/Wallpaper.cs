using HtmlAgilityPack;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using WallHaven.Core.Models;
using WallHaven.Logger;
using _Logger = WallHaven.Logger.Logger;

namespace WallHaven.Core
{
    public class Wallpaper : IWallpaper
    {
        private ILogger logger = new _Logger();
        private const string WALLHAVEN_URL = "https://wallhaven.cc/latest?page={0}";
        private int CURRENT_PAGE = 1;
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);


        private List<WallpaperInfo> GetWallpapers()
        {
            List<WallpaperInfo> wallpaperInfos = new List<WallpaperInfo>();
            Resolution resolution = ServiceInfo.Resolution;
            do
            {
                logger.Log($"Founded Wallpaper Count:{wallpaperInfos.Count}");
                if (wallpaperInfos.Count <= ServiceInfo.SearchLimit)
                {
                    CURRENT_PAGE++;
                    logger.Log($"Current Page Changed : {CURRENT_PAGE}");
                }
                HtmlWeb web = new HtmlWeb();
                var doc = web.Load(string.Format(WALLHAVEN_URL, CURRENT_PAGE));
                var figures = doc.DocumentNode.SelectNodes("//figure");
                foreach (var figure in figures)
                {
                    Thread.Sleep(1000);
                    HtmlDocument figureDocument = new HtmlDocument();
                    figureDocument.LoadHtml(figure.InnerHtml);
                    var resolutionElement = figureDocument.DocumentNode.SelectSingleNode("//span[@class='wall-res']");
                    var resolutionElementContent = resolutionElement.InnerText.Trim().Split('x');
                    int width = int.Parse(resolutionElementContent[0]);
                    int height = int.Parse(resolutionElementContent[1]);
                    if (width != resolution.Width || height != resolution.Height)
                        continue;
                    var wallpaperPreviewElement = figureDocument.DocumentNode.SelectSingleNode("//a[@class='preview']");
                    var wallpaperPreviewHref = wallpaperPreviewElement.Attributes["href"].Value;
                    var previewPage = web.Load(wallpaperPreviewHref);
                    var downloadWallpaperElement = previewPage.DocumentNode.SelectSingleNode("//img[@id='wallpaper']");
                    var downloadWallpaperUrl = downloadWallpaperElement.Attributes["src"].Value;
                    var wallpaperName = downloadWallpaperUrl.Substring(downloadWallpaperUrl.LastIndexOf("-") + 1);
                    WallpaperInfo wallpaperInfo = new WallpaperInfo()
                    {
                        Name = wallpaperName,
                        PreviewUrl = wallpaperPreviewHref,
                        Resolution = new Resolution()
                        {
                            Width = width,
                            Height = height
                        },
                        Url = downloadWallpaperUrl
                    };
                    wallpaperInfos.Add(wallpaperInfo);
                }
            } while (wallpaperInfos.Count <= ServiceInfo.SearchLimit);
            logger.Log($"{wallpaperInfos.Count} Wallpaper Found");
            CURRENT_PAGE = 1;
            return wallpaperInfos;
        }

        private bool WallpaperIsExists(WallpaperInfo wallpaperInfo)
        {
            if (!Directory.Exists(Path.Combine(ServiceInfo.WallpaperDirectory)))
                Directory.CreateDirectory(Path.Combine(ServiceInfo.WallpaperDirectory));
            return File.Exists(Path.Combine(ServiceInfo.WallpaperDirectory, wallpaperInfo.Name));
        }


        public void ChangeWallpaper()
        {
            try
            {
                var wallpapers = GetWallpapers();
                Random random = new Random();
                var selectedWallpaper = wallpapers[random.Next(0, wallpapers.Count)];
                if (WallpaperIsExists(selectedWallpaper))
                {
                    SetWallpaper(selectedWallpaper);
                    return;
                }
                else
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(new Uri(selectedWallpaper.Url), Path.Combine(ServiceInfo.WallpaperDirectory, selectedWallpaper.Name));
                        SetWallpaper(selectedWallpaper);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }           
        }

        public void SetWallpaper(WallpaperInfo wallpaperInfo)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", 2.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
            0,
            Path.Combine(ServiceInfo.WallpaperDirectory, wallpaperInfo.Name),
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            logger.Log("Wallpaper Changed.");
        }

        public void Dispose()
        {

        }
    }
}
