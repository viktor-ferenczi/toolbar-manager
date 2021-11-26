using System;
using System.IO;
using System.Text;
using Sandbox;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using VRage;
using VRage.Game;
using VRage.Utils;
using VRageMath;

namespace ToolbarManager.Gui
{
    class ListDialog : MyGuiScreenDebugBase
    {
        private MyGuiControlListbox listBox;
        private MyGuiControlButton loadButton;
        private MyGuiControlButton mergeButton;
        private MyGuiControlButton cancelButton;

        private readonly Action<string, bool> callBack;
        private readonly string caption;
        private readonly string defaultName;
        private readonly string dirPath;

        public ListDialog(
            Action<string, bool> callBack,
            string caption,
            string defaultName,
            string dirPath)
            : base(new Vector2(0.5f, 0.5f), new Vector2(1f, 0.8f), MyGuiConstants.SCREEN_BACKGROUND_COLOR * MySandboxGame.Config.UIBkOpacity, true)
        {
            this.callBack = callBack;
            this.caption = caption;
            this.defaultName = defaultName;
            this.dirPath = dirPath;

            RecreateControls(true);

            CanBeHidden = true;
            CanHideOthers = true;
            CloseButtonEnabled = true;

            OnEnterCallback = ReturnLoad;
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);

            var dialogSize = m_size ?? Vector2.One;

            AddCaption(caption, Color.White.ToVector4(), new Vector2(0.0f, 0.003f));

            listBox = new MyGuiControlListbox(new Vector2(0.001f, -0.5f * dialogSize.Y + 0.1f))
            {
                IsAutoScaleEnabled = false,
                MultiSelect = false,
                VisibleRowsCount = 10,
                VisualStyle = MyGuiControlListboxStyleEnum.Default,
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP
            };
            ListFiles();
            // Keen!!!!!! Adding the items overrides Size.Y to be 1f, so the Size has to be set afterwards.
            listBox.Size = new Vector2(0.85f * dialogSize.X, dialogSize.Y - 0.25f);
            listBox.ItemDoubleClicked += OnItemDoubleClicked;
            Controls.Add(listBox);

            loadButton = new MyGuiControlButton(originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, text: new StringBuilder("Load"), onButtonClick: OnLoad);
            mergeButton = new MyGuiControlButton(originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, text: new StringBuilder("Merge"), onButtonClick: OnMerge);
            cancelButton = new MyGuiControlButton(originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, text: MyTexts.Get(MyCommonTexts.Cancel), onButtonClick: OnCancel);

            var middlePosition = new Vector2(0.001f, 0.5f * dialogSize.Y - 0.071f);
            var spacing = new Vector2(0.2f, 0.0f);

            loadButton.Position = middlePosition - spacing;
            mergeButton.Position = middlePosition;
            cancelButton.Position = middlePosition + spacing;

            loadButton.SetToolTip("Loads the selected toolbar replacing the current one");
            mergeButton.SetToolTip("Merges the selected toolbar into the current one");
            cancelButton.SetToolTip(MyTexts.GetString(MySpaceTexts.ToolTipOptionsSpace_Cancel));

            Controls.Add(loadButton);
            Controls.Add(mergeButton);
            Controls.Add(cancelButton);

            var myGuiControlLabel = new MyGuiControlLabel
            {
                Position = loadButton.Position,
                Name = GAMEPAD_HELP_LABEL_NAME,
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM
            };
            Controls.Add(myGuiControlLabel);

            GamepadHelpTextId = MySpaceTexts.DialogBlueprintRename_GamepadHelp;
        }

        private void ListFiles()
        {
            var index = 0;
            var selectByDefault = 0;
            foreach (var path in Directory.EnumerateFiles(dirPath))
            {
                if (!path.EndsWith(".xml"))
                    continue;

                var fileName = Path.GetFileName(path);
                var name = fileName.Substring(0, fileName.Length - 4);

                listBox.Items.Add(new MyGuiControlListbox.Item
                {
                    Text = new StringBuilder(name)
                });

                if (name == defaultName)
                    selectByDefault = index;

                index++;
            }

            listBox.Size = new Vector2(0.385f, 1f);
            if (listBox.Items.Count > 0)
                listBox.SelectSingleItem(listBox.Items[selectByDefault]);
        }

        // public override bool Update(bool hasFocus)
        // {
        //     loadButton.Visible = !MyInput.Static.IsJoystickLastUsed;
        //     cancelButton.Visible = !MyInput.Static.IsJoystickLastUsed;
        //     return base.Update(hasFocus);
        // }

        private void OnItemDoubleClicked(MyGuiControlListbox _)
        {
            ReturnLoad();
        }

        private void CallResultCallback(string text, bool merge)
        {
            if (text == null)
                return;

            callBack(text, merge);
        }

        private void ReturnLoad()
        {
            CallResultCallback(SelectedName, false);
            CloseScreen();
        }

        private void ReturnMerge()
        {
            CallResultCallback(SelectedName, true);
            CloseScreen();
        }

        private string SelectedName => listBox.GetLastSelected()?.Text?.ToString() ?? "";

        private void OnLoad(MyGuiControlButton button) => ReturnLoad();
        private void OnMerge(MyGuiControlButton button) => ReturnMerge();
        private void OnCancel(MyGuiControlButton button) => CloseScreen();

        public override string GetFriendlyName() => "ListDialog";
    }
}