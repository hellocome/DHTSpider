using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DHTSpider.Core.Security
{
    public static class CryptoUtility
    {
        public static RandomNumberGenerator RandomGenerator = new RNGCryptoServiceProvider();

        public static byte[] GenerateRandomByteArray(int length)
        {
            byte[] byteArray = new byte[length];

            RandomGenerator.GetBytes(byteArray);

            return byteArray;
        }

        public static T[] GenerateArrayFillWith<T>(T value, int length)
        {
            return Enumerable.Repeat<T>(value, length).ToArray();
        }
    }
}
