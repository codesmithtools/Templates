Imports System
Imports System.Collections
	#Region "StringHashtable"
    Public Class StringHashtable
        Implements IDictionary
        Implements ICollection
        Implements IEnumerable
        Implements ICloneable
        Protected _innerHash As Hashtable

#Region "Constructors"
        Public Sub New()
            _innerHash = New Hashtable()
        End Sub

        Public Sub New(ByVal original As StringHashtable)
            _innerHash = New Hashtable(original.innerHash)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary)
            _innerHash = New Hashtable(dictionary)
        End Sub

        Public Sub New(ByVal capacity As Integer)
            _innerHash = New Hashtable(capacity)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary, ByVal loadFactor As Single)
            _innerHash = New Hashtable(dictionary, loadFactor)
        End Sub

        Public Sub New(ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(codeProvider, comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer, ByVal loadFactor As Integer)
            _innerHash = New Hashtable(capacity, loadFactor)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(dictionary, codeProvider, comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(capacity, codeProvider, comparer)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary, ByVal loadFactor As Single, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(dictionary, loadFactor, codeProvider, comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer, ByVal loadFactor As Integer, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(capacity, loadFactor, codeProvider, comparer)
        End Sub


#End Region

#Region "Implementation of IDictionary"

        Private Function _GetEnumerator() As IDictionaryEnumerator
            Return New StringHashtableEnumerator(Me)
        End Function

        Public Function IDictionary_GetEnumerator() As IDictionaryEnumerator Implements IDictionary.GetEnumerator
            Return _GetEnumerator()
        End Function

        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _GetEnumerator()
        End Function

        Public Sub Remove(ByVal key As String)
            _innerHash.Remove(key)
        End Sub

        Public Sub Remove(ByVal key As Object) Implements IDictionary.Remove
            Remove(CType(key, String))
        End Sub

        Public Function Contains(ByVal key As String) As Boolean
            Return _innerHash.Contains(key)

        End Function

        Public Function Contains(ByVal key As Object) As Boolean Implements IDictionary.Contains
            Return Contains(CType(key, String))
        End Function

        Public Sub Clear() Implements IDictionary.Clear
            _innerHash.Clear()
        End Sub

        Public Sub Add(ByVal key As String, ByVal value As String)
            _innerHash.Add(key, value)
        End Sub

        Public Sub Add(ByVal key As Object, ByVal value As Object) Implements IDictionary.Add
            Add(CType(key, String), CType(value, String))
        End Sub

        Public ReadOnly Property IsReadOnly() As Boolean Implements IDictionary.IsReadOnly
            Get
                Return _innerHash.IsReadOnly
            End Get
        End Property

        Default Public Property Item(ByVal key As String) As String
            Get
                Return CType(_innerHash(key), String)
            End Get
            Set(ByVal Value As String)
                _innerHash(key) = value
            End Set
        End Property


        Default Public Property Item(ByVal key As Object) As Object Implements IDictionary.Item
            Get
                Return item(CType(key, String))
            End Get
            Set(ByVal Value As Object)
                item(CType(key, String)) = CType(value, String)
            End Set
        End Property

        Public ReadOnly Property Values() As System.Collections.ICollection Implements IDictionary.Values
            Get
                Return _innerHash.Values
            End Get
        End Property

        Public ReadOnly Property Keys() As System.Collections.ICollection Implements IDictionary.Keys
            Get
                Return _innerHash.Keys
            End Get
        End Property

        Public ReadOnly Property IsFixedSize() As Boolean Implements IDictionary.IsFixedSize
            Get
                Return _innerHash.IsFixedSize
            End Get
        End Property

#End Region

#Region "Implementation of ICollection"

        Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements ICollection.CopyTo
            _innerHash.CopyTo(array, index)
        End Sub

        Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
            Get
                Return _innerHash.IsSynchronized
            End Get
        End Property

        Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
            Get
                Return _innerHash.Count
            End Get
        End Property

        Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
            Get
                Return _innerHash.SyncRoot
            End Get
        End Property

#End Region

#Region "Implementation of ICloneable"

        Public Function Clone() As StringHashtable
            Dim innerClone As StringHashtable = New StringHashtable()
            innerClone._innerHash = CType(_innerHash.Clone(), Hashtable)

            Return innerClone
        End Function

        Public Function ICloneable_Clone() As Object Implements ICloneable.Clone
            Return Clone()
        End Function
#End Region

#Region "HashTable Methods"
        Public Function ContainsKey(ByVal key As String) As Boolean
            Return _innerHash.ContainsKey(key)
        End Function

        Public Function ContainsValue(ByVal value As String) As Boolean
            Return _innerHash.ContainsValue(value)
        End Function

        Public Shared Function Synchronized(ByVal nonSync As StringHashtable) As StringHashtable
            Dim sync As StringHashtable = New StringHashtable()
            sync._innerHash = Hashtable.Synchronized(nonSync.innerHash)
            Return sync
        End Function
#End Region

        Friend ReadOnly Property InnerHash() As Hashtable
            Get
                Return _innerHash
            End Get
        End Property
    End Class

    Public Class StringHashtableEnumerator
        Implements IDictionaryEnumerator

        Private innerEnumerator As IDictionaryEnumerator

        Friend Sub New(ByVal enumerable As StringHashtable)
            innerEnumerator = enumerable.InnerHash.GetEnumerator()
        End Sub

#Region "Implementation of IDictionaryEnumerator"
        Public ReadOnly Property Key() As String
            Get
                Return CType(innerEnumerator.Key, String)
            End Get
        End Property

        Public ReadOnly Property IDictionaryEnumerator_Key() As Object Implements IDictionaryEnumerator.Key
            Get
                Return Key
            End Get
        End Property

        Public ReadOnly Property Value() As String
            Get
                Return CType(innerEnumerator.Value, String)
            End Get
        End Property

        Public ReadOnly Property IDictionaryEnumerator_Value() As Object Implements IDictionaryEnumerator.Value
            Get
                Return Value
            End Get
        End Property

        Public ReadOnly Property Entry() As System.Collections.DictionaryEntry Implements IDictionaryEnumerator.Entry
            Get
                Return innerEnumerator.Entry
            End Get
        End Property
#End Region

#Region "Implementation of IEnumerator"
        Public Sub Reset() Implements IDictionaryEnumerator.Reset
            innerEnumerator.Reset()
        End Sub

        Public Function MoveNext() As Boolean Implements IDictionaryEnumerator.MoveNext
            Return innerEnumerator.MoveNext()
        End Function

        Public ReadOnly Property Current() As Object Implements IDictionaryEnumerator.Current
            Get
                Return innerEnumerator.Current
            End Get
        End Property
#End Region
    End Class
	#End Region
	#Region "StringIntegerHashtable"
    Public Class StringIntegerHashtable
        Implements IDictionary
        Implements ICollection
        Implements IEnumerable
        Implements ICloneable
        Protected _innerHash As Hashtable

#Region "Constructors"
        Public Sub New()
            _innerHash = New Hashtable()
        End Sub

        Public Sub New(ByVal original As StringIntegerHashtable)
            _innerHash = New Hashtable(original.innerHash)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary)
            _innerHash = New Hashtable(dictionary)
        End Sub

        Public Sub New(ByVal capacity As Integer)
            _innerHash = New Hashtable(capacity)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary, ByVal loadFactor As Single)
            _innerHash = New Hashtable(dictionary, loadFactor)
        End Sub

        Public Sub New(ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(codeProvider, comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer, ByVal loadFactor As Integer)
            _innerHash = New Hashtable(capacity, loadFactor)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(dictionary, codeProvider, comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(capacity, codeProvider, comparer)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary, ByVal loadFactor As Single, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(dictionary, loadFactor, codeProvider, comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer, ByVal loadFactor As Integer, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(capacity, loadFactor, codeProvider, comparer)
        End Sub


#End Region

#Region "Implementation of IDictionary"

        Private Function _GetEnumerator() As IDictionaryEnumerator
            Return New StringIntegerHashtableEnumerator(Me)
        End Function

        Public Function IDictionary_GetEnumerator() As IDictionaryEnumerator Implements IDictionary.GetEnumerator
            Return _GetEnumerator()
        End Function

        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _GetEnumerator()
        End Function

        Public Sub Remove(ByVal key As String)
            _innerHash.Remove(key)
        End Sub

        Public Sub Remove(ByVal key As Object) Implements IDictionary.Remove
            Remove(CType(key, String))
        End Sub

        Public Function Contains(ByVal key As String) As Boolean
            Return _innerHash.Contains(key)

        End Function

        Public Function Contains(ByVal key As Object) As Boolean Implements IDictionary.Contains
            Return Contains(CType(key, String))
        End Function

        Public Sub Clear() Implements IDictionary.Clear
            _innerHash.Clear()
        End Sub

        Public Sub Add(ByVal key As String, ByVal value As Integer)
            _innerHash.Add(key, value)
        End Sub

        Public Sub Add(ByVal key As Object, ByVal value As Object) Implements IDictionary.Add
            Add(CType(key, String), CType(value, Integer))
        End Sub

        Public ReadOnly Property IsReadOnly() As Boolean Implements IDictionary.IsReadOnly
            Get
                Return _innerHash.IsReadOnly
            End Get
        End Property

        Default Public Property Item(ByVal key As String) As Integer
            Get
                Return CType(_innerHash(key), Integer)
            End Get
            Set(ByVal Value As Integer)
                _innerHash(key) = value
            End Set
        End Property


        Default Public Property Item(ByVal key As Object) As Object Implements IDictionary.Item
            Get
                Return item(CType(key, String))
            End Get
            Set(ByVal Value As Object)
                item(CType(key, String)) = CType(value, Integer)
            End Set
        End Property

        Public ReadOnly Property Values() As System.Collections.ICollection Implements IDictionary.Values
            Get
                Return _innerHash.Values
            End Get
        End Property

        Public ReadOnly Property Keys() As System.Collections.ICollection Implements IDictionary.Keys
            Get
                Return _innerHash.Keys
            End Get
        End Property

        Public ReadOnly Property IsFixedSize() As Boolean Implements IDictionary.IsFixedSize
            Get
                Return _innerHash.IsFixedSize
            End Get
        End Property

#End Region

#Region "Implementation of ICollection"

        Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements ICollection.CopyTo
            _innerHash.CopyTo(array, index)
        End Sub

        Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
            Get
                Return _innerHash.IsSynchronized
            End Get
        End Property

        Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
            Get
                Return _innerHash.Count
            End Get
        End Property

        Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
            Get
                Return _innerHash.SyncRoot
            End Get
        End Property

#End Region

#Region "Implementation of ICloneable"

        Public Function Clone() As StringIntegerHashtable
            Dim innerClone As StringIntegerHashtable = New StringIntegerHashtable()
            innerClone._innerHash = CType(_innerHash.Clone(), Hashtable)

            Return innerClone
        End Function

        Public Function ICloneable_Clone() As Object Implements ICloneable.Clone
            Return Clone()
        End Function
#End Region

#Region "HashTable Methods"
        Public Function ContainsKey(ByVal key As String) As Boolean
            Return _innerHash.ContainsKey(key)
        End Function

        Public Function ContainsValue(ByVal value As Integer) As Boolean
            Return _innerHash.ContainsValue(value)
        End Function

        Public Shared Function Synchronized(ByVal nonSync As StringIntegerHashtable) As StringIntegerHashtable
            Dim sync As StringIntegerHashtable = New StringIntegerHashtable()
            sync._innerHash = Hashtable.Synchronized(nonSync.innerHash)
            Return sync
        End Function
#End Region

        Friend ReadOnly Property InnerHash() As Hashtable
            Get
                Return _innerHash
            End Get
        End Property
    End Class

    Public Class StringIntegerHashtableEnumerator
        Implements IDictionaryEnumerator

        Private innerEnumerator As IDictionaryEnumerator

        Friend Sub New(ByVal enumerable As StringIntegerHashtable)
            innerEnumerator = enumerable.InnerHash.GetEnumerator()
        End Sub

#Region "Implementation of IDictionaryEnumerator"
        Public ReadOnly Property Key() As String
            Get
                Return CType(innerEnumerator.Key, String)
            End Get
        End Property

        Public ReadOnly Property IDictionaryEnumerator_Key() As Object Implements IDictionaryEnumerator.Key
            Get
                Return Key
            End Get
        End Property

        Public ReadOnly Property Value() As Integer
            Get
                Return CType(innerEnumerator.Value, Integer)
            End Get
        End Property

        Public ReadOnly Property IDictionaryEnumerator_Value() As Object Implements IDictionaryEnumerator.Value
            Get
                Return Value
            End Get
        End Property

        Public ReadOnly Property Entry() As System.Collections.DictionaryEntry Implements IDictionaryEnumerator.Entry
            Get
                Return innerEnumerator.Entry
            End Get
        End Property
#End Region

#Region "Implementation of IEnumerator"
        Public Sub Reset() Implements IDictionaryEnumerator.Reset
            innerEnumerator.Reset()
        End Sub

        Public Function MoveNext() As Boolean Implements IDictionaryEnumerator.MoveNext
            Return innerEnumerator.MoveNext()
        End Function

        Public ReadOnly Property Current() As Object Implements IDictionaryEnumerator.Current
            Get
                Return innerEnumerator.Current
            End Get
        End Property
#End Region
    End Class
	#End Region
	#Region "IntegerHashtable"
    Public Class IntegerHashtable
        Implements IDictionary
        Implements ICollection
        Implements IEnumerable
        Implements ICloneable
        Protected _innerHash As Hashtable

#Region "Constructors"
        Public Sub New()
            _innerHash = New Hashtable()
        End Sub

        Public Sub New(ByVal original As IntegerHashtable)
            _innerHash = New Hashtable(original.innerHash)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary)
            _innerHash = New Hashtable(dictionary)
        End Sub

        Public Sub New(ByVal capacity As Integer)
            _innerHash = New Hashtable(capacity)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary, ByVal loadFactor As Single)
            _innerHash = New Hashtable(dictionary, loadFactor)
        End Sub

        Public Sub New(ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(codeProvider, comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer, ByVal loadFactor As Integer)
            _innerHash = New Hashtable(capacity, loadFactor)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(dictionary, codeProvider, comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(capacity, codeProvider, comparer)
        End Sub

        Public Sub New(ByVal dictionary As IDictionary, ByVal loadFactor As Single, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(dictionary, loadFactor, codeProvider, comparer)
        End Sub

        Public Sub New(ByVal capacity As Integer, ByVal loadFactor As Integer, ByVal codeProvider As IHashCodeProvider, ByVal comparer As IComparer)
            _innerHash = New Hashtable(capacity, loadFactor, codeProvider, comparer)
        End Sub


#End Region

#Region "Implementation of IDictionary"

        Private Function _GetEnumerator() As IDictionaryEnumerator
            Return New IntegerHashtableEnumerator(Me)
        End Function

        Public Function IDictionary_GetEnumerator() As IDictionaryEnumerator Implements IDictionary.GetEnumerator
            Return _GetEnumerator()
        End Function

        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _GetEnumerator()
        End Function

        Public Sub Remove(ByVal key As Integer)
            _innerHash.Remove(key)
        End Sub

        Public Sub Remove(ByVal key As Object) Implements IDictionary.Remove
            Remove(CType(key, Integer))
        End Sub

        Public Function Contains(ByVal key As Integer) As Boolean
            Return _innerHash.Contains(key)

        End Function

        Public Function Contains(ByVal key As Object) As Boolean Implements IDictionary.Contains
            Return Contains(CType(key, Integer))
        End Function

        Public Sub Clear() Implements IDictionary.Clear
            _innerHash.Clear()
        End Sub

        Public Sub Add(ByVal key As Integer, ByVal value As Integer)
            _innerHash.Add(key, value)
        End Sub

        Public Sub Add(ByVal key As Object, ByVal value As Object) Implements IDictionary.Add
            Add(CType(key, Integer), CType(value, Integer))
        End Sub

        Public ReadOnly Property IsReadOnly() As Boolean Implements IDictionary.IsReadOnly
            Get
                Return _innerHash.IsReadOnly
            End Get
        End Property

        Default Public Property Item(ByVal key As Integer) As Integer
            Get
                Return CType(_innerHash(key), Integer)
            End Get
            Set(ByVal Value As Integer)
                _innerHash(key) = value
            End Set
        End Property


        Default Public Property Item(ByVal key As Object) As Object Implements IDictionary.Item
            Get
                Return item(CType(key, Integer))
            End Get
            Set(ByVal Value As Object)
                item(CType(key, Integer)) = CType(value, Integer)
            End Set
        End Property

        Public ReadOnly Property Values() As System.Collections.ICollection Implements IDictionary.Values
            Get
                Return _innerHash.Values
            End Get
        End Property

        Public ReadOnly Property Keys() As System.Collections.ICollection Implements IDictionary.Keys
            Get
                Return _innerHash.Keys
            End Get
        End Property

        Public ReadOnly Property IsFixedSize() As Boolean Implements IDictionary.IsFixedSize
            Get
                Return _innerHash.IsFixedSize
            End Get
        End Property

#End Region

#Region "Implementation of ICollection"

        Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements ICollection.CopyTo
            _innerHash.CopyTo(array, index)
        End Sub

        Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
            Get
                Return _innerHash.IsSynchronized
            End Get
        End Property

        Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
            Get
                Return _innerHash.Count
            End Get
        End Property

        Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
            Get
                Return _innerHash.SyncRoot
            End Get
        End Property

#End Region

#Region "Implementation of ICloneable"

        Public Function Clone() As IntegerHashtable
            Dim innerClone As IntegerHashtable = New IntegerHashtable()
            innerClone._innerHash = CType(_innerHash.Clone(), Hashtable)

            Return innerClone
        End Function

        Public Function ICloneable_Clone() As Object Implements ICloneable.Clone
            Return Clone()
        End Function
#End Region

#Region "HashTable Methods"
        Public Function ContainsKey(ByVal key As Integer) As Boolean
            Return _innerHash.ContainsKey(key)
        End Function

        Public Function ContainsValue(ByVal value As Integer) As Boolean
            Return _innerHash.ContainsValue(value)
        End Function

        Public Shared Function Synchronized(ByVal nonSync As IntegerHashtable) As IntegerHashtable
            Dim sync As IntegerHashtable = New IntegerHashtable()
            sync._innerHash = Hashtable.Synchronized(nonSync.innerHash)
            Return sync
        End Function
#End Region

        Friend ReadOnly Property InnerHash() As Hashtable
            Get
                Return _innerHash
            End Get
        End Property
    End Class

    Public Class IntegerHashtableEnumerator
        Implements IDictionaryEnumerator

        Private innerEnumerator As IDictionaryEnumerator

        Friend Sub New(ByVal enumerable As IntegerHashtable)
            innerEnumerator = enumerable.InnerHash.GetEnumerator()
        End Sub

#Region "Implementation of IDictionaryEnumerator"
        Public ReadOnly Property Key() As Integer
            Get
                Return CType(innerEnumerator.Key, Integer)
            End Get
        End Property

        Public ReadOnly Property IDictionaryEnumerator_Key() As Object Implements IDictionaryEnumerator.Key
            Get
                Return Key
            End Get
        End Property

        Public ReadOnly Property Value() As Integer
            Get
                Return CType(innerEnumerator.Value, Integer)
            End Get
        End Property

        Public ReadOnly Property IDictionaryEnumerator_Value() As Object Implements IDictionaryEnumerator.Value
            Get
                Return Value
            End Get
        End Property

        Public ReadOnly Property Entry() As System.Collections.DictionaryEntry Implements IDictionaryEnumerator.Entry
            Get
                Return innerEnumerator.Entry
            End Get
        End Property
#End Region

#Region "Implementation of IEnumerator"
        Public Sub Reset() Implements IDictionaryEnumerator.Reset
            innerEnumerator.Reset()
        End Sub

        Public Function MoveNext() As Boolean Implements IDictionaryEnumerator.MoveNext
            Return innerEnumerator.MoveNext()
        End Function

        Public ReadOnly Property Current() As Object Implements IDictionaryEnumerator.Current
            Get
                Return innerEnumerator.Current
            End Get
        End Property
#End Region
    End Class
	#End Region
