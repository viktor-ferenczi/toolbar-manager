using System;
using System.IO;
using System.Text;
using LitJson;
using Sandbox;
using Sandbox.Game.Gui;
using Sandbox.Graphics.GUI;
using ToolbarManager.Extensions;
using VRage.Game;
using VRage.Utils;
using VRageMath;

// ReSharper disable VirtualMemberCallInConstructor

namespace ToolbarManager.Gui
{
    public class ListDialog : MyGuiScreenDebugBase
    {
        private MyGuiControlTable toolbarTable;
        private MyGuiControlButton newButton;
        private MyGuiControlButton loadButton;
        private MyGuiControlButton mergeButton;
        private MyGuiControlButton renameButton;
        private MyGuiControlButton deleteButton;
        private MyGuiControlButton closeButton;

        private readonly Action<string, bool> callBack;
        private readonly string caption;
        private readonly string defaultName;
        private readonly string dirPath;
        private readonly int[] usedSlotCounts = new int[9];

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

            m_onEnterCallback = ReturnLoad;
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
            toolbarTable = new MyGuiControlTable
            {
                Position = new Vector2(0.001f, -0.5f * DialogSize.Y + 0.1f),
                Size = new Vector2(0.85f * DialogSize.X, DialogSize.Y - 0.25f),
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP,
                ColumnsCount = 10,
                VisibleRowsCount = 15,
            };

            var q = 0.76f;
            var w = 0.22f / 9f;
            toolbarTable.SetCustomColumnWidths(new[] { q, w, w, w, w, w, w, w, w, w });
            toolbarTable.SetColumnName(0, new StringBuilder("Name"));
            toolbarTable.SetColumnComparison(0, CellTextComparison);
            for (var i = 1; i < 10; i++)
                toolbarTable.SetColumnName(i, new StringBuilder($"{i}"));
            toolbarTable.SortByColumn(0);
            ListFiles();
            toolbarTable.ItemDoubleClicked += OnItemDoubleClicked;
            Controls.Add(toolbarTable);
        }

        private int CellTextComparison(MyGuiControlTable.Cell x, MyGuiControlTable.Cell y)
        {
            return TextComparison(x.Text, y.Text);
        }

        private int TextComparison(StringBuilder x, StringBuilder y)
        {
            if (x == null)
                return y == null ? 0 : 1;

            return y == null ? -1 : x.CompareTo(y);
        }

        private void CreateButtons()
        {
            newButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Small,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("New"),
                onButtonClick: OnNew);

            loadButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Small,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("Load"),
                onButtonClick: OnLoad);

            mergeButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Small,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("Merge"),
                onButtonClick: OnMerge);

            renameButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Small,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("Rename"),
                onButtonClick: OnRename);

            deleteButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Small,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("Delete"),
                onButtonClick: OnDelete);

            closeButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Small,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("Close"),
                onButtonClick: OnClose);

            var xs = 0.85f * DialogSize.X;
            var y = 0.5f * (DialogSize.Y - 0.15f);
            newButton.Position = new Vector2(-0.39f * xs, y);
            loadButton.Position = new Vector2(-0.39f * xs, y);
            mergeButton.Position = new Vector2(-0.16f * xs, y);
            renameButton.Position = new Vector2(0.06f * xs, y);
            deleteButton.Position = new Vector2(0.24f * xs, y);
            closeButton.Position = new Vector2(0.42f * xs, y);

            newButton.SetToolTip("Saves the current toolbar as a new profile");
            loadButton.SetToolTip("Loads the selected profile replacing the current toolbar");
            mergeButton.SetToolTip("Merges the selected profile into the current toolbar");
            renameButton.SetToolTip("Renames the selected profile");
            deleteButton.SetToolTip("Deletes the selected profile");
            closeButton.SetToolTip("Closes the dialog with no further changes");

            Controls.Add(newButton);
            Controls.Add(loadButton);
            Controls.Add(mergeButton);
            Controls.Add(renameButton);
            Controls.Add(deleteButton);
            Controls.Add(closeButton);
        }

        private void OnNew(MyGuiControlButton button)
        {
            throw new NotImplementedException();
        }

        private void OnLoad(MyGuiControlButton button)
        {
            ReturnLoad();
        }

        private void OnMerge(MyGuiControlButton button)
        {
            CallResultCallback(SelectedName, true);
            CloseScreen();
        }

        private void OnRename(MyGuiControlButton _)
        {
            var oldName = SelectedName;
            if (oldName == "")
                return;

            MyGuiSandbox.AddScreen(new NameDialog(newName => OnNewNameSpecified(oldName, newName), "Rename saved toolbar", oldName));
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

        private void OnClose(MyGuiControlButton button)
        {
            CloseScreen();
        }

        private void ListFiles()
        {
            foreach (var path in Directory.EnumerateFiles(dirPath))
            {
                if (!path.EndsWith(".xml"))
                    continue;

                AddRowForFile(path);
            }

            if (TryFindListItem(defaultName, out var index))
                toolbarTable.SelectedRowIndex = index;
        }

        private void AddRowForFile(string path)
        {
            var json = ReadJson(path);
            CountUsedSlots(json);

            var fileName = Path.GetFileName(path);
            var name = fileName.Substring(0, fileName.Length - 4);

            var row = new MyGuiControlTable.Row(path);
            row.AddCell(new MyGuiControlTable.Cell(name));
            for (var i = 0; i < 9; i++)
                row.AddCell(new MyGuiControlTable.Cell(usedSlotCounts[i] > 0 ? $"{usedSlotCounts[i]}" : "-"));
            toolbarTable.Add(row);
        }

        private static JsonData ReadJson(string xmlPath)
        {
            var jsonPath = XmlToJsonPath(xmlPath);

            if (!File.Exists(jsonPath))
                return null;

            try
            {
                var jsonText = File.ReadAllText(jsonPath);
                return JsonMapper.ToObject(jsonText);
            }
            catch (JsonException e)
            {
                MyLog.Default.Warning($"ToolbarManager: Failed to load JSON toolbar file \"{jsonPath}\" ({e})");
                return null;
            }
        }

        private static string XmlToJsonPath(string xmlPath)
        {
            return xmlPath.Substring(0, xmlPath.Length - 4) + ".json";
        }

        private void CountUsedSlots(JsonData json)
        {
            for (var i = 0; i < 9; i++)
                usedSlotCounts[i] = 0;

            try
            {
                var slots = json["Slots"];
                if (slots == null)
                    return;

                var slotCount = slots.Count;
                for (var i = 0; i < slotCount; i++)
                {
                    var page = i / 9;
                    if (page > 8)
                        break;
                    if (!(bool) slots[i]["IsEmpty"])
                        usedSlotCounts[page]++;
                }
            }
            catch (SystemException e)
            {
                MyLog.Default.Warning($"ToolbarManager: Failed to count free slots ({e})");
            }
        }

        private void OnItemDoubleClicked(MyGuiControlTable table, MyGuiControlTable.EventArgs args)
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

        private string SelectedName => toolbarTable.SelectedRow?.GetCell(0)?.Text?.ToString() ?? "";

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

            var oldJsonPath = XmlToJsonPath(oldPath);
            var newJsonPath = XmlToJsonPath(newPath);

            if (File.Exists(newPath))
                File.Delete(newPath);

            if (File.Exists(newJsonPath))
                File.Delete(newJsonPath);

            if (File.Exists(oldPath))
                File.Move(oldPath, newPath);

            if (File.Exists(oldJsonPath))
                File.Move(oldJsonPath, newJsonPath);

            if (TryFindListItem(newName, out var overwrittenItemIndex))
                toolbarTable.Remove(toolbarTable.GetRow(overwrittenItemIndex));

            if (TryFindListItem(oldName, out var renamedItemIndex))
            {
                var sb = toolbarTable.GetRow(renamedItemIndex).GetCell(0).Text;
                sb.Clear();
                sb.Append(newName);
            }
        }

        private void OnDeleteForSure(MyGuiScreenMessageBox.ResultEnum result, string name)
        {
            if (result != MyGuiScreenMessageBox.ResultEnum.YES)
                return;

            var path = Path.Combine(dirPath, $"{name}.xml");
            var jsonPath = XmlToJsonPath(path);

            if (File.Exists(path))
                File.Delete(path);

            if (File.Exists(jsonPath))
                File.Delete(jsonPath);

            if (TryFindListItem(name, out var index))
                toolbarTable.Remove(toolbarTable.GetRow(index));
        }

        private bool TryFindListItem(string name, out int index)
        {
            var count = toolbarTable.RowsCount;
            for (index = 0; index < count; index++)
            {
                if (toolbarTable.GetRow(index).GetCell(0).Text.ToString() == name)
                    return true;
            }

            index = -1;
            return false;
        }

        public override string GetFriendlyName() => "ListDialog";
    }
}