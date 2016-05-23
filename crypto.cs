using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Windows.Forms;

//Based on code project article 
//http://www.codeproject.com/KB/security/encryption_decryption.aspx
//authored by fangfrank@hotmail.com



namespace FileEncrypter
{
    public class Crypto
    {

        private SymmetricAlgorithm CryptoService;


        public Crypto()
        {
            CryptoService = new RijndaelManaged();
            CryptoService.BlockSize = 256;
            CryptoService.Padding = PaddingMode.ISO10126;            
        }

        public string GenerateKey()
        {
            CryptoService.GenerateKey();
            return ASCIIEncoding.ASCII.GetString(CryptoService.Key);
        }

        private byte[] PrepKeyIV(string Key)
        {          
            int blockSize = CryptoService.BlockSize;                       
            // key sizes are in bits
            if (Key.Length * 8 > blockSize)
            {
                //truncate key to block size since password and IV are the same
                return ASCIIEncoding.ASCII.GetBytes(Key.Remove((blockSize / 8)));
            }
            else if (Key.Length * 8 < blockSize)
            {
                //pad key to block size since password and IV are the same
                return ASCIIEncoding.ASCII.GetBytes(Key.PadRight(blockSize / 8, ' '));
            }
            else
            {
                //if the key is a proper key, return it untouched.
                return ASCIIEncoding.ASCII.GetBytes(Key);
            }

        }

        public string Encrypting(string sInputFilename, string sOutputFilename, string sKey)
        {

            FileStream fsInput = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
            FileStream fsEncrypted = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write);

            byte[] bytKey = PrepKeyIV(sKey);

            //IV and BlockSize must be the same size
            CryptoService.Key = bytKey;
            CryptoService.IV = bytKey;
            
            ICryptoTransform encrypto = CryptoService.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted, encrypto, CryptoStreamMode.Write);

            // write out encrypted content into MemoryStream
            
            byte[] bytearrayinput = new byte[fsInput.Length];
            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Close();
            fsInput.Close();
            fsEncrypted.Close();

            return sKey;
        }


        public bool Decrypting(string sInputFilename, string sOutputFilename, string sKey)
        {

            byte[] bytKey = PrepKeyIV(sKey);
            // set the private key
            CryptoService.Key = bytKey;
            CryptoService.IV = bytKey;

            // create a MemoryStream with the input
            FileStream fsInput = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
            FileStream fsDecrypted = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write);

            // create a Decryptor from the Provider Service instance
            ICryptoTransform decrypto = CryptoService.CreateDecryptor();

            // create Crypto Stream that transforms a stream using the decryption
            CryptoStream cryptostreamDecr = new CryptoStream(fsDecrypted, decrypto, CryptoStreamMode.Write);

            try
            {
                //Print the contents of the decrypted file.

                /*
                 * This method is not binary safe and will muck up the encoding :-(
                 * 
                StreamWriter fsDecrypted = new StreamWriter(sOutputFilename);
                fsDecrypted.Write(new StreamReader(cryptostreamDecr).ReadToEnd());
                fsDecrypted.Flush();
                fsDecrypted.Close();
                */

                byte[] bytearrayinput = new byte[fsInput.Length];
                fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
                cryptostreamDecr.Write(bytearrayinput, 0, bytearrayinput.Length);
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show("Unable to Decrypt file, file maybe damaged, or passphrase is incorrect, error details:" + Environment.NewLine + err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            finally
            {
                cryptostreamDecr.Close();
                fsInput.Close();
                fsDecrypted.Close();
            }

        }
    }
}