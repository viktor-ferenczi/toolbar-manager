using System;
using System.Reflection;
using System.Text;
using Sandbox;
using Sandbox.Game.Gui;
using Sandbox.Graphics.GUI;
using VRage;
using VRage.Game;
using VRage.Input;
using VRage.Utils;
using VRageMath;

namespace ToolbarManager.Gui
{
    public class PluginConfigDialog : MyGuiScreenBase
    {
        private const string Caption = "Toolbar Manager Configuration";
        public override string GetFriendlyName() => "ToolbarManagerConfigDialog";

        private MyLayoutTable layoutTable;

        private MyGuiControlLabel keepBlockSearchTextLabel;
        private MyGuiControlCheckbox keepBlockSearchTextCheckbox;

        private MyGuiControlLabel keepProfileSearchTextLabel;
        private MyGuiControlCheckbox keepProfileSearchTextCheckbox;

        private MyControl blockSearchKeyControl;
        private MyGuiControlLabel blockSearchKeyLabel;
        private MyGuiControlButton blockSearchKeyButton;

        private MyGuiControlButton closeButton;

        public PluginConfigDialog() : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.5f, 0.5f), false, null, MySandboxGame.Config.UIBkOpacity, MySandboxGame.Config.UIOpacity)
        {
            EnabledBackgroundFade = true;
            m_closeOnEsc = true;
            m_drawEvenWithoutFocus = true;
            CanHideOthers = true;
            CanBeHidden = true;
            CloseButtonEnabled = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            RecreateControls(true);
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);

            CreateControls();
            LayoutControls();
        }

        private void CreateControls()
        {
            AddCaption(Caption);

            CreateCheckbox(
                out keepProfileSearchTextLabel,
                out keepProfileSearchTextCheckbox,
                Config.Data.KeepProfileSearchText,
                value => { Config.Data.KeepProfileSearchText = value; },
                "Keep profile search text",
                "Keep the search text between subsequent uses of the Profile dialog.");

            CreateCheckbox(
                out keepBlockSearchTextLabel,
                out keepBlockSearchTextCheckbox,
                Config.Data.KeepBlockSearchText,
                value => { Config.Data.KeepBlockSearchText = value; },
                "Keep block search text",
                "Keep the search text between subsequent uses of the block search.");

            blockSearchKeyControl = new MyControl(
                MyStringId.GetOrCompute("ToolbarManagerBlockSearch"),
                MyStringId.GetOrCompute("Key to start block search"),
                MyGuiControlTypeEnum.General,
                null,
                Config.Data.BlockSearchKey,
                MyStringId.GetOrCompute("Key to open the G menu and activate block search"),
                null,
                MyStringId.GetOrCompute("Key to open the G menu and activate block search"));

            CreateControlButton(out blockSearchKeyLabel, out blockSearchKeyButton, blockSearchKeyControl, "Key to start block search");

            closeButton = new MyGuiControlButton(originAlign: MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER, text: MyTexts.Get(MyCommonTexts.Ok), onButtonClick: OnOk);
        }

        private class ControlButtonData
        {
            public readonly MyControl Control;
            public readonly MyGuiInputDeviceEnum Device;

            public ControlButtonData(MyControl control, MyGuiInputDeviceEnum device)
            {
                Control = control;
                Device = device;
            }
        }

        private void CreateControlButton(out MyGuiControlLabel label, out MyGuiControlButton button, MyControl control, string labelText, bool enabled = true)
        {
            label = new MyGuiControlLabel
            {
                Text = labelText,
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP,
                Enabled = enabled,
            };

            StringBuilder output = null;
            control.AppendBoundButtonNames(ref output, MyGuiInputDeviceEnum.Keyboard);
            MyControl.AppendUnknownTextIfNeeded(ref output, MyTexts.GetString(MyCommonTexts.UnknownControl_None));
            button = new MyGuiControlButton(text: output, onButtonClick: OnControlClick, onSecondaryButtonClick: OnSecondaryControlClick)
            {
                VisualStyle = MyGuiControlButtonStyleEnum.ControlSetting,
                UserData = new ControlButtonData(control, MyGuiInputDeviceEnum.Keyboard),
            };
        }

        private void OnControlClick(MyGuiControlButton button)
        {
            var userData = (ControlButtonData) button.UserData;
            var messageText = MyCommonTexts.AssignControlKeyboard;
            if (userData.Device == MyGuiInputDeviceEnum.Mouse)
                messageText = MyCommonTexts.AssignControlMouse;

            // KEEN!!! MyGuiScreenOptionsControls.MyGuiControlAssignKeyMessageBox is PRIVATE!
            var screenClass = typeof(MyGuiScreenOptionsControls).GetNestedType("MyGuiControlAssignKeyMessageBox", BindingFlags.NonPublic);
            var editBindingDialog = (MyGuiScreenBase) Activator.CreateInstance(screenClass, BindingFlags.CreateInstance, null, new object[] { userData.Device, userData.Control, messageText }, null);
            editBindingDialog.Closed += (s, isUnloading) => StoreControlSelection(button);
            MyGuiSandbox.AddScreen(editBindingDialog);
        }

        private static void StoreControlSelection(MyGuiControlButton button)
        {
            StringBuilder output = null;
            var userData = (ControlButtonData) button.UserData;
            userData.Control.AppendBoundButtonNames(ref output, userData.Device);

            Config.Data.BlockSearchKey = userData.Control.GetKeyboardControl();

            MyControl.AppendUnknownTextIfNeeded(ref output, MyTexts.GetString(MyCommonTexts.UnknownControl_None));
            button.Text = output.ToString();
            output.Clear();
        }

        private void OnSecondaryControlClick(MyGuiControlButton obj)
        {
        }

        private void OnOk(MyGuiControlButton _) => CloseScreen();

        private void CreateCheckbox(out MyGuiControlLabel labelControl, out MyGuiControlCheckbox checkboxControl, bool value, Action<bool> store, string label, string tooltip, bool enabled = true)
        {
            labelControl = new MyGuiControlLabel
            {
                Text = label,
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP,
                Enabled = enabled,
            };

            checkboxControl = new MyGuiControlCheckbox(toolTip: tooltip)
            {
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP,
                IsChecked = value,
                Enabled = enabled,
                CanHaveFocus = enabled
            };
            if (enabled)
            {
                checkboxControl.IsCheckedChanged += cb => store(cb.IsChecked);
            }
            else
            {
                checkboxControl.IsCheckedChanged += cb => { cb.IsChecked = value; };
            }
        }

        private void LayoutControls()
        {
            layoutTable = new MyLayoutTable(this, new Vector2(-0.2f, -0.25f), new Vector2(0.20f, 0.25f));
            layoutTable.SetColumnWidths(500f, 500f);
            layoutTable.SetRowHeights(150f, 90f, 90f, 90f, 90f, 180f);

            var row = 1;

            layoutTable.Add(keepProfileSearchTextLabel, MyAlignH.Left, MyAlignV.Top, row, 0);
            layoutTable.Add(keepProfileSearchTextCheckbox, MyAlignH.Left, MyAlignV.Top, row, 1);
            row++;

            layoutTable.Add(keepBlockSearchTextLabel, MyAlignH.Left, MyAlignV.Top, row, 0);
            layoutTable.Add(keepBlockSearchTextCheckbox, MyAlignH.Left, MyAlignV.Top, row, 1);
            row++;

            layoutTable.Add(blockSearchKeyLabel, MyAlignH.Left, MyAlignV.Top, row, 0);
            layoutTable.Add(blockSearchKeyButton, MyAlignH.Left, MyAlignV.Top, row, 1);
            row++;

            layoutTable.Add(closeButton, MyAlignH.Center, MyAlignV.Center, row, 0, colSpan: 2);
        }
    }
}