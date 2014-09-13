using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TEdit.Data.Types;
using TEdit.Data.Util;

namespace TEdit.Data
{
    class Player
    {
        #region Properties

        /// <summary>
        /// The release version for the given PLR file. This is important because certain features weren't added
        /// until certain releases, so the underlying file structure changed.
        /// </summary>
        [DefaultValue(0)]
        public Int32 Release { get; set; }

        /// <summary>
        /// The player's name.
        /// </summary>
        [DefaultValue("Player")]
        public String Name { get; set; }

        /// <summary>
        /// The game's difficulty level. In older versions of Terraria, this 
        /// was a simple boolean easy/hard.
        /// </summary>
        [DefaultValue((byte)0)]
        public byte Difficulty { get; set; }

        /// <summary>
        /// The player's hair style. Color set seperately.
        /// </summary>
        [DefaultValue(0)]
        public Int32 Hair { get; set; }

        /// <summary>
        /// A dye, if any, applied to the player's hair.
        /// </summary>
        [DefaultValue((byte)0)]
        public byte HairDye { get; set; }

        /// <summary>
        /// Flag called 'hide visual'. Pulled from Terraria source, not sure what it does.
        /// </summary>
        public BitsByte HideVisual { get; set; }

        /// <summary>
        /// Boolean flag. If true, the player is male; false otherwise.
        /// </summary>
        [DefaultValue(true)]
        public bool IsMale { get; set; }

        /// <summary>
        /// Player's current hit points.
        /// </summary>
        [DefaultValue(100)]
        public int CurrentLife { get; set; }

        /// <summary>
        /// Player's maximum hit points.
        /// </summary>
        [DefaultValue(100)]
        public int MaxLife { get; set; }

        /// <summary>
        /// Player's current mana.
        /// </summary>
        [DefaultValue(20)]
        public int CurrentMana { get; set; }

        /// <summary>
        /// Player's maximum mana.
        /// </summary>
        [DefaultValue(20)]
        public int MaxMana { get; set; }

        /// <summary>
        /// Player's (natural, un-dyed) hair color.
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color HairColor { get; set; }

        /// <summary>
        /// Player's skin color.
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color SkinColor { get; set; }

        /// <summary>
        /// Player's eye color.
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color EyeColor { get; set; }

        /// <summary>
        /// Player's shirt color.
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color ShirtColor { get; set; }

        /// <summary>
        /// Player's undershirt color.
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color UndershirtColor { get; set; }

        /// <summary>
        /// Player's pants color.
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color PantsColor { get; set; }

        /// <summary>
        /// Player's shoe color.
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color ShoeColor { get; set; }

        /// <summary>
        /// Array of Items that are located in the Player's armor slots.
        /// </summary>
        public Item[] Armor { get; private set; }

        /// <summary>
        /// Array of Items are that located in the Player's dye slots.
        /// </summary>
        public Item[] Dyes { get; private set; }

        /// <summary>
        /// Array if items that are located in the Player's inventory
        /// </summary>
        public Item[] Inventory { get; private set; }

        [DefaultValue(new Object[0])]
        public Item[] Bank1 { get; set; }

        [DefaultValue(new Object[0])]
        public Item[] Bank2 { get; set; }

        [DefaultValue(false)]
        public Boolean HotBarLocked { get; set; }

        [DefaultValue(new Object[0])]
        public Buff[] Buffs { get; set; }

        // spawn data -- apparently there's a limit of 200 worlds. who knew?
        public int[] SpawnX { get; private set; }
        public int[] SpawnY { get; private set; }
        public string[] SpawnWorldName { get; private set; }
        public int[] SpawnWorldId { get; private set; }

        [DefaultValue(0)]
        public Int32 AnglerQuests { get; set; }

        #endregion

        public Player()
        {
            // initialize item and buff arrays
            int release = VersionUtils.TERRARIA_CURRENT_RELEASE;
            Armor = new Item[VersionUtils.GetArmorSize(release)];
            Dyes = new Item[VersionUtils.GetDyeSize(release)];
            Inventory = new Item[VersionUtils.GetInventorySize(release)];

            // initialize spawn data arrays
            int size = VersionUtils.MAX_WORLDS;
            SpawnX = new int[size];
            SpawnY = new int[size];
            SpawnWorldName = new string[size];
            SpawnWorldId = new int[size];
        }

        public override string ToString()
        {
            string fmt = "Player ({0})<\n\t" +
                ":name => {1}\n\t" +
                ":inventory => {2}\n>";
            return string.Format(fmt, Release, Name, Inventory);

        }

        public override bool Equals(System.Object obj)
        {
            Player p = obj as Player;
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Equals(p);
        }

        public bool Equals(Player p)
        {
            if (p == null)
            {
                return false;
            }

            return Release == p.Release &&
                   IsBothNullOrEqual(Name, p.Name) &&
                   Difficulty == p.Difficulty &&
                   Hair == p.Hair &&
                   HairDye == p.HairDye &&
                // skip HideVisual, we stll don't know what it does
                   IsMale == p.IsMale &&
                   CurrentLife == p.CurrentLife &&
                   MaxLife == p.MaxLife &&
                   CurrentMana == p.CurrentMana &&
                   MaxMana == p.MaxMana &&
                   IsBothNullOrEqual(HairColor, p.HairColor) &&
                   IsBothNullOrEqual(SkinColor, p.SkinColor) &&
                   IsBothNullOrEqual(EyeColor, p.EyeColor) &&
                   IsBothNullOrEqual(ShirtColor, p.ShirtColor) &&
                   IsBothNullOrEqual(UndershirtColor, p.UndershirtColor) &&
                   IsBothNullOrEqual(PantsColor, p.PantsColor) &&
                   IsBothNullOrEqual(ShoeColor, p.ShoeColor) &&
                   IsArraysEqual(Armor, p.Armor) &&
                   IsArraysEqual(Dyes, p.Dyes) &&
                   IsArraysEqual(Inventory, p.Inventory) &&
                   IsArraysEqual(Bank1, p.Bank1) &&
                   IsArraysEqual(Bank2, p.Bank2) &&
                   HotBarLocked == p.HotBarLocked &&
                   IsArraysEqual(Buffs, p.Buffs) &&
                   IsArraysEqual(SpawnX, p.SpawnX) &&
                   IsArraysEqual(SpawnY, p.SpawnY) &&
                   IsArraysEqual(SpawnWorldId, p.SpawnWorldId) &&
                   IsArraysEqual(SpawnWorldName, p.SpawnWorldName) &&
                   AnglerQuests == p.AnglerQuests;
        }

        public override int GetHashCode()
        {
            // This is a really stupid way to do this, but I'm not actually a .NET developer...
            return base.GetHashCode() ^ Release.GetHashCode() ^ Name.GetHashCode() ^ Difficulty.GetHashCode() ^
                Hair.GetHashCode() ^ HairDye.GetHashCode() ^ HideVisual.GetHashCode() ^ IsMale.GetHashCode() ^ CurrentLife.GetHashCode() ^
                MaxLife.GetHashCode() ^ CurrentMana.GetHashCode() ^ MaxMana.GetHashCode() ^ HairColor.GetHashCode() ^
                SkinColor.GetHashCode() ^ EyeColor.GetHashCode() ^ ShirtColor.GetHashCode() ^ UndershirtColor.GetHashCode() ^
                PantsColor.GetHashCode() ^ ShoeColor.GetHashCode() ^ Armor.GetHashCode() ^ Dyes.GetHashCode() ^
                Inventory.GetHashCode() ^ Bank1.GetHashCode() ^ Bank2.GetHashCode() ^ HotBarLocked.GetHashCode() ^
                Buffs.GetHashCode() ^ SpawnX.GetHashCode() ^ SpawnY.GetHashCode() ^ SpawnWorldName.GetHashCode() ^
                SpawnWorldId.GetHashCode() ^ AnglerQuests.GetHashCode();
        }

        private static bool IsArraysEqual(int[] a, int[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            else if ((a == null ^ b == null) || (a.Length != b.Length))
            {
                return false;
            }
            else
            {
                return a.OrderBy(x => x).SequenceEqual(b.OrderBy(y => y));
            }
        }

        private static bool IsArraysEqual(Object[] a, Object[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            else if ((a == null ^ b == null) || (a.Length != b.Length))
            {
                return false;
            }
            else
            {
                return a.OrderBy(x => x).SequenceEqual(b.OrderBy(y => y));
            }
        }
       
        private static bool IsBothNullOrEqual(Item a, Item b)
        {
            return (a == null && b == null) ||
                   (a != null && b != null && a.Equals(b));
        }

        private static bool IsBothNullOrEqual(Color a, Color b)
        {
            return (a == null && b == null) ||
                   (a != null && b != null && a.Equals(b));
        }

        private static bool IsBothNullOrEqual(string a, string b)
        {
            return (a == null && b == null) ||
                (a != null && b != null && a.Equals(b));
        }
    }
}
