using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.Gui;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using ToolbarManager.Gui;
using VRage.Input;

namespace ToolbarManager.Patches
{
    [HarmonyPatch(typeof(MyGuiScreenGamePlay))]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    // ReSharper disable once UnusedType.Global
    public static class MyGuiScreenGamePlayPatch
    {
        private static readonly List<MyKeys> PressedKeys = new List<MyKeys>();

        private static readonly StringBuilder MatchingKeys = new StringBuilder();

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MyGuiScreenGamePlay.HandleUnhandledInput))]
        public static bool HandleUnhandledInputPrefix()
        {
            if (MyGuiScreenGamePlay.ActiveGameplayScreen != null || !(MySession.Static.ControlledEntity is MyCharacter))
                return true;

#if DISABLED
            // Disabled due to Issue #12: Conflicting keys Ctrl+Shift+C, Ctrl+Shift+X and Ctrl+Shift+B
            if (myInput.IsAnyShiftKeyPressed() && myInput.IsAnyCtrlKeyPressed())
            {
                GetPressedKeys();
                if (MatchingKeys.Length != 0)
                {
                    OpenCubeBuilder(MatchingKeys.ToString());
                    return false;
                }
            }
#endif

            var myInput = MyInput.Static;
            if (myInput.IsNewKeyPressed(MyKeys.OemPipe))
            {
                OpenCubeBuilder("");
                return false;
            }

            return true;
        }

        private static void OpenCubeBuilder(string searchText)
        {
            if (!(MyGuiSandbox.CreateScreen(typeof(CustomToolbarConfigScreen), 0, null, null) is CustomToolbarConfigScreen dialog))
                return;

            MyGuiScreenGamePlay.ActiveGameplayScreen = dialog;
            MyGuiSandbox.AddScreen(dialog);

            dialog.SetSearchText(searchText);
        }

#if DISABLED
        private static void GetPressedKeys()
        {
            MatchingKeys.Clear();

            var myInput = MyInput.Static;
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
#endif
    }
}