using System.Diagnostics.CodeAnalysis;
using Sandbox.Game.Entities;
using Sandbox.Game.Gui;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Graphics.GUI;
using ToolbarManager.Extensions;
using VRage.Game;
using VRage.Input;
using VRageMath;

namespace ToolbarManager.Gui
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class CustomToolbarConfigScreen : MyGuiScreenCubeBuilder
    {
        private readonly CustomSearchCondition customSearchCondition = new CustomSearchCondition();
        private MyGuiControlLabel searchInfoLabel;

        public CustomToolbarConfigScreen(int scrollOffset = 0, MyCubeBlock owner = null, int? gamepadSlot = null) : base(scrollOffset, owner, gamepadSlot)
        {
        }

        public CustomToolbarConfigScreen(int scrollOffset, MyCubeBlock owner, string selectedPage, bool hideOtherPages, int? gamepadSlot = null) : base(scrollOffset, owner, selectedPage, hideOtherPages, gamepadSlot)
        {
        }

        public void SetSearchText(string text)
        {
            m_searchBox.SearchText = text;
            m_searchBox.TextBox.MoveCarriageToEnd();
        }

        protected override void AddToolsAndAnimations(IMySearchCondition searchCondition)
        {
            if (searchCondition == m_nameSearchCondition)
            {
                customSearchCondition.SearchName = m_searchBox.TextBox.Text;
                base.AddToolsAndAnimations(customSearchCondition);
                return;
            }

            base.AddToolsAndAnimations(searchCondition);
        }

        protected override void UpdateGridBlocksBySearchCondition(IMySearchCondition searchCondition)
        {
            if (searchCondition == m_nameSearchCondition)
            {
                customSearchCondition.SearchName = m_searchBox.TextBox.Text;
                base.UpdateGridBlocksBySearchCondition(customSearchCondition);
                return;
            }

            base.UpdateGridBlocksBySearchCondition(searchCondition);
        }

        public override void HandleInput(bool receivedFocusInThisUpdate)
        {
            if (MyInput.Static.IsNewKeyPressed(MyKeys.Enter))
            {
                LoadSelectedItem();
                return;
            }

            base.HandleInput(receivedFocusInThisUpdate);
        }

        private void LoadSelectedItem()
        {
            if (m_gridBlocks.SelectedIndex == null)
                return;

            var selectedItem = m_gridBlocks.GetItemAt(m_gridBlocks.SelectedIndex ?? 0);
            if (!(selectedItem?.UserData is GridItemUserData userData))
                return;

            var toolbarItemBuilder = userData.ItemData();
            if (toolbarItemBuilder is MyObjectBuilder_ToolbarItemEmpty)
                return;

            var toolbarItem = MyToolbarItemFactory.CreateToolbarItem(userData.ItemData());
            if (toolbarItem is MyToolbarItemActions && MyInput.Static.IsJoystickLastUsed)
                return;

            this.AddGridItemToToolbar(toolbarItemBuilder);

            CloseScreen();
        }

        public override void RecreateControls(bool contructor)
        {
            base.RecreateControls(contructor);

            searchInfoLabel = new MyGuiControlLabel(
                new Vector2(m_searchBox.PositionX + m_searchBox.Size.X * 0.5f + 0.02f, m_searchBox.PositionY + 0.005f),
                new Vector2(0.2f, m_searchBox.Size.Y))
            {
                Font = m_searchBox.TextBox.TextFont,
                Text = "Quick Search Mode"
            };

            Controls.Add(searchInfoLabel);
        }
    }
}