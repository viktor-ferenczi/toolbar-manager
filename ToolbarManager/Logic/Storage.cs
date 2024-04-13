using System;
using System.IO;
using System.Text;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Graphics.GUI;
using ToolbarManager.Extensions;
using ToolbarManager.Gui;
using VRage.FileSystem;
using VRage.Utils;

namespace ToolbarManager.Logic
{
    public static class Storage
    {
        public static string UserDataDir => Path.Combine(MyFileSystem.UserDataPath, "ToolbarManager");
        private static string lastSavedName;

        private static string FormatPath(MyToolbar currentToolbar, string name)
        {
            var dir = FormatDir(currentToolbar);
            var sanitizedName = PathExt.SanitizeFileName(name);
            return Path.Combine(dir, $"{sanitizedName}.xml");
        }

        private static string FormatDir(MyToolbar currentToolbar)
        {
            var dir = Path.Combine(UserDataDir, currentToolbar.ToolbarType.ToString());
            Directory.CreateDirectory(dir);
            return dir;
        }

        public static string Save()
        {
            var currentToolbar = MyToolbarComponent.CurrentToolbar;
            if (currentToolbar == null)
                return null;

            lastSavedName = null;
            MyGuiSandbox.AddScreen(new NameDialog(Save, "Save toolbar", ""));
            return lastSavedName;
        }

        public static void Save(string name)
        {
            if (name == null || name.Trim().Length == 0)
                return;

            var currentToolbar = MyToolbarComponent.CurrentToolbar;
            if (currentToolbar == null)
                return;

            lastSavedName = name;

            var path = FormatPath(currentToolbar, name);
            if (File.Exists(path))
            {
                MyGuiSandbox.AddScreen(
                    MyGuiSandbox.CreateMessageBox(buttonType: MyMessageBoxButtonsType.YES_NO,
                        messageText: new StringBuilder($"Are you sure to overwrite this saved toolbar?\r\n\r\n{name}"),
                        messageCaption: new StringBuilder("Confirmation"),
                        callback: result => OnSaveOverwriteForSure(result, path)));
            }
            else
            {
                OnSaveOverwriteForSure(MyGuiScreenMessageBox.ResultEnum.YES, path);
            }
        }

        private static void OnSaveOverwriteForSure(MyGuiScreenMessageBox.ResultEnum result, string path)
        {
            if (result != MyGuiScreenMessageBox.ResultEnum.YES)
                return;

            var currentToolbar = MyToolbarComponent.CurrentToolbar;
            if (currentToolbar == null)
                return;

            var pathWithoutExtension = path.Substring(0, path.Length - 4);

            var toolbar = new Toolbar(currentToolbar);

            try
            {
                toolbar.WriteJson($"{pathWithoutExtension}.json");
                toolbar.Dissociate(currentToolbar);
                toolbar.Write(path);
            }
            catch (Exception e)
            {
                MyLog.Default.Error($"ToolbarManager: Failed to save character toolbar \"{{name}}\" to file \"{{path}}\": {e}");
                MyGuiSandboxExt.Show("Failed to save character toolbar", "Error");
            }
        }

        public static bool Load(string name, bool merge)
        {
            if (name == null)
                return false;

            var currentToolbar = MyToolbarComponent.CurrentToolbar;
            if (currentToolbar == null)
                return false;

            var path = FormatPath(currentToolbar, name);

            var toolbar = Toolbar.Read(path);

            toolbar.Associate(currentToolbar);

            if (merge)
                toolbar.Merge(currentToolbar);
            else
                toolbar.Set(currentToolbar);

            return true;
        }
    }
}