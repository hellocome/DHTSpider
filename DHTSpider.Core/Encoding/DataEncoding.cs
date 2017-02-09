using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.Core.Encoding
{
    public static class DataEncoding
    {
        public static byte[] StringToByte(string str)
        {
            return System.Text.Encoding.ASCII.GetBytes(str);
        }

        public static string ByteToString(byte[] str)
        {
            return System.Text.Encoding.ASCII.GetString(str);
        }

        public static string BytesToHexString(byte[] data)
        {
            try
            {
                string hexString = BitConverter.ToString(data);

                hexString = hexString.Replace("-", "");
                return hexString;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static byte[] HexStringToByte(string data)
        {
            try
            {
                return Enumerable.Range(0, data.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(data.Substring(x, 2), 16))
                         .ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
