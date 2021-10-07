using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;

namespace CustomerManager.Utilities
{
    public class HashUtility
    {
        private const int DefaultSaltLength = 32;

        public static byte[] ComputeHashedString(string rawdata)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawdata));
                return bytes;
            }
        }
        public static byte[] GenerateSalt()
        {
            return GenerateSalt(DefaultSaltLength);
        }

        public static byte[] GenerateSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
    }
}
