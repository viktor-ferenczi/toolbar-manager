using System.Diagnostics.CodeAnalysis;
using Sandbox.Game.Entities;
using Sandbox.Game.Gui;

namespace ToolbarManager.Gui
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class CustomToolbarConfigScreen : MyGuiScreenCubeBuilder
    {
        private readonly CustomSearchCondition customSearchCondition = new CustomSearchCondition();

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
    }
}