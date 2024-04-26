using System.Diagnostics.CodeAnalysis;
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
        [HarmonyPrefix]
        [HarmonyPatch(nameof(MyGuiScreenGamePlay.HandleUnhandledInput))]
        public static bool HandleUnhandledInputPrefix()
        {
            if (MyGuiScreenGamePlay.ActiveGameplayScreen != null || !(MySession.Static.ControlledEntity is MyCharacter))
                return true;

            var myInput = MyInput.Static;
            if (myInput.IsNewKeyPressed(Config.Data.BlockSearchKey))
            {
                OpenCubeBuilder();
                return false;
            }

            return true;
        }

        private static void OpenCubeBuilder()
        {
            if (!(MyGuiSandbox.CreateScreen(typeof(CustomToolbarConfigScreen), 0, null, null) is CustomToolbarConfigScreen dialog))
                return;

            MyGuiScreenGamePlay.ActiveGameplayScreen = dialog;
            MyGuiSandbox.AddScreen(dialog);
        }
    }
}