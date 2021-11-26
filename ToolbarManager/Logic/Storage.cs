using System;
using System.IO;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.World;
using ToolbarManager.Patches;
using VRage.FileSystem;
using VRage.Utils;

namespace ToolbarManager.Logic
{
    public class Storage
    {
        // MyFileSystem.UserDataPath ~= C:\Users\%USERNAME%\AppData\Roaming\SpaceEngineers
        private static string UserDataDir => Path.Combine(MyFileSystem.UserDataPath, "ToolbarManager");
        private static string CharacterDir => Path.Combine(UserDataDir, "Character");
        private static string ControllerDir => Path.Combine(UserDataDir, "Controller");
        private static string TimerDir => Path.Combine(UserDataDir, "Timer");

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

            Directory.CreateDirectory(CharacterDir);

            var path = Path.Combine(CharacterDir, "toolbar.xml");
            var toolbar = new Toolbar(Character.Toolbar);
            toolbar.Write(path);
        }

        private void OnLoadCharacterToolbar()
        {
            if (!Valid)
                return;

            var path = Path.Combine(CharacterDir, "toolbar.xml");
            var toolbar = Toolbar.Read(path);
            toolbar.Set(Character.Toolbar);
        }
    }
}