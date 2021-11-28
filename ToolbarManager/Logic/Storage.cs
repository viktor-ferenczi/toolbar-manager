using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Graphics.GUI;
using ToolbarManager.Extensions;
using ToolbarManager.Gui;
using ToolbarManager.Patches;
using VRage.FileSystem;
using VRage.Utils;

namespace ToolbarManager.Logic
{
    public class Storage
    {
        // MyFileSystem.UserDataPath ~= C:\Users\%USERNAME%\AppData\Roaming\SpaceEngineers
        private static string UserDataDir => Path.Combine(MyFileSystem.UserDataPath, "ToolbarManager");

        private readonly Dictionary<string, string> latestNames = new Dictionary<string, string>();

        public Storage()
        {
            MyGuiScreenToolbarConfigBasePatch.OnLoadToolbar += OnLoadToolbar;
            MyGuiScreenToolbarConfigBasePatch.OnSaveToolbar += OnSaveToolbar;

            MyLog.Default.Info($"ToolbarManager: UserDataDir = \"{UserDataDir}\"");
        }

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

        private void OnSaveToolbar()
        {
            var currentToolbar = MyToolbarComponent.CurrentToolbar;
            if (currentToolbar == null)
                return;

            var latestName = latestNames.GetValueOrDefault(currentToolbar.ToolbarType.ToString()) ?? "";

            MyGuiSandbox.AddScreen(new NameDialog(OnNameSpecified, "Save character toolbar", latestName));
        }

        private void OnNameSpecified(string name)
        {
            var currentToolbar = MyToolbarComponent.CurrentToolbar;
            if (currentToolbar == null)
                return;

            var path = FormatPath(currentToolbar, name);

            if (File.Exists(path))
            {
                MyGuiSandbox.AddScreen(
                    MyGuiSandbox.CreateMessageBox(buttonType: MyMessageBoxButtonsType.YES_NO,
                        messageText: new StringBuilder($"Are you sure to overwrite this saved toolbar?\r\n\r\n{name}"),
                        messageCaption: new StringBuilder("Confirmation"),
                        callback: result => OnOverwriteForSure(result, path)));
            }
            else
            {
                OnOverwriteForSure(MyGuiScreenMessageBox.ResultEnum.YES, path);
            }
        }

        private void OnOverwriteForSure(MyGuiScreenMessageBox.ResultEnum result, string path)
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
                toolbar.WriteJson( $"{pathWithoutExtension}.json");
                toolbar.Dissociate(currentToolbar);
                toolbar.Write(path);
            }
            catch (Exception e)
            {
                MyLog.Default.Error($"ToolbarManager: Failed to save character toolbar \"{{name}}\" to file \"{{path}}\": {e}");
                MyGuiSandboxExt.Show("Failed to save character toolbar", "Error");
            }
        }

        private void OnLoadToolbar()
        {
            var currentToolbar = MyToolbarComponent.CurrentToolbar;
            if (currentToolbar == null)
                return;

            var latestName = latestNames.GetValueOrDefault(currentToolbar.ToolbarType.ToString()) ?? "";

            MyGuiSandbox.AddScreen(new ListDialog(OnItemSelected, "Load character toolbar", latestName, FormatDir(currentToolbar)));
        }

        private void OnItemSelected(string name, bool merge)
        {
            var currentToolbar = MyToolbarComponent.CurrentToolbar;
            if (currentToolbar == null)
                return;

            latestNames[currentToolbar.ToolbarType.ToString()] = name;

            var path = FormatPath(currentToolbar, name);

            var toolbar = Toolbar.Read(path);

            toolbar.Associate(currentToolbar);

            if (merge)
                toolbar.Merge(currentToolbar);
            else
                toolbar.Set(currentToolbar);
        }
    }
}