using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TEdit.Data.IO
{
    class PlrReader
    {
        private static readonly String KEY = "h3y_gUyZ";

        public static Player Load(String filepath)
        {
            string tempFile = DecryptPlr(filepath);
            Player player = ConvertToPlayer(tempFile);
            File.Delete(tempFile);
            return player;
        }

        private static Player ConvertToPlayer(string tempFile)
        {
            Player player = new Player();
            using (FileStream stream = new FileStream(tempFile, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                try
                {
                    player = PlayerLoader.Load(reader);
                }
                finally
                {
                    stream.Close();
                    reader.Close();
                }
            }
            return player;
        }

        private static string DecryptPlr(String filepath)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(KEY);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();

            string tempFile = "decrypted.dat";

            using (FileStream input = new FileStream(filepath, FileMode.Open))
            using (CryptoStream cryptoStream = new CryptoStream((Stream)input, rijndaelManaged.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read))
            using (FileStream output = new FileStream(tempFile, FileMode.Create))
            {
                int num;
                while ((num = cryptoStream.ReadByte()) != -1)
                {
                    output.WriteByte((byte)num);
                }                
            }
            return tempFile;
        }
    }
}
