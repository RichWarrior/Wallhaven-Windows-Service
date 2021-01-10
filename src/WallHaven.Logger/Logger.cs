using System;
using System.Diagnostics;
using System.IO;

namespace WallHaven.Logger
{
    public class Logger : ILogger
    {
        public void Dispose()
        {
        }

        private string GetLogFile()
        {
            try
            {
                if (!Directory.Exists(Settings.LogPath))
                    Directory.CreateDirectory(Settings.LogPath);
                DateTime now = DateTime.Now;
                return Path.Combine(Settings.LogPath, $"{now.Day}_{now.Month}_{now.Year}.log");
            }
            catch (Exception)
            {
            }
            return "";
        }

        public void Log(string message)
        {
            string logPath = GetLogFile();
            try
            {
                using (var sw = File.AppendText(logPath))
                {
                    sw.WriteLine($"{DateTime.Now}: {message}");
                    Console.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void Log(BaseException ex)
        {
            Log(ex.Message);
        }
    }
}
