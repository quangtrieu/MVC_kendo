using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BCS.Framework.Security.FileEncrypts
{
    public class FileCrypt
    {
        ///<summary>
        /// Encrypts a file using Rijndael algorithm.
        ///</summary>
        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>
        private void EncryptFile(string inputFile, string outputFile, string enCryptKey)
        {
            try
            {
                string password = enCryptKey;
                var UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                var fsCrypt = new FileStream(cryptFile, FileMode.Create);

                var RMCrypto = new RijndaelManaged();

                var cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                var fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch
            {
            }
        }
        ///<summary>
        /// Steve Lydford - 12/05/2008.
        ///
        /// Decrypts a file using Rijndael algorithm.
        ///</summary>
        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>
        private void DecryptFile(string inputFile, string outputFile, string enCryptKey)
        {
            {
                string password = enCryptKey; // Your Key Here

                var UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                var fsCrypt = new FileStream(inputFile, FileMode.Open);

                var RMCrypto = new RijndaelManaged();

                var cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                var fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();
            }
        }
    }
}
