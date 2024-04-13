using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Graphics.GUI;
using ToolbarManager.Gui;
using VRage.Game;
using VRageMath;

namespace ToolbarManager.Patches
{
    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedType.Global
    // ReSharper disable InconsistentNaming
    [HarmonyPatch(typeof(MyGuiScreenToolbarConfigBase))]
    public static class MyGuiScreenToolbarConfigBasePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MyGuiScreenToolbarConfigBase.RecreateControls))]
        private static void RecreateControlsPostfix(MyGuiScreenToolbarConfigBase __instance)
        {
            var toolbarType = MyToolbarComponent.CurrentToolbar?.ToolbarType;

            var button = new MyGuiControlButton
            {
                Text = "TM",
                Name = "OpenToolbarManagerButton",
                VisualStyle = MyGuiControlButtonStyleEnum.Square,
                Position = new Vector2(-0.45f, 0.425f)
            };

            button.SetToolTip("Open Toolbar Manager");

            button.ButtonClicked += OpenProfilesScreen;
            button.Enabled = toolbarType != null && toolbarType != MyToolbarType.None;

            __instance.Elements.Add(button);
        }

        private static void OpenProfilesScreen(MyGuiControlButton obj)
        {
            var toolbarType = MyToolbarComponent.CurrentToolbar?.ToolbarType;
            MyGuiSandbox.AddScreen(new ProfilesDialog(toolbarType));
        }
    }
}