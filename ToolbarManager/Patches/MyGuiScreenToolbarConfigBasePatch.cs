using System;
using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using VRage.Game;
using VRageMath;

// ReSharper disable UnusedMember.Local

// ReSharper disable UnusedType.Global

// ReSharper disable InconsistentNaming

namespace ToolbarManager.Patches
{
    [HarmonyPatch(typeof(MyGuiScreenToolbarConfigBase))]
    public static class MyGuiScreenToolbarConfigBasePatch
    {
        public static event Action OnSaveToolbar;
        public static event Action OnLoadToolbar;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MyGuiScreenToolbarConfigBase.RecreateControls))]
        private static void RecreateControlsPostfix(MyGuiScreenToolbarConfigBase __instance)
        {
            createSaveButton(__instance);
            createLoadButton(__instance);
        }

        private static void createSaveButton(MyGuiScreenToolbarConfigBase screen)
        {
            var button = new MyGuiControlButton
            {
                Text = "Save",
                Name = "SaveToolbarButton",
                VisualStyle = MyGuiControlButtonStyleEnum.Small,
                Position = new Vector2(-0.425f, 0.41f)
            };
            button.ButtonClicked += _ => OnSaveToolbar?.Invoke();
            screen.Elements.Add(button);
        }

        private static void createLoadButton(MyGuiScreenToolbarConfigBase screen)
        {
            var button = new MyGuiControlButton
            {
                Text = "Load",
                Name = "LoadToolbarButton",
                VisualStyle = MyGuiControlButtonStyleEnum.Small,
                Position = new Vector2(-0.425f, 0.46f)
            };
            button.ButtonClicked += _ => OnLoadToolbar?.Invoke();
            screen.Elements.Add(button);
        }
    }
}