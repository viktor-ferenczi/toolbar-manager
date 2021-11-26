using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Sandbox.Game.Screens.Helpers;

namespace ToolbarManager.Logic
{
    public class Toolbar
    {
        private static XmlSerializer Serializer => new XmlSerializer(typeof(Toolbar));

        public readonly List<Slot> Slots = new List<Slot>();

        public Toolbar()
        {
        }

        public Toolbar(int count)
        {
            for (var index = 0; index < count; index++)
                Slots.Add(new Slot(index));
        }

        public Toolbar(MyToolbar toolbar)
        {
            for (var index = 0; index < toolbar.ItemCount; index++)
                Slots.Add(new Slot(index, toolbar.GetItemAtIndex(index)));
        }

        public static Toolbar Read(string path)
        {
            using (var stream = new StreamReader(path))
                return Read(stream);
        }

        public static Toolbar Read(StreamReader stream)
        {
            return (Toolbar)Serializer.Deserialize(stream);
        }

        public void Write(string path)
        {
            using (var stream = new StreamWriter(path))
                Write(stream);
        }

        public void Write(StreamWriter stream)
        {
            Serializer.Serialize(stream, this);
        }

        public void Set(MyToolbar toolbar)
        {
            var count = Math.Min(Slots.Count, toolbar.ItemCount);
            for (var index = 0; index < count; index++)
                Slots[index].Set(toolbar);
        }

        public void Merge(MyToolbar toolbar)
        {
            var count = Math.Min(Slots.Count, toolbar.ItemCount);
            for (var index = 0; index < count; index++)
                Slots[index].Merge(toolbar);
        }
    }
}