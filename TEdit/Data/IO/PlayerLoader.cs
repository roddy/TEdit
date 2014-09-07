using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEdit.Data.IO
{
    class PlayerLoader
    {
        public static Player Load(BinaryReader reader)
        {
            Player player = new Player();
            return player;
        }
    }
}
