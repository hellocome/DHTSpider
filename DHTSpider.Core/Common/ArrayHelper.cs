using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.Core.Common
{
    public static class ArrayHelper
    {
        public static T[] SubArray<T>(this T[] array, int index, int length)
        {
            if (array == null || index >= array.Length || length + index > array.Length)
            {
                return null;
            }

            T[] newArray = new T[length];

            Array.Copy(array, 0, newArray, index, length);

            return newArray;
        }
    }
}
