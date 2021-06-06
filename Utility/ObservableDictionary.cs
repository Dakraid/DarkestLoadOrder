namespace DarkestLoadOrder.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    [Serializable]
    public class ObservableDictionary<TKey, TValue> : ObservableCollection<ObservableKeyValuePair<TKey, TValue>>,
        IDictionary<TKey, TValue>
    {
        public TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out var result))
                    throw new ArgumentException("Key not found", nameof(key));

                return result;
            }

            set
            {
                if (ContainsKey(key))
                    GetPairByTheKey(key).Value = value;
                else
                    Add(key, value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException($"The dictionary already contains the key \"{key}\"");

            Add(new ObservableKeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (ContainsKey(item.Key))
                throw new ArgumentException($"The dictionary already contains the key \"{item.Key}\"");

            Add(item.Key, item.Value);
        }

        public bool Remove(TKey key)
        {
            var remove = ThisAsCollection().Where(pair => Equals(key, pair.Key)).ToList();

            foreach (var pair in remove)
                ThisAsCollection().Remove(pair);

            return remove.Count > 0;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var pair = GetPairByTheKey(item.Key);

            if (Equals(pair, default(ObservableKeyValuePair<TKey, TValue>)))
                return false;

            if (!Equals(pair.Value, item.Value))
                return false;

            return ThisAsCollection().Remove(pair);
        }

        public bool ContainsKey(TKey key)
        {
            return !Equals(default(ObservableKeyValuePair<TKey, TValue>),
                ThisAsCollection().FirstOrDefault(i => Equals(key, i.Key)));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var pair = GetPairByTheKey(item.Key);

            if (Equals(pair, default(ObservableKeyValuePair<TKey, TValue>)))
                return false;

            return Equals(pair.Value, item.Value);
        }

        public ICollection<TKey> Keys => (from i in ThisAsCollection() select i.Key).ToList();

        public ICollection<TValue> Values => (from i in ThisAsCollection() select i.Value).ToList();

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default;
            var pair = GetPairByTheKey(key);

            if (Equals(pair, default(ObservableKeyValuePair<TKey, TValue>)))
                return false;

            value = pair.Value;

            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly => false;

        public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return (from i in ThisAsCollection() select new KeyValuePair<TKey, TValue>(i.Key, i.Value)).ToList()
                                                                                                       .GetEnumerator();
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            var arrayofItems = items.ToArray();

            if (arrayofItems.Any(i => ContainsKey(i.Key)))
                throw new ArgumentException($"The dictionary already contains the key \"{arrayofItems.First(i => ContainsKey(i.Key)).Key}\"");

            foreach (var (key, value) in arrayofItems)
                Add(key, value);
        }

        public void AddRange(IEnumerable<ObservableKeyValuePair<TKey, TValue>> items)
        {
            var arrayofItems = items.ToArray();

            if (arrayofItems.Any(i => ContainsKey(i.Key)))
                throw new ArgumentException($"The dictionary already contains the key \"{arrayofItems.First(i => ContainsKey(i.Key)).Key}\"");

            foreach (var item in arrayofItems)
                Add(item);
        }

        private bool Equals(TKey firstKey, TKey secondKey)
        {
            return EqualityComparer<TKey>.Default.Equals(firstKey, secondKey);
        }

        private ObservableCollection<ObservableKeyValuePair<TKey, TValue>> ThisAsCollection()
        {
            return this;
        }

        private ObservableKeyValuePair<TKey, TValue> GetPairByTheKey(TKey key)
        {
            return ThisAsCollection().FirstOrDefault(i => i.Key.Equals(key));
        }
    }
}
