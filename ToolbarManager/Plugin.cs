using System.Reflection;
using HarmonyLib;
using VRage.Plugins;

namespace ToolbarManager
{
    // ReSharper disable NotAccessedField.Local
    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin
    {
        public void Init(object gameInstance)
        {
            var harmony = new Harmony("ToolbarManager");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void Update()
        {
        }

        public void Dispose()
        {
        }
    }
}