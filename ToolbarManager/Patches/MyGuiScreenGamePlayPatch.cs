using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.Gui;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using ToolbarManager.Gui;
using VRage.Input;

namespace ToolbarManager.Patches
{
    [HarmonyPatch(typeof(MyGuiScreenGamePlay))]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class MyGuiScreenGamePlayPatch
    {
        private static readonly List<MyKeys> PressedKeys = new List<MyKeys>();
        private static readonly StringBuilder MatchingKeys = new StringBuilder();
        // private static readonly ListDialog ListDialog = new ListDialog();

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MyGuiScreenGamePlay.HandleUnhandledInput))]
        public static bool HandleUnhandledInputPrefix()
        {
            // TODO: Add configuration dialog with checkbox to disable this feature
            if (MyGuiScreenGamePlay.ActiveGameplayScreen == null)
            {
                GetPressedKeys();
                if (MatchingKeys.Length != 0)
                {
                    OpenCubeBuilder();
                    return false;
                }
            }
            return true;
        }

        private static void OpenCubeBuilder()
        {
            if (!(MyGuiSandbox.CreateScreen(typeof(CustomToolbarConfigScreen), 0, null, null) is CustomToolbarConfigScreen dialog))
                return;
            
            MyGuiScreenGamePlay.ActiveGameplayScreen = dialog;
            MyGuiSandbox.AddScreen(dialog);
            
            dialog.SetSearchText(MatchingKeys.ToString());
        }

        private static void GetPressedKeys()
        {
            MatchingKeys.Clear();
            
            var myInput = MyInput.Static;
            if (!(myInput.IsAnyCtrlKeyPressed() && myInput.IsAnyShiftKeyPressed()))
                return;
            
            myInput.GetPressedKeys(PressedKeys);
            foreach (var key in PressedKeys)
            {
                var name = myInput.GetKeyName(key);
                if (name.Length != 1)
                    continue;
                    
                var c = name[0];
                if (c >= 'A' && c <= 'Z')
                    MatchingKeys.Append(c);
            }
        }
    }
}