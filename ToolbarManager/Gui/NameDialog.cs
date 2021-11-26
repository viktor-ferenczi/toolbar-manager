using System;
using Sandbox;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using VRage;
using VRage.Input;
using VRage.Utils;
using VRageMath;

namespace ToolbarManager.Gui
{
    class NameDialog : MyGuiBlueprintScreenBase
    {
        private MyGuiControlTextbox nameBox;
        private MyGuiControlButton okButton;
        private MyGuiControlButton cancelButton;
        private readonly string defaultName;
        private readonly string caption;
        private readonly int maxTextLength;
        private readonly Action<string> callBack;

        public NameDialog(
            Action<string> callBack,
            string caption = "",
            string defaultName = "",
            int maxLenght = 20,
            float textBoxWidth = 0.2f)
            : base(new Vector2(0.5f, 0.5f), new Vector2(0.4971429f, 0.2805344f), MyGuiConstants.SCREEN_BACKGROUND_COLOR * MySandboxGame.Config.UIBkOpacity, true)
        {
            maxTextLength = maxLenght;
            this.caption = caption;
            this.callBack = callBack;
            this.defaultName = defaultName;

            RecreateControls(true);

            CanBeHidden = true;
            CanHideOthers = true;
            CloseButtonEnabled = true;

            OnEnterCallback = ReturnOk;
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);

            AddCaption(caption, Color.White.ToVector4(), new Vector2(0.0f, 3f / 1000f));

            var controlSeparatorList1 = new MyGuiControlSeparatorList();
            controlSeparatorList1.AddHorizontal(new Vector2(0.0f, 0.0f) - new Vector2((float)(m_size.Value.X * 0.779999971389771 / 2.0), (float)(m_size.Value.Y / 2.0 - 0.0750000029802322)), m_size.Value.X * 0.78f);
            Controls.Add(controlSeparatorList1);

            var controlSeparatorList2 = new MyGuiControlSeparatorList();
            controlSeparatorList2.AddHorizontal(new Vector2(0.0f, 0.0f) - new Vector2((float)(m_size.Value.X * 0.779999971389771 / 2.0), (float)(-(double)m_size.Value.Y / 2.0 + 0.123000003397465)), m_size.Value.X * 0.78f);
            Controls.Add(controlSeparatorList2);

            nameBox = new MyGuiControlTextbox(new Vector2(0.0f, -0.027f), maxLength: maxTextLength);
            nameBox.Text = defaultName;
            nameBox.Size = new Vector2(0.385f, 1f);
            Controls.Add(nameBox);

            okButton = new MyGuiControlButton(originAlign: MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER, text: MyTexts.Get(MyCommonTexts.Ok), onButtonClick: new Action<MyGuiControlButton>(OnOk));
            cancelButton = new MyGuiControlButton(originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, text: MyTexts.Get(MyCommonTexts.Cancel), onButtonClick: new Action<MyGuiControlButton>(OnCancel));

            var okPosition = new Vector2(1f / 500f, (float)((double)m_size.Value.Y / 2.0 - 0.0710000023245811));
            var halfDistance = new Vector2(0.018f, 0.0f);

            okButton.Position = okPosition - halfDistance;
            cancelButton.Position = okPosition + halfDistance;

            okButton.SetToolTip(MyTexts.GetString(MySpaceTexts.ToolTipNewsletter_Ok));
            cancelButton.SetToolTip(MyTexts.GetString(MySpaceTexts.ToolTipOptionsSpace_Cancel));

            Controls.Add(okButton);
            Controls.Add(cancelButton);

            var myGuiControlLabel = new MyGuiControlLabel();
            myGuiControlLabel.Position = okButton.Position;
            myGuiControlLabel.Name = GAMEPAD_HELP_LABEL_NAME;
            myGuiControlLabel.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM;
            Controls.Add(myGuiControlLabel);

            GamepadHelpTextId = MySpaceTexts.DialogBlueprintRename_GamepadHelp;
        }

        public override bool Update(bool hasFocus)
        {
            okButton.Visible = !MyInput.Static.IsJoystickLastUsed;
            cancelButton.Visible = !MyInput.Static.IsJoystickLastUsed;
            return base.Update(hasFocus);
        }

        public override void HandleInput(bool receivedFocusInThisUpdate)
        {
            base.HandleInput(receivedFocusInThisUpdate);

            if (receivedFocusInThisUpdate)
                return;

            if (MyControllerHelper.IsControl(MyControllerHelper.CX_GUI, MyControlsGUI.BUTTON_X))
                OnOk(null);

            if (!MyControllerHelper.IsControl(MyControllerHelper.CX_GUI, MyControlsGUI.BUTTON_B))
                return;

            OnCancel(null);
        }

        private void CallResultCallback(string text)
        {
            if (text == null)
                return;

            callBack(text);
        }

        private void ReturnOk()
        {
            if (nameBox.GetTextLength() <= 0)
                return;

            CallResultCallback(nameBox.Text);
            CloseScreen();
        }

        private void OnOk(MyGuiControlButton button) => ReturnOk();
        private void OnCancel(MyGuiControlButton button) => CloseScreen();

        public override string GetFriendlyName() => "NameDialog";
    }
}