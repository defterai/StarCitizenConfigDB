using System.Collections.Generic;
using System.Collections.Specialized;

namespace Defter.StarCitizen.ConfigDB.Collection
{
    public interface IOrderedDictionary<TKey, TValue> : IOrderedDictionary, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        new int Add(TKey key, TValue value);
        void Insert(int index, TKey key, TValue value);

        new TValue this[int index]
        {
            get;
            set;
        }
    }
}
