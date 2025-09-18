using System.Reflection;
using HarmonyLib;
using Sandbox.Graphics.GUI;
using ToolbarManager.Settings;
using ToolbarManager.Settings.Layouts;
using VRage.Plugins;

namespace ToolbarManager
{
    // ReSharper disable NotAccessedField.Local
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Plugin : IPlugin
    {
        public const string Name = "ToolbarManager";
        public static Plugin Instance { get; private set; }
        private SettingsGenerator settingsGenerator;
    
        public void Init(object gameInstance)
        {
            Instance = this;
            Instance.settingsGenerator = new SettingsGenerator();
            
            var harmony = new Harmony("ToolbarManager");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void Update()
        {
        }

        public void Dispose()
        {
            Instance = null;
        }

        // ReSharper disable once UnusedMember.Global
        public void OpenConfigDialog()
        {
            Instance.settingsGenerator.SetLayout<Simple>();
            MyGuiSandbox.AddScreen(Instance.settingsGenerator.Dialog);
        }
    }
}