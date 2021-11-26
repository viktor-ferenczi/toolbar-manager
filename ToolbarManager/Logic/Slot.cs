using Sandbox.Definitions;
using Sandbox.Game.Screens.Helpers;
using VRage.Game;

namespace ToolbarManager.Logic
{
    public class Slot
    {
        public int Index;
        public MyDefinitionId Id;

        private MyObjectBuilder_ToolbarItem builder;

        public int Page => Index / 9;
        public int Number => Index % 9;

        public bool IsEmpty => builder == null;

        public Slot()
        {
        }

        public Slot(int index)
        {
            Index = index;
        }

        public Slot(int index, MyDefinitionId id)
        {
            Index = index;

            if (!MyDefinitionManager.Static.TryGetDefinition(Id, out MyDefinitionBase definitionBase))
                return;

            Id = id;
            builder = MyToolbarItemFactory.ObjectBuilderFromDefinition(definitionBase);
        }

        public Slot(int index, MyToolbarItem item)
        {
            Index = index;

            if (item == null)
                return;

            builder = item.GetObjectBuilder();
            Id = builder.GetId();
        }

        public Slot(int index, MyToolbarItemDefinition item)
        {
            Index = index;

            if (item == null)
                return;

            Id = item.Definition.Id;
            builder = item.GetObjectBuilder();
        }

        public void Clear()
        {
            Id = default(MyDefinitionId);
            builder = null;
        }

        public void Get(MyToolbar toolbar)
        {
            var toolbarItem = toolbar.GetItemAtIndex(Index);
            if (toolbarItem == null)
            {
                Clear();
                return;
            }

            builder = toolbarItem.GetObjectBuilder();
            Id = builder.GetId();
        }

        public void Set(MyToolbar toolbar)
        {
            if (IsEmpty)
            {
                toolbar.SetItemAtIndex(Index, null);
                return;
            }

            var toolbarItem = MyToolbarItemFactory.CreateToolbarItem(builder);
            toolbar.SetItemAtIndex(Index, toolbarItem);
        }

        public void Merge(MyToolbar toolbar)
        {
            if (IsEmpty)
                return;

            Set(toolbar);
        }

        public void Activate(MyToolbar toolbar)
        {
            if (toolbar.CurrentPage != Page)
                toolbar.SwitchToPage(Page);

            toolbar.ActivateItemAtIndex(Index);
        }
    }
}