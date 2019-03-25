using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace AgentHub.Entities.Utilities
{
    public class RSAEncryption
    {
        private const string PrivateKey = "<RSAKeyValue><Modulus>r8NFTgO+1p2RAuAo1l0hNKFivEbcxrXxphsTpKDxi6naCd6w5f965VJYRd43M/Hr</Modulus><Exponent>AQAB</Exponent><P>50y2qBZJVvNB45g4/wrdwChu4yBe1Twd</P><Q>wohJMYAoCMPCNM/weV61U9ba/TII1ben</Q><DP>n+bl48A8hXL4WxpyVMczVFPfjP9k8C91</DP><DQ>cOMDjn0vPj7TYTBV/SiPkzJ4bDvv0o27</DQ><InverseQ>T32sblOiRZI16rrLf5NZyRI1s4V6M/i/</InverseQ><D>S9zNyhmFwl5qG8Ki/btevLe3xSoU2tPmjI22t1mF7vI2c61EUQF1+HyP5zaMUkXZ</D></RSAKeyValue>";

        private const string PublicKey = "<RSAKeyValue><Modulus>r8NFTgO+1p2RAuAo1l0hNKFivEbcxrXxphsTpKDxi6naCd6w5f965VJYRd43M/Hr</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private static readonly UnicodeEncoding Encoder = new UnicodeEncoding();

        public static string GeneratePublicKey()
        {
            using (var csp = new RSACryptoServiceProvider(2048))
            {
                return csp.ToXmlString(false);
            }
        }

        public static string GeneratePrivateKey()
        {
            using (var csp = new RSACryptoServiceProvider(2048))
            {
                return csp.ToXmlString(true);
            }
        }

        public static string GenerateAccessToken(string applicationKey, string serviceKey)
        {
            return Encrypt(string.Format("{0}:{1}", applicationKey, serviceKey));
        }

        public static string Encrypt(string inputString, int dwKeySize = 1024)
        {
            try
            {
                using (var provider = new RSACryptoServiceProvider(dwKeySize))
                {
                    provider.FromXmlString(PublicKey);
                    var keySize = dwKeySize/8;
                    var bytes = Encoding.UTF32.GetBytes(inputString);
                    var maxLength = keySize - 42;
                    var dataLength = bytes.Length;
                    var iterations = dataLength/maxLength;
                    var stringBuilder = new StringBuilder();
                    for (var i = 0; i <= iterations; i++)
                    {
                        var tempBytes = new byte[(dataLength - maxLength*i > maxLength) ? maxLength : dataLength - maxLength*i];
                        Buffer.BlockCopy(bytes, maxLength*i, tempBytes, 0, tempBytes.Length);
                        var encryptedBytes = provider.Encrypt(tempBytes, true);
                        // Be aware the RSACryptoServiceProvider reverses the order of 
                        // encrypted bytes. It does this after encryption and before 
                        // decryption. If you do not require compatibility with Microsoft 
                        // Cryptographic API (CAPI) and/or other vendors. Comment out the 
                        // next line and the corresponding one in the DecryptString function.
                        Array.Reverse(encryptedBytes);
                        // Why convert to base 64?
                        // Because it is the largest power-of-two base printable using only 
                        // ASCII characters
                        stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
                    }
                    return stringBuilder.ToString();
                }
            }
            catch (Exception exception)
            {
                // TODO: Exception
                return string.Empty;
            }
        }

        public static string Decrypt(string inputString, int dwKeySize = 1024)
        {
            try
            {
                using (var rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize))
                {
                    rsaCryptoServiceProvider.FromXmlString(PrivateKey);
                    var base64BlockSize = ((dwKeySize/8)%3 != 0)
                        ? (((dwKeySize/8)/3)*4) + 4
                        : ((dwKeySize/8)/3)*4;
                    var iterations = inputString.Length/base64BlockSize;
                    var arrayList = new ArrayList();
                    for (var i = 0; i < iterations; i++)
                    {
                        var encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize*i, base64BlockSize));
                        // Be aware the RSACryptoServiceProvider reverses the order of 
                        // encrypted bytes after encryption and before decryption.
                        // If you do not require compatibility with Microsoft Cryptographic 
                        // API (CAPI) and/or other vendors.
                        // Comment out the next line and the corresponding one in the 
                        // EncryptString function.
                        Array.Reverse(encryptedBytes);
                        arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
                    }
                    return Encoding.UTF32.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
                }
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
                return string.Empty;
            }
        }
    }

}
