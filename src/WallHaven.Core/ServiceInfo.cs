using System.IO;
using System.Xml.Serialization;
using WallHaven.Core.Models;
using WallHaven.Logger.Exceptions;

namespace WallHaven.Core
{
    public static class ServiceInfo
    {
        public static string ServiceDirectory { get; set; }
        public static int RefreshMinute { get; set; }
        public static Resolution Resolution { get; set; }
        public static string WallpaperDirectory { get; set; }
        public static int SearchLimit { get; set; }

        public static void GetServiceInfo(string ServiceDirectory)
        {
            ServiceInfo.ServiceDirectory = ServiceDirectory;
            ServiceInfo.WallpaperDirectory = Path.Combine(ServiceDirectory, "Wallpapers");
            string settingsFile = Path.Combine(ServiceDirectory, "settings.xml");

            if (!File.Exists(settingsFile))
                throw new FileNotFound("settings.xml");

            using (Stream stream = new FileStream(settingsFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(Models.ServiceInfo));

                Models.ServiceInfo si = (Models.ServiceInfo)xmlserializer.Deserialize(stream);

                if (si == null)
                    return;

                RefreshMinute = si.RefreshMinute;
                SearchLimit = si.SearchLimit.HasValue ? si.SearchLimit.Value : 10;
            }
        }
    }
}
