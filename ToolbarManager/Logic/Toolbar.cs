using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Sandbox.Game.Screens.Helpers;
using VRage.Game;
using VRage.ObjectBuilders;

namespace ToolbarManager.Logic
{
    public class Toolbar
    {
        private readonly List<Slot> slots = new List<Slot>();

        public Toolbar(MyToolbar toolbar)
        {
            if (toolbar == null)
                return;

            for (var index = 0; index < toolbar.ItemCount; index++)
                slots.Add(new Slot(index, toolbar.GetItemAtIndex(index)));
        }

        public Toolbar(MyObjectBuilder_Toolbar toolbarBuilder)
        {
            if (toolbarBuilder == null)
                return;

            for (var index = 0; index < toolbarBuilder.Slots.Count; index++)
            {
                slots.Add(new Slot(index, toolbarBuilder.Slots[index].Data));
            }
        }

        public static Toolbar Read(string path)
        {
            using (var stream = new StreamReader(path))
                return Read(stream);
        }

        public static Toolbar Read(StreamReader stream)
        {
            MyObjectBuilderSerializer.DeserializeXML<MyObjectBuilder_Toolbar>(stream.BaseStream, out var toolbarBuilder);
            return new Toolbar(toolbarBuilder);
        }

        public void Write(string path)
        {
            using (var stream = new StreamWriter(path))
                Write(stream);
        }

        public void Write(StreamWriter stream)
        {
            var ob = new MyObjectBuilder_Toolbar();
            ob.ToolbarType = MyToolbarType.Character;
            for (var index = 0; index < slots.Count; index++)
            {
                var slot = new MyObjectBuilder_Toolbar.Slot
                {
                    Index = index,
                    Data = slots[index].Builder
                };
                ob.Slots.Add(slot);
            }

            MyObjectBuilderSerializer.SerializeXML(stream.BaseStream, ob);
        }

        public void Set(MyToolbar toolbar)
        {
            var count = Math.Min(slots.Count, toolbar.ItemCount);
            for (var index = 0; index < count; index++)
                slots[index].Set(toolbar);
        }

        public void Merge(MyToolbar toolbar)
        {
            var count = Math.Min(slots.Count, toolbar.ItemCount);
            for (var index = 0; index < count; index++)
                slots[index].Merge(toolbar);
        }
    }
}