using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using NLog;

namespace FullscreenUtility
{
    public static class SettingsFile
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Settings ReadSettings()
        {
            var settingsPath = GetSavePath();
            if (File.Exists(settingsPath))
            {
                Logger.Info($"Reading settings from: {settingsPath}");
                return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));
            }

            return new Settings
            {
                Version = 1,
                CursorLockEnabled = true,
                MouseTransparencyEnabled = true
            };
        }

        public static void WriteSettings(Settings settings)
        {
            var settingsPath = GetSavePath();
            Logger.Info($"Writing settings to: {settingsPath}");
            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings));
        }

        private static string GetSavePath()
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(currentDir))
            {
                throw new Exception("Unable to determine current directory!");
            }

            return Path.Combine(currentDir, "settings.json");
        }
    }

    public class Settings
    {
        public int Version { get; set; }
        public bool CursorLockEnabled { get; set; }
        public bool MouseTransparencyEnabled { get; set; }
    }
}