using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEdit.Data
{
    class Buff
    {
        [System.ComponentModel.DefaultValue(0)]
        public Int32 Type { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public Int32 Time { get; set; }
        
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

        public bool Equals(Buff b)
        {

            if (b == null)
            {
                return false;
            }

            return Type.Equals(b.Type) &&
                   Time.Equals(b.Time);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ Time.GetHashCode();
        }
    }
}
