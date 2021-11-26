using System;
using System.IO;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.World;
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
        private const string DefaultName = "Latest";

        // MyFileSystem.UserDataPath ~= C:\Users\%USERNAME%\AppData\Roaming\SpaceEngineers
        private static string UserDataDir => Path.Combine(MyFileSystem.UserDataPath, "ToolbarManager");
        private static string CharacterDir => Path.Combine(UserDataDir, "Character");
        private static string BlockDir => Path.Combine(UserDataDir, "Block");

        private static MyCharacter Character => MySession.Static.LocalCharacter;
        private static bool Valid => Character?.Toolbar != null;

        public Storage()
        {
            MyGuiScreenToolbarConfigBasePatch.OnLoadCharacterToolbar += OnLoadCharacterToolbar;
            MyGuiScreenToolbarConfigBasePatch.OnSaveCharacterToolbar += OnSaveCharacterToolbar;

            MyLog.Default.Info($"ToolbarManager: UserDataDir = \"{UserDataDir}\"");
        }

        private void OnSaveCharacterToolbar()
        {
            if (!Valid)
                return;

            MyGuiSandbox.AddScreen(new NameDialog(OnNameSpecified, "Save character toolbar", DefaultName));
        }

        private void OnNameSpecified(string name)
        {
            var toolbar = new Toolbar(Character.Toolbar);

            Directory.CreateDirectory(CharacterDir);

            name = PathExt.SanitizeFileName(name);
            var path = Path.Combine(CharacterDir, $"{name}.xml");

            try
            {
                toolbar.Write(path);
            }
            catch (Exception e)
            {
                MyLog.Default.Error($"ToolbarManager: Failed to save character toolbar \"{{name}}\" to file \"{{path}}\": {e}");
                MyGuiSandboxExt.Show("Failed to save character toolbar", "Error");
            }
        }

        private void OnLoadCharacterToolbar()
        {
            MyGuiSandbox.AddScreen(new ListDialog(OnItemSelected, "Load character toolbar", DefaultName, CharacterDir));
        }

        private void OnItemSelected(string name, bool merge)
        {
            if (!Valid)
                return;

            name = PathExt.SanitizeFileName(name);
            var path = Path.Combine(CharacterDir, $"{name}.xml");
            var toolbar = Toolbar.Read(path);

            if (merge)
                toolbar.Merge(Character.Toolbar);
            else
                toolbar.Set(Character.Toolbar);
        }
    }
}