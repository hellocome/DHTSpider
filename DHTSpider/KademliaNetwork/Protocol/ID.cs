using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using DHTSpider.Core.Encoding;
using DHTSpider.Core.Logging;
using DHTSpider.Core.Security;
using DHTSpider.Core.Common;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol
{
    public class ID
    {
        public static readonly int ID_LENGTH = 160;
        public static readonly int ID_LENGTH_BYTE = ID_LENGTH / 8;

        private byte[] idBytes;
        
        public ID(string data)
        {
            idBytes = DataEncoding.StringToByte(data);

            if (idBytes.Length != ID_LENGTH_BYTE)
            {
                throw new ArgumentException("Specified Data need to be " + ID_LENGTH_BYTE + " characters long.");
            }
        }

        public ID()
        {
            idBytes = CryptoUtility.GenerateRandomByteArray(ID_LENGTH_BYTE);
        }

        public ID(byte[] data)
        {
            if (data == null || data.Length != ID_LENGTH_BYTE)
            {
                throw new ArgumentException("Specified Data need to be " + ID_LENGTH_BYTE + " characters long.");
            }

            idBytes = data;
        }

        private void SetBigInteger()
        {
            byte[] Reversed = new byte[idBytes.Length];

            for (int i = 0; i < idBytes.Length; i++)
            {
                Reversed[i] = idBytes[idBytes.Length - 1 - i];
            }

            IDBigInteger = new BigInteger(Reversed);
        }

        #region Operators etc

        public override string ToString()
        {
            return DataEncoding.ByteToString(idBytes);
        }

        public BigInteger IDBigInteger
        {
            get;
            private set;
        }

        public BigInteger AsDistance()
        {
            return BigInteger.Abs(IDBigInteger);
        }

        public byte[] IdBytes
        {
            get
            {
                return idBytes;
            }
        }

        public string GetHexString()
        {
            return DataEncoding.BytesToHexString(idBytes);
        }

        public byte this[int index]
        {
            get
            {
                return idBytes[index];
            }
        }

        public bool SameID(ID objId)
        {
            if (objId != null)
            {
                for (int i = 0; i < ID_LENGTH_BYTE; i++)
                {
                    if (objId[i] != this[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static ID operator^(ID id1, ID id2)
        {
            if(id1 == null || id2 == null)
            {
                throw new NullReferenceException("id1 = " + id1 + "  id2=" + id2 + ", can't be null");
            }

            byte[] xorRes = new byte[ID_LENGTH_BYTE];
            for (int i = 0; i < ID_LENGTH_BYTE; i++)
            {
                xorRes[i] = (byte)(id1[i] ^ id2[i]);
            }

            return new ID(xorRes);
        }

        public static bool operator ==(ID id1, ID id2)
        {
            if(ReferenceEquals(id1, id2) == true)
            {
                return true;
            }
            else if (id1 == null || id2 == null)
            {
                return false;
            }

            return id1.IDBigInteger == id2.IDBigInteger;
        }

        public static bool operator !=(ID id1, ID id2)
        {
            return !(id1.IDBigInteger == id2.IDBigInteger);
        }

        public int DifferingBit(ID another)
        {
            ID differingBits = this ^ another;

            if (differingBits.IDBigInteger.IsZero)
            {
                return -1;
            }
            else
            {
                int differAt = 8 * ID_LENGTH - 1;

                // Subtract 8 for every zero byte from the right
                int i = ID_LENGTH - 1;
                while (i >= 0 && differingBits.IdBytes[i] == 0)
                {
                    differAt -= 8;
                    i--;
                }

                // Subtract 1 for every zero bit from the right
                int j = 0;
                // 1 << j = pow(2, j)
                while (j < 8 && (differingBits.IdBytes[i] & (1 << j)) == 0)
                {
                    j++;
                    differAt--;
                }

                return differAt;
            }
        }

        public override bool Equals(object obj)
        {
            ID objId = obj as ID;

            if (objId != null)
            {
                return this == objId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < ID_LENGTH; i++)
            {
                unchecked
                {
                    hash *= 31;
                }

                hash ^= idBytes[i];
            }
            return hash;
        }

        #endregion

        #region Methold
        /*
            （1）节点和它本身之间的异或距离是0
            （2）异或距离是对称的：即从A到B的异或距离与从B到A的异或距离是等同的
            （3）异或距离符合三角形不等式：给定三个顶点A B C，假如AC之间的异或距离最大,那么AC之间的异或距离必小于或等于AB异或距离和BC异或距离之和.
             */

        public BigInteger GetDistance(ID id)
        {
            return BigInteger.Abs((this ^ id).IDBigInteger);
        }

        #endregion


        public static bool Validate(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return false;
            }

            byte[] byteID = DataEncoding.StringToByte(id);

            if (byteID == null && byteID.Length != ID_LENGTH_BYTE)
            {
                return false;
            }

            return true;
        }

        public static bool Validate(byte[] byteID)
        {
            if (byteID == null && byteID.Length != ID_LENGTH_BYTE)
            {
                return false;
            }

            return true;
        }
    }
}
