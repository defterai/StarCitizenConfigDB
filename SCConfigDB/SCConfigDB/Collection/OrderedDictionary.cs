using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Defter.StarCitizen.ConfigDB.Collection
{
    public class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>
    {
        private const int DefaultInitialCapacity = 0;

        private static readonly string _keyTypeName = typeof(TKey).FullName;
        private static readonly string _valueTypeName = typeof(TValue).FullName;
        private static readonly bool _valueTypeIsReferenceType = !typeof(ValueType).IsAssignableFrom(typeof(TValue));

        private Dictionary<TKey, TValue>? _dictionary;
        private List<KeyValuePair<TKey, TValue>>? _list;
        private readonly IEqualityComparer<TKey>? _comparer;
        private object? _syncRoot;
        private readonly int _initialCapacity;

        public static IReadOnlyDictionary<TKey, TValue> Empty { get; } = new Dictionary<TKey, TValue>();

        public OrderedDictionary()
            : this(DefaultInitialCapacity, null)
        {
        }

        public OrderedDictionary(int capacity)
            : this(capacity, null)
        {
        }

        public OrderedDictionary(IEqualityComparer<TKey> comparer)
            : this(DefaultInitialCapacity, comparer)
        {
        }

        public OrderedDictionary(int capacity, IEqualityComparer<TKey>? comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", "'capacity' must be non-negative");

            _initialCapacity = capacity;
            _comparer = comparer;
        }

        private static TKey ConvertToKeyType(object keyObject)
        {
            if (keyObject == null)
                throw new ArgumentNullException("key");

            if (keyObject is TKey key)
                return key;
            throw new ArgumentException("'key' must be of type " + _keyTypeName, "key");
        }

        private static TValue ConvertToValueType(object? value)
        {
            if (value == null)
            {
                if (_valueTypeIsReferenceType)
#pragma warning disable CS8603 // Possible null reference return.
                    return default;
#pragma warning restore CS8603 // Possible null reference return.
                else
                    throw new ArgumentNullException("value");
            }
            if (value is TValue val)
                return val;
            throw new ArgumentException("'value' must be of type " + _valueTypeName, "value");
        }

        private Dictionary<TKey, TValue> Dictionary
        {
            get
            {
                if (_dictionary == null)
                {
                    _dictionary = new Dictionary<TKey, TValue>(_initialCapacity, _comparer);
                }
                return _dictionary;
            }
        }

        private List<KeyValuePair<TKey, TValue>> List
        {
            get
            {
                if (_list == null)
                {
                    _list = new List<KeyValuePair<TKey, TValue>>(_initialCapacity);
                }
                return _list;
            }
        }

        IDictionaryEnumerator IOrderedDictionary.GetEnumerator() => Dictionary.GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator() => Dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => List.GetEnumerator();

        public void Insert(int index, TKey key, TValue value)
        {
            if (index > Count || index < 0)
                throw new ArgumentOutOfRangeException("index");

            Dictionary.Add(key, value);
            List.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
        }

        void IOrderedDictionary.Insert(int index, object key, object value) =>
            Insert(index, ConvertToKeyType(key), ConvertToValueType(value));

        public void RemoveAt(int index)
        {
            if (index >= Count || index < 0)
                throw new ArgumentOutOfRangeException("index", "'index' must be non-negative and less than the size of the collection");

            var key = List[index].Key;
            List.RemoveAt(index);
            Dictionary.Remove(key);
        }

        public TValue this[int index]
        {
            get => List[index].Value;
            set
            {
                if (index >= Count || index < 0)
                    throw new ArgumentOutOfRangeException("index", "'index' must be non-negative and less than the size of the collection");
                var key = List[index].Key;
                List[index] = new KeyValuePair<TKey, TValue>(key, value);
                Dictionary[key] = value;
            }
        }

        object? IOrderedDictionary.this[int index]
        {
            get => this[index];
            set => this[index] = ConvertToValueType(value);
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => Add(key, value);

        public int Add(TKey key, TValue value)
        {
            Dictionary.Add(key, value);
            List.Add(new KeyValuePair<TKey, TValue>(key, value));
            return Count - 1;
        }

        void IDictionary.Add(object key, object value) => Add(ConvertToKeyType(key), ConvertToValueType(value));

        public void Clear()
        {
            Dictionary.Clear();
            List.Clear();
        }

        public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);

        bool IDictionary.Contains(object key) => ContainsKey(ConvertToKeyType(key));

        bool IDictionary.IsFixedSize => false;

        public bool IsReadOnly => false;

        ICollection IDictionary.Keys => (ICollection)Keys;

        public int IndexOfKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            for (int index = 0; index < List.Count; index++)
            {
                var entry = List[index];
                var next = entry.Key;
                if (null != _comparer)
                {
                    if (_comparer.Equals(next, key))
                    {
                        return index;
                    }
                }
                else if (next != null && next.Equals(key))
                {
                    return index;
                }
            }
            return -1;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            int index = IndexOfKey(key);
            if (index >= 0)
            {
                if (Dictionary.Remove(key))
                {
                    List.RemoveAt(index);
                    return true;
                }
            }
            return false;
        }

        void IDictionary.Remove(object key) => Remove(ConvertToKeyType(key));

        ICollection IDictionary.Values => (ICollection)Values;

        public TValue this[TKey key]
        {
            get => Dictionary[key];
            set
            {
                if (Dictionary.ContainsKey(key))
                {
                    Dictionary[key] = value;
                    List[IndexOfKey(key)] = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        object? IDictionary.this[object key]
        {
            get => this[ConvertToKeyType(key)];
            set => this[ConvertToKeyType(key)] = ConvertToValueType(value);
        }

        void ICollection.CopyTo(Array array, int index) => ((ICollection)List).CopyTo(array, index);

        public int Count => List.Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        public ICollection<TKey> Keys => Dictionary.Keys;

        public bool TryGetValue(TKey key, out TValue value) => Dictionary.TryGetValue(key, out value);

        public ICollection<TValue> Values => Dictionary.Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Dictionary.Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Dictionary.Values;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Contains(item);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).CopyTo(array, arrayIndex);

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
    }
}
