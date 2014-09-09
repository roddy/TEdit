using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using TEdit.Data.Util;
using System.Windows.Media;
using TEdit.Data.Types;

namespace TEdit.Data.IO
{
    class PlayerLoader
    {
        private static ILog logger = LogManager.GetLogger(typeof(PlayerLoader));

        public static Player Load(BinaryReader reader)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info("Attempting to load Player object from decrypted filestream.");
            }

            Player player = new Player();

            ParseRelease(player, reader);
            checkReleaseSupport(player.Release);

            ParseName(player, reader);
            ParseDifficulty(player, reader);
            ParseHair(player, reader);
            ParseHairDye(player, reader);
            ParseHideVisual(player, reader);
            ParseGender(player, reader);
            ParseHealth(player, reader);
            ParseMana(player, reader);
            ParseHairColor(player, reader);
            ParseSkinColor(player, reader);
            ParseEyeColor(player, reader);
            ParseShirtColor(player, reader);
            ParseUndershirtColor(player, reader);
            ParsePantsColor(player, reader);
            ParseShoesColor(player, reader);
            
            ParseArmor(player, reader);
            ParseDye(player, reader);
            ParseInventory(player, reader);
            ParseBank(player, reader);
            
            // we don't care about these for TEdit purposes, but if we don't parse them we don't be able to 
            // write a good output file at the end of the day
            ParseBuffs(player, reader);

            ParseSpawnPoints(player, reader);

            ParseHotBarLocked(player, reader);

            // We do kinda care about this one.
            ParseAnglerQuests(player, reader);

            return player;
        }

        private static void ParseSpawnPoints(Player player, BinaryReader reader)
        {
            for (int index = 0; index < VersionUtils.MAX_WORLDS; index++)
            {
                Int32 spawnPointX = reader.ReadInt32();
                if (spawnPointX == -1)
                {
                    if (logger.IsDebugEnabled)
                    {
                        logger.Debug("Parsed " + index + " world spawn points for player.");
                    }
                    break; // we've already parsed the last world
                }
                else
                {
                    player.SpawnX[index] = spawnPointX;
                    player.SpawnY[index] = reader.ReadInt32();
                    player.SpawnWorldId[index] = reader.ReadInt32();
                    player.SpawnWorldName[index] = reader.ReadString();
                }
            }
        }

        private static void ParseAnglerQuests(Player player, BinaryReader reader)
        {
            player.AnglerQuests = reader.ReadInt32();
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Completed angler quests: " + player.AnglerQuests);
            }
        }

        private static void ParseHotBarLocked(Player player, BinaryReader reader)
        {
            player.HotBarLocked = reader.ReadBoolean();
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed hotbar locked: " + player.HotBarLocked);
            }
        }

        private static void ParseBuffs(Player player, BinaryReader reader)
        {
            int release = player.Release;
            int num = VersionUtils.GetBuffCount(release);
            List<Buff> buffs = new List<Buff>();
            for (int index = 0; index < num; ++index)
            {
                Int32 type = reader.ReadInt32();
                Int32 time = reader.ReadInt32(); //time
                if (type == 0)
                {
                    --index;
                    --num;
                }
                else
                {
                    Buff buff = new Buff();
                    buff.Type = type;
                    buff.Time = time;
                    buffs.Add(buff);
                }
            }

            Buff[] b = new Buff[buffs.Count];
            int idx = 0;
            foreach (Buff buff in buffs) {
                b[idx] = buff;
                idx++;
            }
            player.Buffs = b;
        }

        private static void checkReleaseSupport(int version)
        {
            if (version < VersionUtils.MIN_SUPPORTED_RELEASE)
            {
                throw new NotSupportedException("PLR file built with release '" + version + "', but the minimum supported release is '" + VersionUtils.MIN_SUPPORTED_RELEASE + "'. Please load the PLR to a terraria world and save it, then try opening it in TEdit again.");
            }
        }

        private static void ParseBank(Player player, BinaryReader reader)
        {
            int size = VersionUtils.GetBankSize(player.Release);
            Item[] bank1 = new Item[size];
            Item[] bank2 = new Item[size];
            Item[][] banks = {bank1, bank2};
            foreach (Item[] bank in banks) {
                for (int index = 0; index < bank.Length; index++)
                {
                    Item item = new Item();
                    item.Id = reader.ReadInt32();
                    item.StackSize = reader.ReadInt32();
                    item.Prefix = (int)reader.ReadByte();
                }
            }
            player.Bank1 = bank1;
            player.Bank2 = bank2;
        }

        private static void ParseInventory(Player player, BinaryReader reader)
        {
            int size = VersionUtils.GetInventorySize(player.Release);
            Item[] inventory = new Item[size];
            for (int index = 0; index < inventory.Length; index++)
            {
                int id = reader.ReadInt32();
                Item item = new Item();
                if (id < VersionUtils.MAX_ITEM_ID)
                {
                    item.Id = id;
                    item.StackSize = reader.ReadInt32();
                    item.Prefix = (int)reader.ReadByte();
                }
                inventory[index] = item;
                
            }
            player.Inventory = inventory;
        }

        private static void ParseDye(Player player, BinaryReader reader)
        {
            int release = player.Release;
            if (VersionUtils.IsDyeSupported(release))
            {
                int size = 3;
                if (VersionUtils.IsExtendedDyeSupported(release))
                {
                    size = 8;
                }
                Item[] dyes = new Item[size];

                for (int index = 0; index < dyes.Length; index++)
                {
                    Item item = new Item();
                    item.Id = reader.ReadInt32();
                    item.StackSize = 1;
                    item.Prefix = (int)reader.ReadByte();
                    dyes[index] = item;
                    if (logger.IsDebugEnabled)
                    {
                        logger.Debug("Parsed dye: " + item);
                    }
                }
                player.Dyes = dyes;
            }
            else if (logger.IsDebugEnabled)
            {
                logger.Debug("Did not parse dyes because they are not enabled in release '" + release + "'.");
            }
        }

        private static void ParseArmor(Player player, BinaryReader reader)
        {
            int size = 11;
            if (VersionUtils.Is16PieceArmorSupported(player.Release))
            {
                size = 16;
            }
            Item[] armor = new Item[size];

            for (int index = 0; index < armor.Length; index++)
            {
                Item item = new Item();
                int id = reader.ReadInt32();
                if (id < VersionUtils.MAX_ITEM_ID)
                {
                    item.Id = id;
                    item.StackSize = 1;
                    item.Prefix = (int)reader.ReadByte();
                }
                armor[index] = item;
                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Parsed item to Armor slot: " + item);
                }
            }

            player.Armor = armor;
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's armor: " + armor.Length + " pieces.");
            }
        }

        /// <summary>
        /// Parses a color.
        /// </summary>
        /// <param name="reader">The source.</param>
        /// <returns>The color.</returns>
        private static Color ParseColor(BinaryReader reader)
        {
            Color color = new Color();
            color.R = reader.ReadByte();
            color.G = reader.ReadByte();
            color.B = reader.ReadByte();
            return color;
        }

        /// <summary>
        /// Parses the player's shoe color.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseShoesColor(Player player, BinaryReader reader)
        {
            player.ShoeColor = ParseColor(reader);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's shoe color: " + player.ShoeColor);
            }
        }

        /// <summary>
        /// Parses the player's pants color.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParsePantsColor(Player player, BinaryReader reader)
        {
            player.PantsColor = ParseColor(reader);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's pants color: " + player.PantsColor);
            }
        }

        /// <summary>
        /// Parses the player's Undershirt color.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseUndershirtColor(Player player, BinaryReader reader)
        {
            player.UndershirtColor = ParseColor(reader);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's undershirt color: " + player.UndershirtColor);
            }
        }

        /// <summary>
        /// Parses the player's shirt color.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseShirtColor(Player player, BinaryReader reader)
        {
            player.ShirtColor = ParseColor(reader);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's shirt color: " + player.ShirtColor);
            }
        }

        /// <summary>
        /// Parses the player's eye color.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseEyeColor(Player player, BinaryReader reader)
        {
            player.EyeColor = ParseColor(reader);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's eye color: " + player.EyeColor);
            }
        }

        /// <summary>
        /// Parses the player's skin color.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseSkinColor(Player player, BinaryReader reader)
        {
            player.SkinColor = ParseColor(reader);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's skin color: " + player.SkinColor);
            }
        }

        /// <summary>
        /// Parses the player's hair color.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseHairColor(Player player, BinaryReader reader)
        {
            player.HairColor = ParseColor(reader);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's hair color: " + player.HairColor);
            }
        }

        /// <summary>
        /// Parses the player's mana.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseMana(Player player, BinaryReader reader)
        {
            player.CurrentMana = reader.ReadInt32();
            player.MaxMana = reader.ReadInt32();

            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's mana: " + player.CurrentMana + "/" + player.MaxMana);
            }
        }

        /// <summary>
        /// Parses the player's health.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseHealth(Player player, BinaryReader reader)
        {
            player.CurrentLife = reader.ReadInt32();
            player.MaxLife = reader.ReadInt32();

            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's health: " + player.CurrentLife + "/" + player.MaxLife);
            }
        }

        /// <summary>
        /// Parses the player's gender.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseGender(Player player, BinaryReader reader)
        {
            player.IsMale = reader.ReadBoolean();
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player gender: " + (player.IsMale ? "Male" : "Female"));
            }
        }

        /// <summary>
        /// Parses the current state of the 'hide visual' flag, if supported.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseHideVisual(Player player, BinaryReader reader)
        {
            if (VersionUtils.IsHideVisualSupported(player.Release))
            {
                player.HideVisual = (BitsByte)reader.ReadByte();

                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Parsed HideVisual: " + player.HideVisual);
                }
            }
            else if (logger.IsDebugEnabled)
            {
                logger.Debug("'HideVisual' is not supported in release version '" + player.Release + "'. Parsing skipped.");
            }
        }

        /// <summary>
        /// Parses the player's active hair dye, if supported.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseHairDye(Player player, BinaryReader reader)
        {
            if (VersionUtils.IsHairDyeSupported(player.Release))
            {
                player.HairDye = reader.ReadByte();

                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Parsed hair dye: " + player.HairDye);
                }
            }
            else if (logger.IsDebugEnabled)
            {
                logger.Debug("Hair dye is not supported in this version of the PLR file: '" + player.Release + "'.");
            }
        }

        /// <summary>
        /// Parses a player's hair style.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseHair(Player player, BinaryReader reader)
        {
            player.Hair = reader.ReadInt32();
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's hair style: " + player.Hair);
            }
        }

        /// <summary>
        /// Parses the player's difficulty level. If the PLR file predates 'difficulty', then the player's
        /// difficulty is set to Softcore (0). 
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseDifficulty(Player player, BinaryReader reader)
        {
            int release = player.Release;
            player.Difficulty = reader.ReadByte();
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed difficulty: " + player.Difficulty);
            }
        }

        /// <summary>
        /// Parses the player's name.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseName(Player player, BinaryReader reader)
        {
            player.Name = reader.ReadString();
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player name: " + player.Name);
            }
        }

        /// <summary>
        /// Parses the release number of the PLR file. This allows us to know which release we're dealing with.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseRelease(Player player, BinaryReader reader)
        {
            player.Release = reader.ReadInt32();
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed release: " + player.Release);
            }
        }
    }
}
