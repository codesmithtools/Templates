using System;
using System.Collections;

namespace CSharpCodeGeneratorSample
{
	#region StringIntegerHashtable
	public class StringIntegerHashtable : IDictionary, ICollection, IEnumerable, ICloneable
	{
		protected Hashtable innerHash;
		
		#region "Constructors"
		public  StringIntegerHashtable()
		{
			innerHash = new Hashtable();
		}
		
		public StringIntegerHashtable(StringIntegerHashtable original)
		{
			innerHash = new Hashtable (original.innerHash);
		}
		
		public StringIntegerHashtable(IDictionary dictionary)
		{
			innerHash = new Hashtable (dictionary);
		}
		
		public StringIntegerHashtable(int capacity)
		{
			innerHash = new Hashtable(capacity);
		}
		
		public StringIntegerHashtable(IDictionary dictionary, float loadFactor)
		{
			innerHash = new Hashtable(dictionary, loadFactor);
		}
		
		public StringIntegerHashtable(IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable (codeProvider, comparer);
		}
		
		public StringIntegerHashtable(int capacity, int loadFactor)
		{
			innerHash = new Hashtable(capacity, loadFactor);
		}
		
		public StringIntegerHashtable(IDictionary dictionary, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable (dictionary, codeProvider, comparer);
		}
		
		public StringIntegerHashtable(int capacity, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable (capacity, codeProvider, comparer);
		}
		
		public StringIntegerHashtable(IDictionary dictionary, float loadFactor, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable (dictionary, loadFactor, codeProvider, comparer);
		}
		
		public StringIntegerHashtable(int capacity, float loadFactor, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable (capacity, loadFactor, codeProvider, comparer);
		}
		#endregion

		#region Implementation of IDictionary
		public StringIntegerHashtableEnumerator GetEnumerator()
		{
			return new StringIntegerHashtableEnumerator(this);
		}
        	
		System.Collections.IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new StringIntegerHashtableEnumerator(this);
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public void Remove(string key)
		{
			innerHash.Remove (key);
		}
		
		void IDictionary.Remove(object key)
		{
			Remove ((string)key);
		}
		
		public bool Contains(string key)
		{
			return innerHash.Contains(key);
		}
		
		bool IDictionary.Contains(object key)
		{
			return Contains((string)key);
		}
		
		public void Clear()
		{
			innerHash.Clear();		
		}
		
		public void Add(string key, int value)
		{
			innerHash.Add (key, value);
		}
		
		void IDictionary.Add(object key, object value)
		{
			Add ((string)key, (int)value);
		}
		
		public bool IsReadOnly
		{
			get
			{
				return innerHash.IsReadOnly;
			}
		}
		
		public int this[string key]
		{
			get
			{
				return (int) innerHash[key];
			}
			set
			{
				innerHash[key] = value;
			}
		}
		
		object IDictionary.this[object key]
		{
			get
			{
				return this[(string)key];
			}
			set
			{
				this[(string)key] = (int)value;
			}
		}
        	
		public System.Collections.ICollection Values
		{
			get
			{
				return innerHash.Values;
			}
		}
		
		public System.Collections.ICollection Keys
		{
			get
			{
				return innerHash.Keys;
			}
		}
		
		public bool IsFixedSize
		{
			get
			{
				return innerHash.IsFixedSize;
			}
		}
		#endregion
		
		#region Implementation of ICollection
		public void CopyTo(System.Array array, int index)
		{
			innerHash.CopyTo (array, index);
		}
		
		public bool IsSynchronized
		{
			get
			{
				return innerHash.IsSynchronized;
			}
		}
		
		public int Count
		{
			get
			{
				return innerHash.Count;
			}
		}
		
		public object SyncRoot
		{
			get
			{
				return innerHash.SyncRoot;
			}
		}
		#endregion
		
		#region Implementation of ICloneable
		public StringIntegerHashtable Clone()
		{
			StringIntegerHashtable clone = new StringIntegerHashtable();
			clone.innerHash = (Hashtable) innerHash.Clone();
			
			return clone;
		}
		
		object ICloneable.Clone()
		{
			return Clone();
		}
		#endregion
		
		#region "HashTable Methods"
		public bool ContainsKey (string key)
		{
			return innerHash.ContainsKey(key);
		}
		
		public bool ContainsValue (int value)
		{
			return innerHash.ContainsValue(value);
		}
		
		public static StringIntegerHashtable Synchronized(StringIntegerHashtable nonSync)
		{
			StringIntegerHashtable sync = new StringIntegerHashtable();
			sync.innerHash = Hashtable.Synchronized(nonSync.innerHash);

			return sync;
		}
		#endregion

		internal Hashtable InnerHash
		{
			get
			{
				return innerHash;
			}
		}
	}
	
	public class StringIntegerHashtableEnumerator : IDictionaryEnumerator
	{
		private IDictionaryEnumerator innerEnumerator;
		
		internal StringIntegerHashtableEnumerator (StringIntegerHashtable enumerable)
		{
			innerEnumerator = enumerable.InnerHash.GetEnumerator();
		}
		
		#region Implementation of IDictionaryEnumerator
		public string Key
		{
			get
			{
				return (string)innerEnumerator.Key;
			}
		}
		
		object IDictionaryEnumerator.Key
		{
			get
			{
				return Key;
			}
		}
		
		public int Value
		{
			get
			{
				return (int)innerEnumerator.Value;
			}
		}
		
		object IDictionaryEnumerator.Value
		{
			get
			{
				return Value;
			}
		}
		
		public System.Collections.DictionaryEntry Entry
		{
			get
			{
				return innerEnumerator.Entry;
			}
		}
		#endregion
		
		#region Implementation of IEnumerator
		public void Reset()
		{
			innerEnumerator.Reset();
		}
		
		public bool MoveNext()
		{
			return innerEnumerator.MoveNext();
		}
		
		public object Current
		{
			get
			{
				return innerEnumerator.Current;
			}
		}
		#endregion
	}
	#endregion
}

