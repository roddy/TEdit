using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TEdit.Data.IO
{
    class PlrWriter
    {
        private static readonly String KEY = "h3y_gUyZ";

        private static ILog logger = LogManager.GetLogger(typeof(PlrWriter));
        public static void Save(Player player)
        {
            if (player == null)
            {
                logger.Error("Failed to save Player because no Player exists.");
                return;
            }

            string name = player.Name;
            if (name == null)
            {
                logger.Warn("Player has no name. Using name Anonymous as a fallback. Player cannot be guaranteed to have been saved properly.");
                name = "Anonymous";
            }
            string tempFile = name + ".dat";
            using(FileStream stream = new FileStream(tempFile, FileMode.Open))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                try
                {
                    PlayerUnloader.unload(player, writer);
                }
                finally
                {
                    stream.Close();
                    writer.Close();
                }

            }

            string targetFile = name + ".plr";
            Encrypt(tempFile, targetFile);
            File.Delete(tempFile);
        }

        private static void Encrypt(string inputFile, string outputFile)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(KEY);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();

            
            using(FileStream outputStream = new FileStream(outputFile, FileMode.Create))
            using(CryptoStream cryptoStream = new CryptoStream((Stream)outputStream, rijndaelManaged.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write))
            using(FileStream inputStream = new FileStream(inputFile, FileMode.Open)) 
            {
                int num;
                while ((num = inputStream.ReadByte()) != -1)
                {
                    cryptoStream.WriteByte((byte)num);
                }
            }
        }
    }
}
