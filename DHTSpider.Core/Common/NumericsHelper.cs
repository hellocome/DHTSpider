using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.Core.Common
{
    public static class NumericsHelper
    {
        public static uint CombineHash(uint u1, uint u2)
        {
            return ((u1 << 7) | (u1 >> 25)) ^ u2;
        }

        public static int CombineHash(int u1, int u2)
        {
            return (int)CombineHash((uint)u1, (uint)u2);
        }
    }
}
