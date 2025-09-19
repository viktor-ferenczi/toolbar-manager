using HarmonyLib;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Graphics.GUI;

namespace ToolbarManager.Patches
{
    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedType.Global
    // ReSharper disable InconsistentNaming
    [HarmonyPatch(typeof(MyGuiControlToolbar))]
    public static class MyGuiControlToolbarPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(MyGuiControlToolbar.HandleDragAndDrop))]
        private static bool HandleDragAndDropPrefix(MyGuiControlToolbar __instance, MyDragAndDropEventArgs eventArgs)
        {
            if (eventArgs.DropTo == null || !__instance.IsToolbarGrid(eventArgs.DropTo.Grid))
            {
                // Prevent clearing the toolbar slot on cancelling drag&drop by releasing the mouse outside the toolbar
                return false;
            }

            return true;
        }
    }
}