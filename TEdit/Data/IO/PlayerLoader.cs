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
            CheckReleaseSupport(player.Release);

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
            int num = 22;
            for (int i = 0; i < num; i++)
            {
                Int32 type = reader.ReadInt32();
                Int32 time = reader.ReadInt32(); //time
                if (type == 0)
                {
                    --i;
                    --num;

                }
                else
                {
                    player.Buffs[i] = new Buff();
                    player.Buffs[i].Time = time;
                    player.Buffs[i].Type = type;
                }
            }
        }

        private static void CheckReleaseSupport(int version)
        {
            if (version != VersionUtils.TERRARIA_CURRENT_RELEASE)
            {
                throw new NotSupportedException("PLR file built with release '" + version + "', but the minimum supported release is '" + VersionUtils.MIN_SUPPORTED_RELEASE + "'. Please load the PLR to a terraria world and save it, then try opening it in TEdit again.");
            }
        }

        private static void ParseBank(Player player, BinaryReader reader)
        {
            int size = 40;
            Item[] bank1 = player.Bank1;
            Item[] bank2 = player.Bank2;
            Item[][] banks = {bank1, bank2};
            foreach (Item[] bank in banks) {
                int index = 0;
                for (; index < size; index++)
                {
                    Item item = new Item();
                    item.Id = reader.ReadInt32();
                    item.StackSize = reader.ReadInt32();
                    item.Prefix = reader.ReadByte();
                    bank[index] = item;
                }
            }
        }

        private static void ParseInventory(Player player, BinaryReader reader)
        {
            int size = 58;
            for (int index = 0; index < size; index++)
            {
                int id = reader.ReadInt32();
                Item item = new Item();
                if (id < VersionUtils.MAX_ITEM_ID)
                {
                    item.Id = id;
                    item.StackSize = reader.ReadInt32();
                    item.Prefix = reader.ReadByte();
                }
                player.Inventory[index] = item;
            }
        }

        private static void ParseDye(Player player, BinaryReader reader)
        {
            for (int index = 0; index < 8; index++)
            {
                Item dye = ParseDye(reader);
                player.Dyes[index] = dye;
                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Parsed dye: " + dye);
                }
            }
        }

        /// <summary>
        /// Parse a single dye from the input stream
        /// </summary>
        /// <param name="reader">The input stream</param>
        /// <returns>A single dye</returns>
        private static Item ParseDye(BinaryReader reader)
        {
            Item item = new Item();
            item.Id = reader.ReadInt32();
            item.StackSize = 1;
            item.Prefix = reader.ReadByte();
            return item;
        }

        private static void ParseArmor(Player player, BinaryReader reader)
        {
            int size = 16;
            for (int index = 0; index < size && index < player.Armor.Length; index++)
            {
                Item item = new Item();
                int id = reader.ReadInt32();
                byte prefix = reader.ReadByte();
                if (id < VersionUtils.MAX_ITEM_ID)
                {
                    item.Id = id;
                    item.StackSize = 1;
                    item.Prefix = prefix;
                }
                player.Armor[index] = item;
                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Parsed item to Armor slot: " + item);
                }
            }
            for (int index = size; index < player.Armor.Length; index++)
            {
                player.Armor[index] = new Item();
            }

            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed player's armor: " + player.Armor.Length + " pieces.");
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
            player.HideVisual = (BitsByte)reader.ReadByte();
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed HideVisual: " + player.HideVisual);
            }
        }

        /// <summary>
        /// Parses the player's active hair dye, if supported.
        /// </summary>
        /// <param name="player">The player object to update</param>
        /// <param name="reader">The binary input stream we're parsing</param>
        private static void ParseHairDye(Player player, BinaryReader reader)
        {
            player.HairDye = reader.ReadByte();
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parsed hair dye: " + player.HairDye);
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
