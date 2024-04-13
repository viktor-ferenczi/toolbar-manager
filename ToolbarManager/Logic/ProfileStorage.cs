using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;
using Sandbox.Game.Screens.Helpers;
using ToolbarManager.Extensions;
using VRage.FileSystem;
using VRage.Utils;

namespace ToolbarManager.Logic
{
    public class ProfileStorage
    {
        private static string PluginDataDir => Path.Combine(MyFileSystem.UserDataPath, "ToolbarManager");

        private readonly MyToolbar currentToolbar;
        private readonly string profilesDir;

        public ProfileStorage(MyToolbar currentToolbar)
        {
            this.currentToolbar = currentToolbar;
            profilesDir = Path.Combine(PluginDataDir, currentToolbar.ToolbarType.ToString());
            Directory.CreateDirectory(profilesDir);
        }

        private string FormatPath(string name, string extension)
        {
            var sanitizedName = PathExt.SanitizeFileName(name);
            return Path.Combine(profilesDir, $"{sanitizedName}.{extension}");
        }

        public IEnumerable<string> IterProfileNames()
        {
            foreach (var path in Directory.EnumerateFiles(profilesDir))
            {
                if (!path.EndsWith(".xml"))
                    continue;

                var start = profilesDir.Length + 1;
                var end = path.Length - 4;
                yield return path.Substring(start, end - start);
            }
        }

        public bool HasProfile(string name)
        {
            return File.Exists(FormatPath(name, "xml"));
        }

        public void Save(string name)
        {
            var toolbar = new Toolbar(currentToolbar);

            var jsonPath = FormatPath(name, "json");
            toolbar.WriteJson(jsonPath);

            toolbar.Dissociate(currentToolbar);

            var xmlPath = FormatPath(name, "xml");
            toolbar.Write(xmlPath);
        }

        public void Load(string name, bool merge)
        {
            var xmlPath = FormatPath(name, "xml");

            var toolbar = Toolbar.Read(xmlPath);

            toolbar.Associate(currentToolbar);

            if (merge)
                toolbar.Merge(currentToolbar);
            else
                toolbar.Set(currentToolbar);
        }

        public JsonData ReadJson(string name)
        {
            var jsonPath = FormatPath(name, "json");
            if (!File.Exists(jsonPath))
                return null;

            try
            {
                var jsonText = File.ReadAllText(jsonPath);
                return JsonMapper.ToObject(jsonText);
            }
            catch (JsonException e)
            {
                MyLog.Default.Warning($"ToolbarManager: Failed to load JSON toolbar file: \"{jsonPath}\" ({e})");
                return null;
            }
        }

        public void Rename(string oldName, string newName)
        {
            var oldXmlPath = FormatPath(oldName, "xml");
            var newXmlPath = FormatPath(newName, "xml");

            var oldJsonPath = FormatPath(oldName, "json");
            var newJsonPath = FormatPath(newName, "json");

            if (File.Exists(newXmlPath))
                File.Delete(newXmlPath);

            if (File.Exists(newJsonPath))
                File.Delete(newJsonPath);

            File.Move(oldXmlPath, newXmlPath);
            File.Move(oldJsonPath, newJsonPath);
        }

        public void Delete(string name)
        {
            var xmlPath = FormatPath(name, "xml");
            var jsonPath = FormatPath(name, "json");

            if (File.Exists(xmlPath))
                File.Delete(xmlPath);

            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }
    }
}