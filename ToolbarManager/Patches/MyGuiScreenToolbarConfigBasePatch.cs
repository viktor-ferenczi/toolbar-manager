using System;
using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Graphics.GUI;
using VRage.Game;
using VRageMath;

// ReSharper disable UnusedType.Global

// ReSharper disable InconsistentNaming

namespace ToolbarManager.Patches
{
    [HarmonyPatch(typeof(MyGuiScreenToolbarConfigBase))]
    public static class MyGuiScreenToolbarConfigBasePatch
    {
        public static event Action OnSaveCharacterToolbar;
        public static event Action OnLoadCharacterToolbar;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MyGuiScreenToolbarConfigBase.RecreateControls))]
        private static void RecreateControlsPostfix(MyGuiScreenToolbarConfigBase __instance, bool contructor) // sic!
        {
            createSaveButton(__instance);
            createLoadButton(__instance);
        }

        private static void createSaveButton(MyGuiScreenToolbarConfigBase __instance)
        {
            var button = new MyGuiControlButton
            {
                Text = "Save",
                Name = "SaveToolbarButton",
                VisualStyle = MyGuiControlButtonStyleEnum.Small,
                Position = new Vector2(-0.425f, 0.41f)
            };
            button.ButtonClicked += _ => OnSaveCharacterToolbar?.Invoke();
            __instance.Elements.Add(button);
        }

        private static void createLoadButton(MyGuiScreenToolbarConfigBase __instance)
        {
            var button = new MyGuiControlButton
            {
                Text = "Load",
                Name = "LoadToolbarButton",
                VisualStyle = MyGuiControlButtonStyleEnum.Small,
                Position = new Vector2(-0.425f, 0.46f)
            };
            button.ButtonClicked += _ => OnLoadCharacterToolbar?.Invoke();
            __instance.Elements.Add(button);
        }
    }
}