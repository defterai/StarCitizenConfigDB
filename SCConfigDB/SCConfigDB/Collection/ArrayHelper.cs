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
    }
}
