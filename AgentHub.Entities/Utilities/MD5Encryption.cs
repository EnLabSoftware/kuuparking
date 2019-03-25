using System;
using System.Security.Cryptography;
using System.Text;

namespace AgentHub.Entities.Utilities
{
    public class MD5Encryption
    {
        private const string Key = "6EE0F741-6117-4BC0-A8D1-F20A126C21CD";

        public static string Encrypt(string dataToEncrypt)
        {
            try
            {
                var toEncryptArray = Encoding.UTF8.GetBytes(dataToEncrypt);

                var hashmd5 = new MD5CryptoServiceProvider();
                var keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(Key));
                hashmd5.Clear();

                var tdes = new TripleDESCryptoServiceProvider { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

                var cTransform = tdes.CreateEncryptor();
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
                return string.Empty;
            }
        }

        public static string Decrypt(string encryptedData)
        {
            try
            {
                var toEncryptArray = Convert.FromBase64String(encryptedData);

                var hashmd5 = new MD5CryptoServiceProvider();
                var keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(Key));
                hashmd5.Clear();

                var tdes = new TripleDESCryptoServiceProvider { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

                var cTransform = tdes.CreateDecryptor();
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();

                return Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
                return string.Empty;
            }
        }

    }
}
