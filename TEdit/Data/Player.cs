using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TEdit.Data.Types;

namespace TEdit.Data
{
    class Player
    {
        #region Properties

        /// <summary>
        /// The player's name.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The game's difficulty level. In older versions of Terraria, this 
        /// was a simple boolean easy/hard.
        /// </summary>
        public byte Difficulty { get; set; }

        /// <summary>
        /// The player's hair style. Color set seperately.
        /// </summary>
        public Int32 Hair { get; set; }

        /// <summary>
        /// A dye, if any, applied to the player's hair.
        /// </summary>
        public byte HairDye { get; set; }

        /// <summary>
        /// Flag called 'hide visual'. Pulled from Terraria source, not sure what it does.
        /// </summary>
        public BitsByte HideVisual { get; set; }

        /// <summary>
        /// Boolean flag. If true, the player is male; false otherwise.
        /// </summary>
        public bool IsMale { get; set; }

        /// <summary>
        /// Player's current hit points.
        /// </summary>
        public int currentLife { get; set; }

        /// <summary>
        /// Player's maximum hit points.
        /// </summary>
        public int maxLife { get; set; }

        /// <summary>
        /// Player's current mana.
        /// </summary>
        public int currentMana { get; set; }

        /// <summary>
        /// Player's maximum mana.
        /// </summary>
        public int maxMana { get; set; }

        /// <summary>
        /// Player's (natural, un-dyed) hair color.
        /// </summary>
        public Color HairColor { get; set; }

        /// <summary>
        /// Player's skin color.
        /// </summary>
        public Color SkinColor { get; set; }

        /// <summary>
        /// Player's eye color.
        /// </summary>
        public Color EyeColor { get; set; }

        /// <summary>
        /// Player's shirt color.
        /// </summary>
        public Color ShirtColor { get; set; }

        /// <summary>
        /// Player's undershirt color.
        /// </summary>
        public Color UndershirtColor { get; set; }

        /// <summary>
        /// Player's pants color.
        /// </summary>
        public Color PantsColor { get; set; }

        /// <summary>
        /// Player's shoe color.
        /// </summary>
        public Color ShoeColor { get; set; }

        #endregion
    }
}
