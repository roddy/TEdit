using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TEdit.Data.Util;

namespace TEdit.Data.IO
{
    class PlayerUnloader
    {
        public static void unload(Player player, BinaryWriter writer)
        {
            writer.Write(VersionUtils.TERRARIA_CURRENT_RELEASE);
            writer.Write(player.Name);
            writer.Write(player.Difficulty);
            writer.Write(player.Hair);
            writer.Write(player.HairDye);
            writer.Write((byte)player.HideVisual);
            writer.Write(player.IsMale);
            writer.Write(player.CurrentLife);
            writer.Write(player.MaxLife);
            writer.Write(player.CurrentMana);
            writer.Write(player.MaxMana);
            WriteColor(writer, player.HairColor);
            WriteColor(writer, player.SkinColor);
            WriteColor(writer, player.EyeColor);
            WriteColor(writer, player.ShirtColor);
            WriteColor(writer, player.UndershirtColor);
            WriteColor(writer, player.PantsColor);
            WriteColor(writer, player.ShoeColor);

            WriteArmor(player, writer);
            WriteDyes(player, writer);
            WriteInventory(player, writer);
            WriteBank(player, writer);
            WriteBuffs(player, writer);
            WriteWorlds(player, writer);
            writer.Write(player.HotBarLocked);
            writer.Write(player.AnglerQuests);
            writer.Close();
        }

        private static void WriteBuffs(Player player, BinaryWriter writer)
        {
            for (int index = 0; index < player.Buffs.Length; ++index)
            {
                Buff buff = player.Buffs[index];
                if (buff == null)
                {
                    buff = new Buff();
                    buff.Type = 0;
                    buff.Time = 0;
                }
                Int32 type = buff.Type;
                Int32 time = buff.Time;
                if (ShouldNotSaveBuff(type))
                {
                    writer.Write(0);
                    writer.Write(0);
                }
                else
                {
                    writer.Write(type);
                    writer.Write(time);
                }
            }
        }

        private static List<Int32> buffNoSave = new List<Int32>() { 
            20, 22, 23, 24, 28, 30, 31, 34, 35, 37, 38, 39, 43, 44, 46, 47, 48, 58, 59, 60, 62, 63, 64, 67, 68, 69, 70, 
            72, 80, 87, 88, 89, 94, 95, 96, 97, 98, 99, 100, 103, 118, 138, 119, 120, 90, 125, 126, 128, 129, 130, 131, 
            132, 133, 134, 135, 137
        };

        private static bool ShouldNotSaveBuff(Int32 type)
        {
            return buffNoSave.IndexOf(type) != -1;
        }

        private static void WriteInventory(Player player, BinaryWriter writer)
        {
            for (int index = 0; index < 58; ++index)
            {
                Item inv;
                if (player.Inventory.Length <= index)
                {
                    inv = new Item();
                    inv.Id = 0;
                    inv.Prefix = (byte)0;
                    inv.StackSize = 1;
                }
                else
                {
                    inv = player.Inventory[index];
                }
                writer.Write(inv.Id);
                writer.Write(inv.StackSize);
                writer.Write(inv.Prefix);
            }
        }

        private static void WriteBank(Player player, BinaryWriter writer)
        {
            Item[][] banks = { player.Bank1, player.Bank2 };
            foreach (Item[] bank in banks)
            {
                for (int i = 0; i < bank.Length; i++)
                {
                    Item item = bank[i];
                    if (item == null)
                    {
                        item = new Item();
                        item.Id = 0;
                        item.StackSize = 1;
                        item.Prefix = (byte)0;
                    }
                    writer.Write(item.Id);
                    writer.Write(item.StackSize);
                    writer.Write(item.Prefix);
                }
            }
        }

        private static void WriteWorlds(Player player, BinaryWriter writer)
        {
            for (int index = 0; index < 200; ++index)
            {

                if (player.SpawnWorldName[index] == null)
                {
                    writer.Write(-1);
                    break;
                }
                else
                {
                    writer.Write(player.SpawnX[index]);
                    writer.Write(player.SpawnY[index]);
                    writer.Write(player.SpawnWorldId[index]);
                    writer.Write(player.SpawnWorldName[index]);
                }
            }
        }

        private static void WriteDyes(Player player, BinaryWriter writer)
        {
            for (int index = 0; index < 8; ++index)
            {
                Item dye;
                if (player.Dyes.Length <= index)
                {
                    dye = new Item();
                    dye.Id = 0;
                    dye.Prefix = (byte)0;
                }
                else
                {
                    dye = player.Dyes[index];
                }
                writer.Write(dye.Id);
                writer.Write(dye.Prefix);
            }
        }

        private static void WriteArmor(Player player, BinaryWriter writer)
        {
            for (int index = 0; index < 16; ++index)
            {
                Item armor;
                if (player.Armor.Length <= index)
                {
                    armor = new Item();
                    armor.Id = 0;
                    armor.Name = "";
                    armor.Prefix = (byte)0;
                }
                else
                {
                    armor = player.Armor[index];
                }
                writer.Write(armor.Id);
                writer.Write(armor.Prefix);
            }
        }

        private static void WriteColor(BinaryWriter writer, Color color)
        {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
        }
    }
}
