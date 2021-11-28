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

// ReSharper disable VirtualMemberCallInConstructor

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

            AddCaption(caption, Color.White.ToVector4(), new Vector2(0.0f, 0.003f));

            CreateListBox();
            CreateButtons();
        }

        private Vector2 DialogSize => m_size ?? Vector2.One;

        private void CreateListBox()
        {
            const float scrollbarWidth = 0.041f;
            const float listItemHeight = 0.034f;
            listBox = new MyGuiControlListbox(new Vector2(0.001f, -0.5f * DialogSize.Y + 0.1f))
            {
                MultiSelect = false,
                VisualStyle = MyGuiControlListboxStyleEnum.Default,
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP,
                VisibleRowsCount = (int)((DialogSize.Y - 0.25f) / listItemHeight),
                Size = new Vector2(0.85f * DialogSize.X, DialogSize.Y - 0.25f)
            };
            ListFiles();
            listBox.ItemSize = new Vector2(0.85f * DialogSize.X - scrollbarWidth, listBox.ItemSize.Y);
            listBox.ItemDoubleClicked += OnItemDoubleClicked;
            Controls.Add(listBox);
        }

        private void CreateButtons()
        {
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

            var xs = 0.85f * DialogSize.X;
            var y = 0.5f * (DialogSize.Y - 0.15f);
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
            var oldName = SelectedName;
            if (oldName == "")
                return;

            MyGuiSandbox.AddScreen(new NameDialog(newName => OnNewNameSpecified(oldName, newName), "Rename saved toolbar", oldName));
        }

        private void OnNewNameSpecified(string oldName, string newName)
        {
            newName = PathExt.SanitizeFileName(newName);

            var newPath = Path.Combine(dirPath, $"{newName}.xml");
            if (File.Exists(newPath))
            {
                MyGuiSandbox.AddScreen(
                    MyGuiSandbox.CreateMessageBox(buttonType: MyMessageBoxButtonsType.YES_NO,
                        messageText: new StringBuilder($"Are you sure to overwrite this saved toolbar?\r\n\r\n{newName}"),
                        messageCaption: new StringBuilder("Confirmation"),
                        callback: result => OnOverwriteForSure(result, oldName, newName)));
            }
            else
            {
                OnOverwriteForSure(MyGuiScreenMessageBox.ResultEnum.YES, oldName, newName);
            }
        }

        private void OnOverwriteForSure(MyGuiScreenMessageBox.ResultEnum result, string oldName, string newName)
        {
            if (result != MyGuiScreenMessageBox.ResultEnum.YES)
                return;

            var oldPath = Path.Combine(dirPath, $"{oldName}.xml");
            var newPath = Path.Combine(dirPath, $"{newName}.xml");

            if (File.Exists(newPath))
                File.Delete(newPath);

            if (File.Exists(oldPath))
                File.Move(oldPath, newPath);

            if (TryFindListItem(newName, out var overwrittenItemIndex))
                listBox.Items.RemoveAt(overwrittenItemIndex);

            if (TryFindListItem(oldName, out var renamedItemIndex))
                listBox.Items[renamedItemIndex].Text = new StringBuilder(newName);
        }

        private void OnDelete(MyGuiControlButton _)
        {
            var name = SelectedName;
            if (name == "")
                return;

            MyGuiSandbox.AddScreen(
                MyGuiSandbox.CreateMessageBox(buttonType: MyMessageBoxButtonsType.YES_NO,
                    messageText: new StringBuilder($"Are you sure to delete this saved toolbar?\r\n\r\n{name}"),
                    messageCaption: new StringBuilder("Confirmation"),
                    callback: result => OnDeleteForSure(result, name)));
        }

        private void OnDeleteForSure(MyGuiScreenMessageBox.ResultEnum result, string name)
        {
            if (result != MyGuiScreenMessageBox.ResultEnum.YES)
                return;

            var path = Path.Combine(dirPath, $"{name}.xml");
            if (File.Exists(path))
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