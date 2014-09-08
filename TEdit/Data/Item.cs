using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEdit.Data
{
    /// <summary>
    /// Models an in-game Item. At this point it's only going to hold the properties we need
    /// for the Player editor, though we can conceivably implement an Item editor. (TODO)
    /// </summary>
    class Item
    {
        public Int32 StackSize { get; set; }
        public Int32 Id { get; set; }
        public int Prefix { get; set; }
        public string Name { get; set; }

        public Item()
        {
            Id = 0; // "No item"
            StackSize = 0;
            Prefix = 0;
            Name = "";
        }
    }
}
