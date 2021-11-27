using System.Reflection;
using HarmonyLib;
using ToolbarManager.Logic;
using VRage.Plugins;

namespace ToolbarManager
{
    // ReSharper disable NotAccessedField.Local
    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin
    {
        private static bool initialized;
        private static Storage storage;

        public void Dispose()
        {
        }

        public void Init(object gameInstance)
        {
            var harmony = new Harmony("ToolbarManager");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void Update()
        {
            if (initialized)
                return;

            storage = new Storage();

            initialized = true;
        }
    }
}