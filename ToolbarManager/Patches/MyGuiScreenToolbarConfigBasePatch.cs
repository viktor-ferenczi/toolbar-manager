using System;
using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Graphics.GUI;
using VRage.Game;
using VRage.Utils;
using VRageMath;

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
        private static void RecreateControlsPostfix(MyGuiScreenToolbarConfigBase __instance, bool contructor)  // sic!
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
                VisualStyle = MyGuiControlButtonStyleEnum.Tiny,
                Position = new Vector2(-0.425f, 0.425f)
            };
            button.ButtonClicked += _ => OnSaveToolbar?.Invoke();
            __instance.Elements.Add(button);
        }

        private static void createLoadButton(MyGuiScreenToolbarConfigBase __instance)
        {
            var button = new MyGuiControlButton
            {
                Text = "Load",
                Name = "LoadToolbarButton",
                VisualStyle = MyGuiControlButtonStyleEnum.Tiny,
                Position = new Vector2(-0.375f, 0.425f)
            };
            button.ButtonClicked += _ => OnLoadToolbar?.Invoke();
            __instance.Elements.Add(button);
        }
    }
}