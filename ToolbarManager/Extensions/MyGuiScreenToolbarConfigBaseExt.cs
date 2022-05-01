using System.Reflection;
using HarmonyLib;
using Sandbox.Game.Gui;
using VRage.Game;

namespace ToolbarManager.Extensions
{
    public static class MyGuiScreenToolbarConfigBaseExt
    {
        private static readonly MethodInfo AddGridItemToToolbarMethodInfo = AccessTools.DeclaredMethod(typeof(MyGuiScreenToolbarConfigBase), "AddGridItemToToolbar");
        public static void AddGridItemToToolbar(this MyGuiScreenToolbarConfigBase obj, MyObjectBuilder_ToolbarItem toolbarItemBuilder) => AddGridItemToToolbarMethodInfo.Invoke(obj, new object[] { toolbarItemBuilder });
    }
}