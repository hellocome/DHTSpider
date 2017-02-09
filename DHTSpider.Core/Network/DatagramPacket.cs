using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DHTSpider.Core.Network
{
    // We can do some optimize with liner buff
    public class DatagramPacket
    {
        internal byte[] m_Buffer;
        internal int m_Count;
        internal int m_Offset;

        public void SetBuffer(byte[] buffer, int offset, int length)
        {
            SetBufferInternal(buffer, offset, length);
        }

        private void SetBufferInternal(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                // Clear out existing buffer.
                m_Buffer = null;
                m_Offset = 0;
                m_Count = 0;

            }
            else
            {
                // Offset and count can't be negative and the 
                // combination must be in bounds of the array.
                if (offset < 0 || offset > buffer.Length)
                {
                    throw new ArgumentOutOfRangeException("offset");
                }
                if (count < 0 || count > (buffer.Length - offset))
                {
                    throw new ArgumentOutOfRangeException("count");
                }

                m_Buffer = buffer;
                m_Offset = offset;
                m_Count = count;
            }
        }
    }
}
