using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AgentHub.Entities.Utilities
{
    public class Encryption
    {
        private const string Key = "BCCD585F-E74F-4C82-B345-9AED60758DB8";

        public static string Encrypt(string dataToEncrypt, bool useHashing = true)
        {
            byte[] keyArray;
            var toEncryptArray = Encoding.UTF8.GetBytes(dataToEncrypt);

            if (useHashing)
            {
                var hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(Key));
                hashmd5.Clear();
            }
            else
                keyArray = Encoding.UTF8.GetBytes(Key);

            var tdes = new TripleDESCryptoServiceProvider { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

            var cTransform = tdes.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string encryptedData, bool useHashing = true)
        {
            byte[] keyArray;
            var toEncryptArray = Convert.FromBase64String(encryptedData);

            if (useHashing)
            {
                var hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(Key));
                hashmd5.Clear();
            }
            else
            {
                keyArray = Encoding.UTF8.GetBytes(Key);
            }

            var tdes = new TripleDESCryptoServiceProvider { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

            var cTransform = tdes.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return Encoding.UTF8.GetString(resultArray);
        }

        public static string Base64ForUrlEncode(string value)
        {
            var encbuff = Encoding.UTF8.GetBytes(value);
            return HttpServerUtility.UrlTokenEncode(encbuff);
        }

        public static string Base64ForUrlDecode(string value)
        {
            var decbuff = HttpServerUtility.UrlTokenDecode(value);
            return decbuff != null ? Encoding.UTF8.GetString(decbuff) : null;
        }
    }

}
