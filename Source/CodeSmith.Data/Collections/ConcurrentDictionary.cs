using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace CodeSmith.Data.Collections
{
    [Serializable, ComVisible(false), DebuggerDisplay("Count = {Count}"), HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class ConcurrentDictionary<TKey, TValue>
        : IDictionary<TKey, TValue>, IDictionary
    {
        // Fields
        private const int DEFAULT_CAPACITY = 0x1f;
        private const int DEFAULT_CONCURRENCY_MULTIPLIER = 4;

        [NonSerialized]
        private volatile Node<TKey, TValue>[] _buckets;

        [NonSerialized]
        private volatile int[] _countPerLock;

        [NonSerialized]
        private object[] _locks;

        private readonly IEqualityComparer<TKey> _comparer;
        private KeyValuePair<TKey, TValue>[] _serializationArray;
        private int _serializationCapacity;
        private int _serializationConcurrencyLevel;

        // Methods
        public ConcurrentDictionary()
            : this(DefaultConcurrencyLevel, DEFAULT_CAPACITY)
        {
        }

        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : this(collection, EqualityComparer<TKey>.Default)
        {
        }

        public ConcurrentDictionary(IEqualityComparer<TKey> comparer)
            : this(DefaultConcurrencyLevel, DEFAULT_CAPACITY, comparer)
        {
        }

        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(DefaultConcurrencyLevel, collection, comparer)
        {
        }

        public ConcurrentDictionary(int concurrencyLevel, int capacity)
            : this(concurrencyLevel, capacity, EqualityComparer<TKey>.Default)
        {
        }

        public ConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(concurrencyLevel, DEFAULT_CAPACITY, comparer)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (comparer == null)
                throw new ArgumentNullException("comparer");
            InitializeFromCollection(collection);
        }

        public ConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
        {
            if (concurrencyLevel < 1)
                throw new ArgumentOutOfRangeException("concurrencyLevel", GetResource("ConcurrentDictionary_ConcurrencyLevelMustBePositive"));
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", GetResource("ConcurrentDictionary_CapacityMustNotBeNegative"));
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            if (capacity < concurrencyLevel)
                capacity = concurrencyLevel;

            _locks = new object[concurrencyLevel];
            for (int i = 0; i < _locks.Length; i++)
                _locks[i] = new object();

            _countPerLock = new int[_locks.Length];
            _buckets = new Node<TKey, TValue>[capacity];
            _comparer = comparer;
        }

        private void AcquireAllLocks(ref int locksAcquired)
        {
            AcquireLocks(0, _locks.Length, ref locksAcquired);
        }

        private void AcquireLocks(int fromInclusive, int toExclusive, ref int locksAcquired)
        {
            for (int i = fromInclusive; i < toExclusive; i++)
            {
                bool lockTaken = false;
                try
                {
                    lockTaken = Monitor.TryEnter(_locks[i]);
                }
                finally
                {
                    if (lockTaken)
                        locksAcquired++;
                }
            }
        }

        /// <summary>Adds a key/value pair to the ConcurrentDictionary if the key does not already exist, or updates a key/value pair in the ConcurrentDictionary if the key already exists.</summary>
        /// <returns>The new value for the key. This will be either be the result of addValueFactory (if the key was absent) or the result of updateValueFactory (if the key was present).</returns>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValue">The value to be added for an absent key</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is a null reference (Nothing in Visual Basic).-or-<paramref name="updateValueFactory" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="T:System.OverflowException">The dictionary contains too many elements.</exception>
        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            TValue updateValue = default(TValue);
            TValue originalValue;

            if (key == null)
                throw new ArgumentNullException("key");
            if (updateValueFactory == null)
                throw new ArgumentNullException("updateValueFactory");

            do
            {
                if (!TryGetValue(key, out originalValue))
                {
                    TValue resultingValue;
                    if (!TryAddInternal(key, addValue, false, true, out resultingValue))
                        continue;

                    return resultingValue;
                }
                updateValue = updateValueFactory(key, originalValue);
            }
            while (!TryUpdate(key, updateValue, originalValue));

            return updateValue;
        }

        /// <summary>Adds a key/value pair to the ConcurrentDictionary if the key does not already exist, or updates a key/value pair in the ConcurrentDictionary if the key already exists.</summary>
        /// <returns>The new value for the key. This will be either be the result of addValueFactory (if the key was absent) or the result of updateValueFactory (if the key was present).</returns>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValueFactory">The function used to generate a value for an absent key</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is a null reference (Nothing in Visual Basic).-or-<paramref name="addValueFactory" /> is a null reference (Nothing in Visual Basic).-or-<paramref name="updateValueFactory" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="T:System.OverflowException">The dictionary contains too many elements.</exception>
        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            TValue updateValue;
            TValue originalValue;

            if (key == null)
                throw new ArgumentNullException("key");

            if (addValueFactory == null)
                throw new ArgumentNullException("addValueFactory");

            if (updateValueFactory == null)
                throw new ArgumentNullException("updateValueFactory");

            do
            {
                if (!TryGetValue(key, out originalValue))
                {
                    TValue resultingValue;
                    updateValue = addValueFactory(key);
                    if (!TryAddInternal(key, updateValue, false, true, out resultingValue))
                        continue;

                    return resultingValue;
                }
                updateValue = updateValueFactory(key, originalValue);
            }
            while (!TryUpdate(key, updateValue, originalValue));

            return updateValue;
        }

        public void Clear()
        {
            int locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);
                _buckets = new Node<TKey, TValue>[DEFAULT_CAPACITY];
                Array.Clear(_countPerLock, 0, _countPerLock.Length);
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
        }

        public bool ContainsKey(TKey key)
        {
            TValue value;
            if (key == null)
                throw new ArgumentNullException("key");

            return TryGetValue(key, out value);
        }

        private void CopyToEntries(DictionaryEntry[] array, int index)
        {
            Node<TKey, TValue>[] buckets = _buckets;
            for (int i = 0; i < buckets.Length; i++)
            {
                for (Node<TKey, TValue> node = buckets[i]; node != null; node = node.Next)
                {
                    array[index] = new DictionaryEntry(node.Key, node.Value);
                    index++;
                }
            }
        }

        private void CopyToObjects(object[] array, int index)
        {
            Node<TKey, TValue>[] buckets = _buckets;
            for (int i = 0; i < buckets.Length; i++)
            {
                for (Node<TKey, TValue> node = buckets[i]; node != null; node = node.Next)
                {
                    array[index] = new KeyValuePair<TKey, TValue>(node.Key, node.Value);
                    index++;
                }
            }
        }

        private void CopyToPairs(KeyValuePair<TKey, TValue>[] array, int index)
        {
            Node<TKey, TValue>[] buckets = _buckets;
            for (int i = 0; i < buckets.Length; i++)
            {
                for (Node<TKey, TValue> node = buckets[i]; node != null; node = node.Next)
                {
                    array[index] = new KeyValuePair<TKey, TValue>(node.Key, node.Value);
                    index++;
                }
            }
        }

        private void GetBucketAndLockNo(int hashcode, out int bucketNo, out int lockNo, int bucketCount)
        {
            bucketNo = (hashcode & 0x7fffffff) % bucketCount;
            lockNo = bucketNo % _locks.Length;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var enumerator = new ConcurrentDictionaryEnumerator(this);
            return enumerator;
        }

        private ReadOnlyCollection<TKey> GetKeys()
        {
            ReadOnlyCollection<TKey> readOnlyCollection;
            int locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);
                var list = new List<TKey>();
                for (int i = 0; i < _buckets.Length; i++)
                    for (Node<TKey, TValue> node = _buckets[i]; node != null; node = node.Next)
                        list.Add(node.Key);

                readOnlyCollection = new ReadOnlyCollection<TKey>(list);
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
            return readOnlyCollection;
        }

        /// <summary>Adds a key/value pair to the ConcurrentDictionary if the key does not already exist.</summary>
        /// <returns>The value for the key. This will be either the existing value for the key if the key is already in the dictionary, or the new value if the key was not in the dictionary.</returns>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">the value to be added, if the key does not already exist</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="T:System.OverflowException">The dictionary contains too many elements.</exception>
        public TValue GetOrAdd(TKey key, TValue value)
        {
            TValue resultingValue;
            if (key == null)
                throw new ArgumentNullException("key");

            TryAddInternal(key, value, false, true, out resultingValue);
            return resultingValue;
        }

        /// <summary>Adds a key/value pair to the ConcurrentDictionary if the key does not already exist.</summary>
        /// <returns>The value for the key. This will be either the existing value for the key if the key is already in the dictionary, or the new value for the key as returned by valueFactory if the key was not in the dictionary.</returns>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">The function used to generate a value for the key</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is a null reference (Nothing in Visual Basic).-or-<paramref name="valueFactory" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="T:System.OverflowException">The dictionary contains too many elements.</exception>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue resultingValue;
            if (key == null)
                throw new ArgumentNullException("key");

            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");

            if (!TryGetValue(key, out resultingValue))
                TryAddInternal(key, valueFactory(key), false, true, out resultingValue);

            return resultingValue;
        }

        private string GetResource(string key)
        {
            return ""; // Environment.GetResourceString(key);
        }

        private ReadOnlyCollection<TValue> GetValues()
        {
            ReadOnlyCollection<TValue> readOnlyCollection;
            int locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);
                var list = new List<TValue>();
                for (int i = 0; i < _buckets.Length; i++)
                    for (Node<TKey, TValue> node = _buckets[i]; node != null; node = node.Next)
                        list.Add(node.Value);

                readOnlyCollection = new ReadOnlyCollection<TValue>(list);
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
            return readOnlyCollection;
        }

        private void GrowTable(Node<TKey, TValue>[] buckets)
        {
            int locksAcquired = 0;
            try
            {
                AcquireLocks(0, 1, ref locksAcquired);
                if (buckets == _buckets)
                {
                    int size;
                    try
                    {
                        size = (buckets.Length * 2) + 1;
                        while ((((size % 3) == 0) || ((size % 5) == 0)) || ((size % 7) == 0))
                        {
                            size += 2;
                        }
                    }
                    catch (OverflowException)
                    {
                        return;
                    }

                    var nodeArray = new Node<TKey, TValue>[size];
                    int[] numArray = new int[_locks.Length];
                    AcquireLocks(1, _locks.Length, ref locksAcquired);

                    for (int i = 0; i < buckets.Length; i++)
                    {
                        Node<TKey, TValue> next;
                        for (Node<TKey, TValue> node = buckets[i]; node != null; node = next)
                        {
                            int bucketNo;
                            int lockNo;
                            next = node.Next;
                            GetBucketAndLockNo(node.Hashcode, out bucketNo, out lockNo, nodeArray.Length);
                            nodeArray[bucketNo] = new Node<TKey, TValue>(node.Key, node.Value, node.Hashcode, nodeArray[bucketNo]);
                            numArray[lockNo]++;
                        }
                    }
                    _buckets = nodeArray;
                    _countPerLock = numArray;
                }
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
        }

        private void InitializeFromCollection(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            foreach (KeyValuePair<TKey, TValue> pair in collection)
            {
                TValue resultingValue;

                if (pair.Key == null)
                    throw new ArgumentNullException("key");

                if (!TryAddInternal(pair.Key, pair.Value, false, false, out resultingValue))
                    throw new ArgumentException(GetResource("ConcurrentDictionary_SourceContainsDuplicateKeys"));
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            KeyValuePair<TKey, TValue>[] serializationArray = _serializationArray;
            _buckets = new Node<TKey, TValue>[_serializationCapacity];
            _countPerLock = new int[_serializationConcurrencyLevel];
            _locks = new object[_serializationConcurrencyLevel];

            for (int i = 0; i < _locks.Length; i++)
                _locks[i] = new object();

            InitializeFromCollection(serializationArray);
            _serializationArray = null;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            _serializationArray = ToArray();
            _serializationConcurrencyLevel = _locks.Length;
            _serializationCapacity = _buckets.Length;
        }

        private void ReleaseLocks(int fromInclusive, int toExclusive)
        {
            for (int i = fromInclusive; i < toExclusive; i++)
                Monitor.Exit(_locks[i]);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            ((IDictionary<TKey, TValue>)this).Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            TValue value;
            if (!TryGetValue(keyValuePair.Key, out value))
                return false;

            return EqualityComparer<TValue>.Default.Equals(value, keyValuePair.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", GetResource("ConcurrentDictionary_IndexIsNegative"));

            int locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);
                int countPerLock = 0;
                for (int i = 0; i < _locks.Length; i++)
                    countPerLock += _countPerLock[i];

                if (((array.Length - countPerLock) < index) || (countPerLock < 0))
                    throw new ArgumentException(GetResource("ConcurrentDictionary_ArrayNotLargeEnough"));

                CopyToPairs(array, index);
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            TValue value;
            if (keyValuePair.Key == null)
                throw new ArgumentNullException(GetResource("ConcurrentDictionary_ItemKeyIsNull"));

            return TryRemoveInternal(keyValuePair.Key, out value, true, keyValuePair.Value);
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new ArgumentException(GetResource("ConcurrentDictionary_KeyAlreadyExisted"));
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            TValue value;
            return TryRemove(key, out value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", GetResource("ConcurrentDictionary_IndexIsNegative"));

            int locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);
                int size = 0;
                for (int i = 0; i < _locks.Length; i++)
                    size += _countPerLock[i];
                if (((array.Length - size) < index) || (size < 0))
                    throw new ArgumentException(GetResource("ConcurrentDictionary_ArrayNotLargeEnough"));

                var pairArray = array as KeyValuePair<TKey, TValue>[];
                if (pairArray != null)
                {
                    CopyToPairs(pairArray, index);
                }
                else
                {
                    var entryArray = array as DictionaryEntry[];
                    if (entryArray != null)
                    {
                        CopyToEntries(entryArray, index);
                    }
                    else
                    {
                        var objArray = array as object[];
                        if (objArray == null)
                            throw new ArgumentException(GetResource("ConcurrentDictionary_ArrayIncorrectType"), "array");

                        CopyToObjects(objArray, index);
                    }
                }
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
        }

        void IDictionary.Add(object key, object value)
        {
            TValue local;

            if (key == null)
                throw new ArgumentNullException("key");

            if (!(key is TKey))
                throw new ArgumentException(GetResource("ConcurrentDictionary_TypeOfKeyIncorrect"));

            try
            {
                local = (TValue)value;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(GetResource("ConcurrentDictionary_TypeOfValueIncorrect"));
            }

            ((IDictionary<TKey, TValue>)this).Add((TKey)key, local);
        }

        bool IDictionary.Contains(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return ((key is TKey) && ContainsKey((TKey)key));
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator<TKey, TValue>(this);
        }

        void IDictionary.Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (key is TKey)
            {
                TValue value;
                TryRemove((TKey)key, out value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            KeyValuePair<TKey, TValue>[] keyValuePairs;
            int locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);
                int size = 0;
                for (int i = 0; i < _locks.Length; i++)
                    size += _countPerLock[i];

                var array = new KeyValuePair<TKey, TValue>[size];
                CopyToPairs(array, 0);
                keyValuePairs = array;
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
            return keyValuePairs;
        }

        public bool TryAdd(TKey key, TValue value)
        {
            TValue resultingValue;
            if (key == null)
                throw new ArgumentNullException("key");

            return TryAddInternal(key, value, false, true, out resultingValue);
        }

        private bool TryAddInternal(TKey key, TValue value, bool updateIfExists, bool acquireLock, out TValue resultingValue)
        {
            int bucketNo;
            int lockNo;
            Node<TKey, TValue>[] nodeArray;
            int hashCode = _comparer.GetHashCode(key);
        Retry:
            nodeArray = _buckets;
            GetBucketAndLockNo(hashCode, out bucketNo, out lockNo, nodeArray.Length);
            bool flag = false;
            bool lockTaken = false;

            try
            {
                if (acquireLock)
                    lockTaken = Monitor.TryEnter(_locks[lockNo]);
                if (nodeArray != _buckets)
                    goto Retry;

                Node<TKey, TValue> previousNode = null;
                for (Node<TKey, TValue> currentNode = nodeArray[bucketNo]; currentNode != null; currentNode = currentNode.Next)
                {
                    if (_comparer.Equals(currentNode.Key, key))
                    {
                        if (updateIfExists)
                        {
                            var newNode = new Node<TKey, TValue>(currentNode.Key, value, hashCode, currentNode.Next);
                            if (previousNode == null)
                                nodeArray[bucketNo] = newNode;
                            else
                                previousNode.Next = newNode;

                            resultingValue = value;
                        }
                        else
                        {
                            resultingValue = currentNode.Value;
                        }
                        return false;
                    }
                    previousNode = currentNode;
                }

                nodeArray[bucketNo] = new Node<TKey, TValue>(key, value, hashCode, nodeArray[bucketNo]);
                _countPerLock[lockNo] += 1;
                if (_countPerLock[lockNo] > (nodeArray.Length / _locks.Length))
                    flag = true;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(_locks[lockNo]);
            }

            if (flag)
                GrowTable(nodeArray);

            resultingValue = value;
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int bucketNo;
            int lockNo;
            if (key == null)
                throw new ArgumentNullException("key");

            Node<TKey, TValue>[] buckets = _buckets;
            GetBucketAndLockNo(_comparer.GetHashCode(key), out bucketNo, out lockNo, buckets.Length);
            Node<TKey, TValue> next = buckets[bucketNo];
            Thread.MemoryBarrier();

            while (next != null)
            {
                if (_comparer.Equals(next.Key, key))
                {
                    value = next.Value;
                    return true;
                }
                next = next.Next;
            }

            value = default(TValue);
            return false;
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return TryRemoveInternal(key, out value, false, default(TValue));
        }

        private bool TryRemoveInternal(TKey key, out TValue value, bool matchValue, TValue oldValue)
        {
            Node<TKey, TValue>[] nodeArray;
            int bucketNo;
            int lockNo;

        Retry:
            nodeArray = _buckets;
            GetBucketAndLockNo(_comparer.GetHashCode(key), out bucketNo, out lockNo, nodeArray.Length);
            bool lockTaken = false;

            try
            {
                lockTaken = Monitor.TryEnter(_locks[lockNo]);
                if (nodeArray != _buckets)
                    goto Retry;

                Node<TKey, TValue> previousNode = null;
                for (Node<TKey, TValue> currentNode = _buckets[bucketNo]; currentNode != null; currentNode = currentNode.Next)
                {
                    if (_comparer.Equals(currentNode.Key, key))
                    {
                        if (matchValue && !EqualityComparer<TValue>.Default.Equals(oldValue, currentNode.Value))
                        {
                            value = default(TValue);
                            return false;
                        }

                        if (previousNode == null)
                            _buckets[bucketNo] = currentNode.Next;
                        else
                            previousNode.Next = currentNode.Next;

                        value = currentNode.Value;
                        _countPerLock[lockNo] -= 1;
                        return true;
                    }
                    previousNode = currentNode;
                }
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(_locks[lockNo]);
            }
            value = default(TValue);
            return false;
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            int bucketNo;
            int lockNo;
            Node<TKey, TValue>[] nodeArray;

            if (key == null)
                throw new ArgumentNullException("key");

            int hashCode = _comparer.GetHashCode(key);
            IEqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;

        Retry:
            nodeArray = _buckets;
            GetBucketAndLockNo(hashCode, out bucketNo, out lockNo, nodeArray.Length);
            bool lockTaken = false;

            try
            {
                lockTaken = Monitor.TryEnter(_locks[lockNo]);
                if (nodeArray != _buckets)
                    goto Retry;

                Node<TKey, TValue> previousNode = null;
                for (Node<TKey, TValue> currentNode = nodeArray[bucketNo]; currentNode != null; currentNode = currentNode.Next)
                {
                    if (_comparer.Equals(currentNode.Key, key))
                    {
                        if (!comparer.Equals(currentNode.Value, comparisonValue))
                            return false;

                        var newNode = new Node<TKey, TValue>(currentNode.Key, newValue, hashCode, currentNode.Next);
                        if (previousNode == null)
                            nodeArray[bucketNo] = newNode;
                        else
                            previousNode.Next = newNode;

                        return true;
                    }
                    previousNode = currentNode;
                }
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(_locks[lockNo]);
            }

            return false;
        }

        // Properties
        public int Count
        {
            get
            {
                int size = 0;
                int locksAcquired = 0;
                try
                {
                    AcquireAllLocks(ref locksAcquired);
                    for (int i = 0; i < _countPerLock.Length; i++)
                        size += _countPerLock[i];
                }
                finally
                {
                    ReleaseLocks(0, locksAcquired);
                }
                return size;
            }
        }

        private static int DefaultConcurrencyLevel
        {
            get { return (DEFAULT_CONCURRENCY_MULTIPLIER * Environment.ProcessorCount); }
        }

        public bool IsEmpty
        {
            get
            {
                int locksAcquired = 0;
                try
                {
                    AcquireAllLocks(ref locksAcquired);
                    for (int i = 0; i < _countPerLock.Length; i++)
                        if (_countPerLock[i] != null)
                            return false;
                }
                finally
                {
                    ReleaseLocks(0, locksAcquired);
                }
                return true;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!TryGetValue(key, out value))
                    throw new KeyNotFoundException();

                return value;
            }
            set
            {
                TValue resultingValue;
                if (key == null)
                    throw new ArgumentNullException("key");

                TryAddInternal(key, value, true, true, out resultingValue);
            }
        }

        public ICollection<TKey> Keys
        {
            get { return GetKeys(); }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                TValue value;
                if (key == null)
                    throw new ArgumentNullException("key");

                if ((key is TKey) && TryGetValue((TKey)key, out value))
                    return value;

                return null;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                if (!(key is TKey))
                    throw new ArgumentException(GetResource("ConcurrentDictionary_TypeOfKeyIncorrect"));

                if (!(value is TValue))
                    throw new ArgumentException(GetResource("ConcurrentDictionary_TypeOfValueIncorrect"));

                this[(TKey)key] = (TValue)value;
            }
        }

        ICollection IDictionary.Keys
        {
            get { return GetKeys(); }
        }

        ICollection IDictionary.Values
        {
            get { return GetValues(); }
        }

        public ICollection<TValue> Values
        {
            get { return GetValues(); }
        }

        // Nested Types
        private sealed class ConcurrentDictionaryEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
        {
            // Fields
            private int _state;
            private KeyValuePair<TKey, TValue> _current;
            private ConcurrentDictionary<TKey, TValue> _dictionary;
            private Node<TKey, TValue>[] _buckets;
            private Node<TKey, TValue> _currentNode;
            private int _index;

            // Methods
            public ConcurrentDictionaryEnumerator(ConcurrentDictionary<TKey, TValue> dictionary)
            {
                _state = 0;
                _dictionary = dictionary;
            }

            public bool MoveNext()
            {
                switch (_state)
                {
                    case 0:
                        _state = -1;
                        _buckets = _dictionary._buckets;
                        _index = 0;
                        if (SetCurrent())
                            return true;

                        break;
                    case 1:
                        _state = -1;
                        _currentNode = _currentNode.Next;
                        if (_currentNode != null)
                        {
                            _current = new KeyValuePair<TKey, TValue>(_currentNode.Key, _currentNode.Value);
                            _state = 1;
                            return true;
                        }
                        _index++;

                        if (SetCurrent())
                            return true;

                        break;
                }

                return false;
            }

            private bool SetCurrent()
            {
                while (_index < _buckets.Length)
                {
                    _currentNode = _buckets[_index];
                    Thread.MemoryBarrier();
                    while (_currentNode != null)
                    {
                        _current = new KeyValuePair<TKey, TValue>(_currentNode.Key, _currentNode.Value);
                        _state = 1;
                        return true;
                    }
                    _index++;
                }
                return false;
            }

            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            // Properties
            KeyValuePair<TKey, TValue> IEnumerator<KeyValuePair<TKey, TValue>>.Current
            {
                get { return _current; }
            }

            object IEnumerator.Current
            {
                get { return _current; }
            }
        }


        private class DictionaryEnumerator<TKey, TValue>
            : IDictionaryEnumerator
        {
            // Fields
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

            // Methods
            internal DictionaryEnumerator(ConcurrentDictionary<TKey, TValue> dictionary)
            {
                _enumerator = dictionary.GetEnumerator();
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            // Properties
            public object Current
            {
                get { return Entry; }
            }

            public DictionaryEntry Entry
            {
                get { return new DictionaryEntry(_enumerator.Current.Key, _enumerator.Current.Value); }
            }

            public object Key
            {
                get { return _enumerator.Current.Key; }
            }

            public object Value
            {
                get { return _enumerator.Current.Value; }
            }
        }

        private class Node<TKey, TValue>
        {
            // Fields
            internal readonly int Hashcode;
            internal readonly TKey Key;
            internal volatile ConcurrentDictionary<TKey, TValue>.Node<TKey, TValue> Next;
            internal readonly TValue Value;

            // Methods
            internal Node(TKey key, TValue value, int hashcode)
                : this(key, value, hashcode, null)
            {
            }

            internal Node(TKey key, TValue value, int hashcode, ConcurrentDictionary<TKey, TValue>.Node<TKey, TValue> next)
            {
                Key = key;
                Value = value;
                Next = next;
                Hashcode = hashcode;
            }
        }
    }


}
