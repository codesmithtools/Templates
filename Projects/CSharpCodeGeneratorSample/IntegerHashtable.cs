using System;
using System.Collections;

namespace CSharpCodeGeneratorSample
{
	#region IntegerHashtable
	public class IntegerHashtable : IDictionary, ICollection, IEnumerable, ICloneable
	{
		protected Hashtable innerHash;
		
		#region "Constructors"
		public  IntegerHashtable()
		{
			innerHash = new Hashtable();
		}
		
		public IntegerHashtable(IntegerHashtable original)
		{
			innerHash = new Hashtable (original.innerHash);
		}
		
		public IntegerHashtable(IDictionary dictionary)
		{
			innerHash = new Hashtable (dictionary);
		}
		
		public IntegerHashtable(int capacity)
		{
			innerHash = new Hashtable(capacity);
		}
		
		public IntegerHashtable(IDictionary dictionary, float loadFactor)
		{
			innerHash = new Hashtable(dictionary, loadFactor);
		}
		
		public IntegerHashtable(IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable (codeProvider, comparer);
		}
		
		public IntegerHashtable(int capacity, int loadFactor)
		{
			innerHash = new Hashtable(capacity, loadFactor);
		}
		
		public IntegerHashtable(IDictionary dictionary, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable (dictionary, codeProvider, comparer);
		}
		
		public IntegerHashtable(int capacity, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable (capacity, codeProvider, comparer);
		}
		
		public IntegerHashtable(IDictionary dictionary, float loadFactor, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable (dictionary, loadFactor, codeProvider, comparer);
		}
		
		public IntegerHashtable(int capacity, float loadFactor, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable (capacity, loadFactor, codeProvider, comparer);
		}
		#endregion

		#region Implementation of IDictionary
		public IntegerHashtableEnumerator GetEnumerator()
		{
			return new IntegerHashtableEnumerator(this);
		}
        	
		System.Collections.IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new IntegerHashtableEnumerator(this);
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public void Remove(int key)
		{
			innerHash.Remove (key);
		}
		
		void IDictionary.Remove(object key)
		{
			Remove ((int)key);
		}
		
		public bool Contains(int key)
		{
			return innerHash.Contains(key);
		}
		
		bool IDictionary.Contains(object key)
		{
			return Contains((int)key);
		}
		
		public void Clear()
		{
			innerHash.Clear();		
		}
		
		public void Add(int key, int value)
		{
			innerHash.Add (key, value);
		}
		
		void IDictionary.Add(object key, object value)
		{
			Add ((int)key, (int)value);
		}
		
		public bool IsReadOnly
		{
			get
			{
				return innerHash.IsReadOnly;
			}
		}
		
		public int this[int key]
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
				return this[(int)key];
			}
			set
			{
				this[(int)key] = (int)value;
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
		public IntegerHashtable Clone()
		{
			IntegerHashtable clone = new IntegerHashtable();
			clone.innerHash = (Hashtable) innerHash.Clone();
			
			return clone;
		}
		
		object ICloneable.Clone()
		{
			return Clone();
		}
		#endregion
		
		#region "HashTable Methods"
		public bool ContainsKey (int key)
		{
			return innerHash.ContainsKey(key);
		}
		
		public bool ContainsValue (int value)
		{
			return innerHash.ContainsValue(value);
		}
		
		public static IntegerHashtable Synchronized(IntegerHashtable nonSync)
		{
			IntegerHashtable sync = new IntegerHashtable();
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
	
	public class IntegerHashtableEnumerator : IDictionaryEnumerator
	{
		private IDictionaryEnumerator innerEnumerator;
		
		internal IntegerHashtableEnumerator (IntegerHashtable enumerable)
		{
			innerEnumerator = enumerable.InnerHash.GetEnumerator();
		}
		
		#region Implementation of IDictionaryEnumerator
		public int Key
		{
			get
			{
				return (int)innerEnumerator.Key;
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

