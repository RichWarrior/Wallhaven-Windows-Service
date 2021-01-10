using System;
using System.Management;
using WallHaven.Core.Models;

namespace WallHaven.Core
{
    public class Screen : IScreen
    {
        public Resolution GetResolution()
        {
            Resolution resolution = new Resolution();
            var managementScope = new ManagementScope();
            managementScope.Connect();
            var query = new System.Management.ObjectQuery("SELECT CurrentHorizontalResolution, CurrentVerticalResolution FROM Win32_VideoController");
            var searcher = new System.Management.ManagementObjectSearcher(managementScope, query);
            var records = searcher.Get();
            int width = 0;
            int height = 0;
            foreach (var record in records)
            {
                if (!int.TryParse(record.GetPropertyValue("CurrentHorizontalResolution").ToString(), out width))
                {
                    throw new Exception("Throw some exception");
                }
                if (!int.TryParse(record.GetPropertyValue("CurrentVerticalResolution").ToString(), out height))
                {
                    throw new Exception("Throw some exception");
                }
            }

            resolution.Height = height;
            resolution.Width = width;
            return resolution;
        }


        public void Dispose()
        {
        }
    }
}
