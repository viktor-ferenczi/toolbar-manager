using System.Reflection;
using HarmonyLib;
using VRage.Plugins;

namespace ToolbarManager
{
    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin
    {
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
        }
    }
}