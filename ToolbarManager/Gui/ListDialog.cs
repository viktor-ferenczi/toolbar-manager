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


namespace ToolbarManager.Gui
{
    // ReSharper disable VirtualMemberCallInConstructor
    public class ListDialog : MyGuiScreenDebugBase
    {
        private MyGuiControlTable profilesTable;
        private MyGuiControlButton newButton, updateButton, loadButton, mergeButton, renameButton, deleteButton, closeButton;

        private readonly Action<string, bool> callBack;
        private readonly string caption;
        private readonly string dirPath;
        private readonly int[] usedSlotCounts = new int[9];

        public ListDialog(Action<string, bool> callBack,
            string caption,
            string dirPath)
            : base(new Vector2(0.5f, 0.5f), new Vector2(1f, 0.8f), MyGuiConstants.SCREEN_BACKGROUND_COLOR * MySandboxGame.Config.UIBkOpacity, true)
        {
            this.callBack = callBack;
            this.caption = caption;
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

            CreateTable();
            CreateButtons();
        }

        private Vector2 DialogSize => m_size ?? Vector2.One;

        private void CreateTable()
        {
            profilesTable = new MyGuiControlTable
            {
                Position = new Vector2(0.001f, -0.5f * DialogSize.Y + 0.08f),
                Size = new Vector2(0.9f * DialogSize.X, DialogSize.Y - 0.1f),
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP,
                ColumnsCount = 10,
                VisibleRowsCount = 17,
            };

            var q = 0.76f;
            var w = 0.22f / 9f;
            profilesTable.SetCustomColumnWidths(new[] { q, w, w, w, w, w, w, w, w, w });
            profilesTable.SetColumnName(0, new StringBuilder("Name"));
            profilesTable.SetColumnComparison(0, CellTextComparison);
            for (var i = 1; i < 10; i++)
                profilesTable.SetColumnName(i, new StringBuilder($"{i}"));
            profilesTable.SortByColumn(0);

            AddTableRows();

            profilesTable.ItemSelected += OnItemSelected;
            profilesTable.ItemDoubleClicked += OnItemDoubleClicked;

            Controls.Add(profilesTable);
        }

        private void OnItemSelected(MyGuiControlTable table, MyGuiControlTable.EventArgs args)
        {
            UpdateButtons();
        }

        private void OnItemDoubleClicked(MyGuiControlTable table, MyGuiControlTable.EventArgs args)
        {
            ReturnLoad();
        }

        private void UpdateButtons()
        {

            var isProfileSelected = profilesTable.SelectedRowIndex != null && profilesTable.SelectedRow.UserData != null;

            newButton.Visible = !isProfileSelected;
            updateButton.Visible = isProfileSelected;
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

            updateButton = new MyGuiControlButton(
                visualStyle: MyGuiControlButtonStyleEnum.Small,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                text: new StringBuilder("Update"),
                onButtonClick: OnUpdate);

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

            var y = 0.5f * (DialogSize.Y - 0.15f);

            var w = 0.85f * DialogSize.X;
            var s = w / 6f;
            var x = -2.5f * s;
            var i = 0;

            newButton.Position = new Vector2(x, y);
            updateButton.Position = new Vector2(x, y);
            loadButton.Position = new Vector2(++i * s + x, y);
            mergeButton.Position = new Vector2(++i * s + x, y);
            renameButton.Position = new Vector2(++i * s + x, y);
            deleteButton.Position = new Vector2(++i * s + x, y);
            closeButton.Position = new Vector2(++i * s + x, y);

            newButton.SetMaxWidth(0.04f);
            updateButton.SetMaxWidth(0.08f);
            loadButton.SetMaxWidth(0.08f);
            mergeButton.SetMaxWidth(0.08f);
            renameButton.SetMaxWidth(0.08f);
            deleteButton.SetMaxWidth(0.08f);
            closeButton.SetMaxWidth(0.08f);

            newButton.SetToolTip("Saves the current toolbar as a new profile");
            updateButton.SetToolTip("Updates the selected profile with the current toolbar");
            loadButton.SetToolTip("Loads the selected profile replacing the current toolbar");
            mergeButton.SetToolTip("Merges the selected profile into the current toolbar");
            renameButton.SetToolTip("Renames the selected profile");
            deleteButton.SetToolTip("Deletes the selected profile");
            closeButton.SetToolTip("Closes the dialog with no further changes");

            Controls.Add(newButton);
            Controls.Add(updateButton);
            Controls.Add(loadButton);
            Controls.Add(mergeButton);
            Controls.Add(renameButton);
            Controls.Add(deleteButton);
            Controls.Add(closeButton);

            UpdateButtons();
        }

        private void OnNew(MyGuiControlButton button)
        {
            // Storage.OnSaveToolbar();
        }

        private void OnUpdate(MyGuiControlButton obj)
        {
            // Storage.OnSaveToolbar();
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

        private void AddTableRows()
        {
            foreach (var path in Directory.EnumerateFiles(dirPath))
            {
                if (!path.EndsWith(".xml"))
                    continue;

                AddRowForFile(path);
            }

            profilesTable.Add(new MyGuiControlTable.Row());
            profilesTable.SelectedRowIndex = profilesTable.Rows.Count - 1;
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
            profilesTable.Add(row);
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

        private string SelectedName => profilesTable.SelectedRow?.GetCell(0)?.Text?.ToString() ?? "";

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
                profilesTable.Remove(profilesTable.GetRow(overwrittenItemIndex));

            if (TryFindListItem(oldName, out var renamedItemIndex))
            {
                var sb = profilesTable.GetRow(renamedItemIndex).GetCell(0).Text;
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
                profilesTable.Remove(profilesTable.GetRow(index));
        }

        private bool TryFindListItem(string name, out int index)
        {
            var count = profilesTable.RowsCount;
            for (index = 0; index < count; index++)
            {
                if (profilesTable.GetRow(index).GetCell(0).Text.ToString() == name)
                    return true;
            }

            index = -1;
            return false;
        }

        public override string GetFriendlyName() => "ListDialog";
    }
}