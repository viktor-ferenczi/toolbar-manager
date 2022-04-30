using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Game.Screens.Helpers.RadialMenuActions;
using VRage.Input;

namespace ToolbarManager.Patches
{
    [HarmonyPatch(typeof(MyGuiScreenGamePlay))]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class MyGuiScreenGamePlayPatch
    {
        private static readonly List<MyKeys> PressedKeys = new List<MyKeys>();
        private static readonly StringBuilder MatchingKeys = new StringBuilder();
        // private static readonly ListDialog ListDialog = new ListDialog();
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MyGuiScreenGamePlay.HandleUnhandledInput))]
        public static void HandleUnhandledInputPrefix()
        {
            if (MyGuiScreenGamePlay.ActiveGameplayScreen == null)
                OpenCubeBuilderOnKeypress();
        }

        private static void OpenCubeBuilderOnKeypress()
        {
            GetPressedKeys();
            if (MatchingKeys.Length == 1)
            {
                new MyActionShowProgressionTree().ExecuteAction();
                if (MyGuiScreenGamePlay.ActiveGameplayScreen is MyGuiScreenCubeBuilder screen)
                {
                    var toolbarConfigBase = screen as MyGuiScreenToolbarConfigBase;
                    var field = AccessTools.DeclaredField(typeof(MyGuiScreenToolbarConfigBase), "m_searchBox");
                    if (field.GetValue(toolbarConfigBase) is MyGuiControlSearchBox searchBox)
                    {
                        searchBox.TextBox.Text = MatchingKeys.ToString();
                    }
                }
            }
        }

        private static void GetPressedKeys()
        {
            MatchingKeys.Clear();
            
            var myInput = MyInput.Static;
            if (!myInput.IsAnyShiftKeyPressed())
                return;
            
            myInput.GetPressedKeys(PressedKeys);
            foreach (var key in PressedKeys)
            {
                var name = myInput.GetKeyName(key);
                if (name.Length != 1)
                    continue;
                    
                var c = name[0];
                if ((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                    MatchingKeys.Append(c);
            }
        }
    }
}