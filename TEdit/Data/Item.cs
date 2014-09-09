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
        [System.ComponentModel.DefaultValue(0)]
        public Int32 StackSize { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public Int32 Id { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public int Prefix { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string Name { get; set; }
        
        public override string ToString()
        {
            return string.Format("{0} {1} ({2}) x{3}", Prefix, Name, Id, StackSize);
        }

        public override bool Equals(System.Object obj)
        {
            Buff b = obj as Buff;
            if ((object)b == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Equals(b);
        }

        public bool Equals(Item i)
        {

            if (i == null)
            {
                return false;
            }

            return StackSize.Equals(i.StackSize) &&
                   Id.Equals(i.Id) &&
                   IsBothNullOrEqual(Name, i.Name) &&
                   Prefix == i.Prefix;
        }
        
        private static bool IsBothNullOrEqual(string a, string b)
        {
            return (a == null && b == null) ||
                (a != null && b != null && a.Equals(b));
        }

        public override int GetHashCode()
        {
            return StackSize.GetHashCode() ^ Id.GetHashCode() ^ Name.GetHashCode() ^ Prefix.GetHashCode();
        }
    }
}
