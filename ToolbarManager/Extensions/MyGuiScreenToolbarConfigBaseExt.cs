using System.Reflection;
using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.Gui;

namespace ToolbarManager.Extensions
{
    public static class MyGuiScreenToolbarConfigBaseExt
    {
        private static readonly FieldInfo ShipControllerFieldInfo = AccessTools.DeclaredField(typeof(MyGuiScreenToolbarConfigBase), "m_shipController");
        public static MyShipController GetShipController(this MyGuiScreenToolbarConfigBase obj) => ShipControllerFieldInfo.GetValue(obj) as MyShipController;

        private static readonly MethodInfo AddCubeDefinitionsToBlocksMethodInfo = AccessTools.DeclaredMethod(typeof(MyGuiScreenToolbarConfigBase), "AddCubeDefinitionsToBlocks");
        public static void AddCubeDefinitionsToBlocks(this MyGuiScreenToolbarConfigBase obj, IMySearchCondition searchCondition) => AddCubeDefinitionsToBlocksMethodInfo.Invoke(obj, new object[] { searchCondition });

        private static readonly MethodInfo AddShipBlocksDefinitionsMethodInfo = AccessTools.DeclaredMethod(typeof(MyGuiScreenToolbarConfigBase), "AddShipBlocksDefinitions");
        public static void AddShipBlocksDefinitions(this MyGuiScreenToolbarConfigBase obj, MyCubeGrid grid, bool isShip, IMySearchCondition searchCondition) => AddShipBlocksDefinitionsMethodInfo.Invoke(obj, new object[] { grid, isShip, searchCondition });
    }
}