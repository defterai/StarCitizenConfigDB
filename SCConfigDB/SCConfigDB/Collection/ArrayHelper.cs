using System;

namespace Defter.StarCitizen.ConfigDB.Collection
{
    public static class ArrayHelper
    {
        public static T[] Clone<T>(T[] array) where T : class
        {
            T[] result = new T[array.Length];
            Array.Copy(array, result, array.Length);
            return result;
        }

        public static T[] SubArray<T>(this T[] data, int index)
        {
            int length = data.Length - index;
            T[] result = new T[length];
            if (length > 0)
            {
                Array.Copy(data, index, result, 0, length);
            }
            return result;
        }
    }
}
