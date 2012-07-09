using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TeamMgmtCal.Core.Helpers
{
    public class EncryptionHelpers
    {
        /// <summary>
        /// Return A One Way Hash
        /// </summary>
        public static string HashPassword(string PlainText)
        {
            byte[] _data = ASCIIEncoding.ASCII.GetBytes(PlainText);
            byte[] _hash = new MD5CryptoServiceProvider().ComputeHash(_data);

            StringBuilder _sbHashedPass = new StringBuilder();
            foreach (byte b in _hash)
                _sbHashedPass.Append(b.ToString("X2"));

            return _sbHashedPass.ToString();
        }

        //Encryption Settings
        private const string _passPhrase = "M@iN73chS3cur3EncryP7";
        private const string _saltValue = "kbyugit7896t5876bjoihin";
        private const string _hashAlgorithm = "SHA1";
        private const int _passwordIterations = 2;
        private const string _initVector = "@1B2c3D4e5F6g7H8";
        private const int _keySize = 256;

        /// <summary>
        /// Encrypts String using Encryption Settings
        /// </summary>
        public static string Encrypt(string _plainText)
        {
            //Convert string values into byte arrays needed to perform encryption
            byte[] _plainTextBytes = Encoding.ASCII.GetBytes(_plainText);
            byte[] _initVectorBytes = Encoding.ASCII.GetBytes(_initVector);
            byte[] _saltBytes = Encoding.ASCII.GetBytes(_saltValue);

            //Derive a password from supplied values
            PasswordDeriveBytes _password = new PasswordDeriveBytes(
                _passPhrase, _saltBytes, _hashAlgorithm, _passwordIterations);

            //Generate pseudo-random bytes for the encryption key
            byte[] _keyBytes = _password.GetBytes(_keySize / 8);

            RijndaelManaged _symmetricKey = new RijndaelManaged();

            //set encryption mode to Cipher Block Chaining
            _symmetricKey.Mode = CipherMode.CBC;

            //Generate encryptor
            ICryptoTransform _encryptor = _symmetricKey.CreateEncryptor(_keyBytes, _initVectorBytes);

            //Create Memory Stream to handle the encrytion
            MemoryStream _memoryStream = new MemoryStream();

            // Define cryptographic stream 
            CryptoStream _cryptoStream = new CryptoStream(_memoryStream, _encryptor, CryptoStreamMode.Write);

            //Start Encryption
            _cryptoStream.Write(_plainTextBytes, 0, _plainTextBytes.Length);
            _cryptoStream.FlushFinalBlock();

            //Drop memory stream to byte array
            byte[] _cypherTextBytes = _memoryStream.ToArray();

            //Close Streams
            _memoryStream.Close();
            _cryptoStream.Close();

            //Convert output to string
            string _cypherText = Convert.ToBase64String(_cypherTextBytes);
            return _cypherText;
        }

        /// <summary>
        /// Decrypts CipherText using Encryption Settings
        /// </summary>
        public static string DeCrypt(string _cipherText)
        {
            try
            {
                //Convert string values into byte arrays needed to perform decryption
                byte[] _cipherTextBytes = Convert.FromBase64String(_cipherText);

                byte[] _initVectorBytes = Encoding.ASCII.GetBytes(_initVector);
                byte[] _saltBytes = Encoding.ASCII.GetBytes(_saltValue);

                //Derive a password from supplied values
                PasswordDeriveBytes _password = new PasswordDeriveBytes(
                    _passPhrase, _saltBytes, _hashAlgorithm, _passwordIterations);

                //Generate pseudo-random bytes for the encryption key
                byte[] _keyBytes = _password.GetBytes(_keySize / 8);

                RijndaelManaged _symmetricKey = new RijndaelManaged();

                //set encryption mode to Cipher Block Chaining
                _symmetricKey.Mode = CipherMode.CBC;

                //Generate decryptor
                ICryptoTransform _decryptor = _symmetricKey.CreateDecryptor(_keyBytes, _initVectorBytes);

                //Create Memory Stream to handle the decrytion
                MemoryStream _memoryStream = new MemoryStream(_cipherTextBytes);

                // Define cryptographic stream 
                CryptoStream _cryptoStream = new CryptoStream(_memoryStream, _decryptor, CryptoStreamMode.Read);

                //create byte array to hold decrypted stream
                byte[] _plaintextBytes = new byte[_cipherTextBytes.Length];

                // Start decrypting.
                int _decryptedByteCount = _cryptoStream.Read(_plaintextBytes, 0, _plaintextBytes.Length);

                //Close Streams
                _memoryStream.Close();
                _cryptoStream.Close();

                // Convert decrypted data into a string. 
                string _plainText = Encoding.UTF8.GetString(_plaintextBytes, 0, _decryptedByteCount);
                return _plainText;
            }
            catch (Exception Ex)
            {
                return "there was an error decrypting the requested data.  " + Ex.Message;
            }
        }


    }
}
