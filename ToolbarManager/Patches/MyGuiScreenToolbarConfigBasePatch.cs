using System;
using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Graphics.GUI;
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
        public static event Action OnProfilesClicked;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MyGuiScreenToolbarConfigBase.RecreateControls))]
        private static void RecreateControlsPostfix(MyGuiScreenToolbarConfigBase __instance)
        {
            var toolbarType = MyToolbarComponent.CurrentToolbar?.ToolbarType;

            var button = new MyGuiControlButton
            {
                Text = "Profiles",
                Name = "ToolbarProfilesButton",
                VisualStyle = MyGuiControlButtonStyleEnum.Small,
                Position = new Vector2(-0.45f, 0.435f)
            };

            button.ButtonClicked += _ => OnProfilesClicked?.Invoke();
            button.Enabled = toolbarType != null && toolbarType != MyToolbarType.None;

            __instance.Elements.Add(button);
        }
    }
}