using System.Reflection;
using HarmonyLib;
using Sandbox.Graphics.GUI;
using ToolbarManager.Gui;
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

            Config.Load();
        }

        public void Update()
        {
        }

        public void Dispose()
        {
        }

        // ReSharper disable once UnusedMember.Global
        public void OpenConfigDialog()
        {
            var screen = new PluginConfigDialog();
            MyGuiSandbox.AddScreen(screen);
            screen.Closed += (s, isUnloading) => Config.Save();
        }
    }
}