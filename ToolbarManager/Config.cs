using System;
using System.IO;
using System.Xml.Serialization;
using VRage.FileSystem;
using VRage.Input;
using VRage.Utils;

namespace ToolbarManager
{
    public class PluginConfig
    {
        public bool KeepProfileSearchText = false;
        public string LatestProfileSearchText = "";

        public bool KeepBlockSearchText = false;
        public string LatestBlockSearchText = "";

        public MyKeys BlockSearchKey = MyKeys.OemPipe;
    }

    public static class Config
    {
        public static PluginConfig Data { get; private set; } = new PluginConfig();

        private static readonly string ConfigPath = Path.Combine(MyFileSystem.UserDataPath, "Storage", "ToolbarManager.cfg");
        private static readonly XmlSerializer ConfigSerializer = new XmlSerializer(typeof(PluginConfig));

        public static bool Save()
        {
            try
            {
                using (var writer = File.CreateText(ConfigPath))
                {
                    ConfigSerializer.Serialize(writer, Data);
                }
            }
            catch (Exception e)
            {
                MyLog.Default.Error($"Could not save config: {ConfigPath} | {e}");
                return false;
            }

            return true;
        }

        public static void Load()
        {
            if (!File.Exists(ConfigPath))
            {
                MyLog.Default.Error($"Could not find config file: {ConfigPath}");
                return;
            }

            try
            {
                using (var reader = new StreamReader(ConfigPath))
                {
                    Data = (PluginConfig) ConfigSerializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                MyLog.Default.Error($"Failed to load config file: {ConfigPath} | {e}");
            }
        }
    }
}