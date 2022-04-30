using Sandbox.Game.Entities;
using Sandbox.Game.Gui;

namespace ToolbarManager.Gui
{
    public class CustomToolbarConfigScreen: MyGuiScreenCubeBuilder
    {
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
    }
}