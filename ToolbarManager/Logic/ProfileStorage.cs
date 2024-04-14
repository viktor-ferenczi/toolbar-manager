using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Sandbox.Game.Screens.Helpers;
using ToolbarManager.Extensions;
using VRage.FileSystem;
using VRage.Utils;

namespace ToolbarManager.Logic
{
    public class ProfileStorage
    {
        private const int NumberOfBackups = 5;
        private const int DeleteOldBackupsOfDeletedProfilesAfterDays = 90;

        private static string PluginDataDir => Path.Combine(MyFileSystem.UserDataPath, "ToolbarManager");

        private readonly MyToolbar currentToolbar;
        private readonly string profileDir;
        private readonly string backupDir;

        public ProfileStorage(MyToolbar currentToolbar)
        {
            this.currentToolbar = currentToolbar;

            profileDir = Path.Combine(PluginDataDir, currentToolbar.ToolbarType.ToString());
            Directory.CreateDirectory(profileDir);

            backupDir = Path.Combine(profileDir, "Backup");
            Directory.CreateDirectory(backupDir);
        }

        private string FormatProfilePath(string name, string extension)
        {
            var sanitizedName = name.SanitizeFileName();
            return Path.Combine(profileDir, $"{sanitizedName}.{extension}");
        }

        private string FormatBackupPath(string name, string extension, int index)
        {
            var sanitizedName = name.SanitizeFileName();
            return Path.Combine(backupDir, $"{sanitizedName}.{extension}.{index}");
        }

        public IEnumerable<string> IterProfileNames()
        {
            foreach (var path in Directory.EnumerateFiles(profileDir))
            {
                if (!path.EndsWith(".xml"))
                    continue;

                var start = profileDir.Length + 1;
                var end = path.Length - 4;
                yield return path.Substring(start, end - start);
            }
        }

        public bool HasProfile(string name)
        {
            return File.Exists(FormatProfilePath(name, "xml"));
        }

        public void Save(string name)
        {
            var toolbar = new Toolbar(currentToolbar);

            var jsonPath = FormatProfilePath(name, "json");
            toolbar.WriteJson(jsonPath);

            toolbar.Dissociate(currentToolbar);

            var xmlPath = FormatProfilePath(name, "xml");
            toolbar.Write(xmlPath);

            Backup(name, "xml");
            Backup(name, "json");
        }

        public void Load(string name, bool merge)
        {
            var xmlPath = FormatProfilePath(name, "xml");

            var toolbar = Toolbar.Read(xmlPath);

            toolbar.Associate(currentToolbar);

            if (merge)
                toolbar.Merge(currentToolbar);
            else
                toolbar.Set(currentToolbar);
        }

        public JsonData ReadJson(string name)
        {
            var jsonPath = FormatProfilePath(name, "json");
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
            var oldXmlPath = FormatProfilePath(oldName, "xml");
            var newXmlPath = FormatProfilePath(newName, "xml");

            var oldJsonPath = FormatProfilePath(oldName, "json");
            var newJsonPath = FormatProfilePath(newName, "json");

            if (File.Exists(newXmlPath))
                File.Delete(newXmlPath);

            if (File.Exists(newJsonPath))
                File.Delete(newJsonPath);

            File.Move(oldXmlPath, newXmlPath);
            File.Move(oldJsonPath, newJsonPath);

            RenameBackups(oldName, newName);
        }

        public void Delete(string name)
        {
            var xmlPath = FormatProfilePath(name, "xml");
            var jsonPath = FormatProfilePath(name, "json");

            if (File.Exists(xmlPath))
                File.Delete(xmlPath);

            if (File.Exists(jsonPath))
                File.Delete(jsonPath);

            DeleteOldBackupsOfDeletedProfiles();
        }

        private void Backup(string name, string extension)
        {
            RotateBackups(name, extension);

            var profilePath = FormatProfilePath(name, extension);
            var backupPath = FormatBackupPath(name, extension, 0);
            File.Copy(profilePath, backupPath);
        }

        private void RotateBackups(string name, string extension)
        {
            for (var i = NumberOfBackups; i > 0; i--)
            {
                var olderPath = FormatBackupPath(name, extension, i);
                var newerPath = FormatBackupPath(name, extension, i - 1);

                if (File.Exists(olderPath))
                    File.Delete(olderPath);

                if (File.Exists(newerPath))
                    File.Move(newerPath, olderPath);
            }
        }

        private void RenameBackups(string oldName, string newName)
        {
            for (var i = 0; i <= NumberOfBackups; i++)
            {
                RenameBackup(oldName, newName, "xml", i);
                RenameBackup(oldName, newName, "json", i);
            }
        }

        private void RenameBackup(string oldName, string newName, string extension, int index)
        {
            var oldBackup = FormatBackupPath(oldName, extension, index);
            var newBackup = FormatBackupPath(newName, extension, index);

            if (File.Exists(newBackup))
            {
                var overwritten = newBackup;
                for (var j = 0; j < 100; j++)
                {
                    overwritten = $"{overwritten}.{index}";
                    if (!File.Exists(overwritten))
                    {
                        File.Move(newBackup, overwritten);
                        overwritten = null;
                        break;
                    }
                }
                if (overwritten != null)
                    File.Delete(newBackup);
            }

            if (File.Exists(oldBackup))
                File.Move(oldBackup, newBackup);
        }

        private void DeleteOldBackupsOfDeletedProfiles()
        {
            var now = DateTime.Now;
            foreach (var backupPath in Directory.EnumerateFiles(backupDir))
            {
                var backupFilename = Path.GetFileName(backupPath);
                var len = backupFilename.Length;
                if (len < 2 || backupFilename[len - 2] != '.' || !backupFilename[len - 1].IsDigit())
                    continue;

                var profileFilename = backupFilename.Substring(0, len - 2);
                var profilePath = Path.Combine(profileDir, profileFilename);
                if (File.Exists(profilePath))
                    continue;

                var dt = File.GetLastWriteTime(backupPath);
                if ((now - dt).Days < DeleteOldBackupsOfDeletedProfilesAfterDays)
                    continue;

                File.Delete(backupPath);
            }
        }
    }
}