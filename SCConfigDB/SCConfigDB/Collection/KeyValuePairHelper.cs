using System.Collections.Generic;
using System.Linq;

namespace Defter.StarCitizen.ConfigDB.Collection
{
    public static class KeyValuePairHelper
    {
        public static IEnumerable<KeyValuePair<TKey, TValue>> ToNonNullableKeyValues<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue?>> input) where TValue : class
        {
            return from kvp in input
                   where kvp.Value != null
                   select new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value);
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> ToNonNullableKeyValues<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue?>> input) where TValue : struct
        {
            return from kvp in input
                   where kvp.Value.HasValue
#pragma warning disable CS8629 // Nullable value type may be null.
                   select new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value.Value);
#pragma warning restore CS8629 // Nullable value type may be null.
        }
    }
}
