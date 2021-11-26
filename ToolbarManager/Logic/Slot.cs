using Sandbox.Definitions;
using Sandbox.Game.Screens.Helpers;
using VRage.Game;

namespace ToolbarManager.Logic
{
    public class Slot
    {
        public readonly int Index;
        public MyDefinitionId Id { get; private set; }
        public MyObjectBuilder_ToolbarItem Builder { get; private set; }

        public int Page => Index / 9;
        public int Number => Index % 9;
        public bool IsEmpty => Builder == null;

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
            Builder = MyToolbarItemFactory.ObjectBuilderFromDefinition(definitionBase);
        }

        public Slot(int index, MyToolbarItem item)
        {
            Index = index;

            if (item == null)
                return;

            Builder = item.GetObjectBuilder();
            Id = Builder.GetId();
        }

        public Slot(int index, MyObjectBuilder_ToolbarItem itemBuilder)
        {
            Index = index;

            if (itemBuilder == null)
                return;

            Builder = itemBuilder;
            Id = itemBuilder.GetId();
        }

        public Slot(int index, MyToolbarItemDefinition itemDefinition)
        {
            Index = index;

            if (itemDefinition == null)
                return;

            Id = itemDefinition.Definition.Id;
            Builder = itemDefinition.GetObjectBuilder();
        }

        public void Clear()
        {
            Id = default(MyDefinitionId);
            Builder = null;
        }

        public void Get(MyToolbar toolbar)
        {
            var toolbarItem = toolbar.GetItemAtIndex(Index);
            if (toolbarItem == null)
            {
                Clear();
                return;
            }

            Builder = toolbarItem.GetObjectBuilder();
            Id = Builder.GetId();
        }

        public void Set(MyToolbar toolbar)
        {
            if (IsEmpty)
            {
                toolbar.SetItemAtIndex(Index, null);
                return;
            }

            var toolbarItem = MyToolbarItemFactory.CreateToolbarItem(Builder);
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