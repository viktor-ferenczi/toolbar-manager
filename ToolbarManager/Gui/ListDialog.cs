using System;
using System.IO;
using System.Text;
using Sandbox;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using ToolbarManager.Extensions;
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
        private MyGuiControlButton renameButton;
        private MyGuiControlButton deleteButton;
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

            const float scrollbarWidth = 0.041f;
            const float listItemHeight = 0.034f;
            listBox = new MyGuiControlListbox(new Vector2(0.001f, -0.5f * dialogSize.Y + 0.1f))
            {
                MultiSelect = false,
                VisualStyle = MyGuiControlListboxStyleEnum.Default,
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP,
                VisibleRowsCount = (int)((dialogSize.Y - 0.25f) / listItemHeight),
                Size = new Vector2(0.85f * dialogSize.X, dialogSize.Y - 0.25f)
            };
            ListFiles();
            listBox.ItemSize = new Vector2(0.85f * dialogSize.X - scrollbarWidth, listBox.ItemSize.Y);
            listBox.ItemDoubleClicked += OnItemDoubleClicked;
            Controls.Add(listBox);

            loadButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Default,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("Load"), onButtonClick: OnLoad);

            mergeButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Default,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("Merge"), onButtonClick: OnMerge);

            renameButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Small,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("Rename"), onButtonClick: OnRename);

            deleteButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Small,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("Delete"), onButtonClick: OnDelete);

            cancelButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Small,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: MyTexts.Get(MyCommonTexts.Cancel), onButtonClick: OnCancel);

            var xs = 0.85f * dialogSize.X;
            var y = 0.5f * (dialogSize.Y - 0.15f);
            loadButton.Position = new Vector2(-0.39f * xs, y);
            mergeButton.Position = new Vector2(-0.16f * xs, y);
            renameButton.Position = new Vector2(0.06f * xs, y);
            deleteButton.Position = new Vector2(0.24f * xs, y);
            cancelButton.Position = new Vector2(0.42f * xs, y);

            loadButton.SetToolTip("Loads the selected toolbar replacing the current one");
            mergeButton.SetToolTip("Merges the selected toolbar into the current one");
            renameButton.SetToolTip("Renames the selected toolbar save file");
            deleteButton.SetToolTip("Deletes the selected toolbar save file");
            cancelButton.SetToolTip(MyTexts.GetString(MySpaceTexts.ToolTipOptionsSpace_Cancel));

            Controls.Add(loadButton);
            Controls.Add(mergeButton);
            Controls.Add(renameButton);
            Controls.Add(deleteButton);
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
            }

            if (TryFindListItem(defaultName, out var index))
                listBox.SelectSingleItem(listBox.Items[index]);
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

        private void OnRename(MyGuiControlButton _)
        {
            var name = SelectedName;
            if (name == "")
                return;

            MyGuiSandbox.AddScreen(new NameDialog(OnNewNameSpecified, "Rename saved toolbar", name));
        }

        private void OnNewNameSpecified(string newName)
        {
            newName = PathExt.SanitizeFileName(newName);

            var oldName = SelectedName;
            if (oldName == "")
                return;

            var oldPath = Path.Combine(dirPath, $"{oldName}.xml");
            if (!File.Exists(oldPath))
                return;

            var newPath = Path.Combine(dirPath, $"{newName}.xml");
            File.Move(oldPath, newPath);

            if (TryFindListItem(oldName, out var index))
                listBox.Items[index].Text = new StringBuilder(newName);
        }

        private void OnDelete(MyGuiControlButton _)
        {
            var name = SelectedName;
            if (name == "")
                return;

            MyGuiSandbox.AddScreen(
                MyGuiSandbox.CreateMessageBox(buttonType: MyMessageBoxButtonsType.YES_NO,
                    messageText: new StringBuilder($"Are you sure to delete saved toolbar?\r\n\r\n{name}"),
                    messageCaption: new StringBuilder("Confirmation"),
                    callback: OnDeleteForSure));
        }

        private void OnDeleteForSure(MyGuiScreenMessageBox.ResultEnum result)
        {
            if (result != MyGuiScreenMessageBox.ResultEnum.YES)
                return;

            var name = SelectedName;
            if (name == "")
                return;

            var path = Path.Combine(dirPath, $"{name}.xml");
            if (!File.Exists(path))
                return;

            File.Delete(path);

            if (TryFindListItem(name, out var index))
                listBox.Items.RemoveAt(index);
        }

        private bool TryFindListItem(string name, out int index)
        {
            var count = listBox.Items.Count;
            for (index = 0; index < count; index++)
            {
                if (listBox.Items[index].Text.ToString() == name)
                    return true;
            }

            index = -1;
            return false;
        }

        public override string GetFriendlyName() => "ListDialog";
    }
}