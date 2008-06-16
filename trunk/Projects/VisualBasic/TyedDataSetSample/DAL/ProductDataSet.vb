
Imports System 
Imports System.IO 
Imports System.Collections 
Imports System.Data 
Imports System.Data.SqlClient 
Imports System.ComponentModel 
Imports System.ComponentModel.Design 
Imports System.Xml 
Imports System.Xml.Schema 
Imports System.Runtime.Serialization 
NameSpace TypedDataSetTester

#Region "ProductDataSet" 
<Serializable()> _ 
<DesignerCategoryAttribute("code")> _ 
<System.Diagnostics.DebuggerStepThrough()> _ 
<ToolboxItem(True)> _ 
Public Class ProductDataSet 
    Inherits DataSet 
    Private _tableProduct As ProductDataTable 
    
    <DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)> _ 
    Public ReadOnly Property Product() As ProductDataTable 
        Get 
            Return Me._tableProduct 
        End Get 
    End Property 
    
    Public Sub New() 
        Me.InitClass() 
    End Sub 
    
    Protected Overloads Overrides Function GetSchemaSerializable() As XmlSchema 
        Dim stream As New MemoryStream() 
        Me.WriteXmlSchema(New XmlTextWriter(stream, Nothing)) 
        stream.Position = 0 
        Return XmlSchema.Read(New XmlTextReader(stream), Nothing) 
    End Function 
    
    Protected Overloads Overrides Sub ReadXmlSerializable(ByVal reader As XmlReader) 
        Me.Reset() 
        Dim ds As New DataSet() 
        ds.ReadXml(reader) 
        If (ds.Tables("Product") IsNot Nothing) Then 
            Me.Tables.Add(New ProductDataTable(ds.Tables("Product"))) 
        End If 
        Me.DataSetName = ds.DataSetName 
        Me.Prefix = ds.Prefix 
        Me.[Namespace] = ds.[Namespace] 
        Me.Locale = ds.Locale 
        Me.CaseSensitive = ds.CaseSensitive 
        Me.EnforceConstraints = ds.EnforceConstraints 
        Me.Merge(ds, False, System.Data.MissingSchemaAction.Add) 
        Me.InitVars() 
    End Sub 
    
    Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext) 
        Dim strSchema As String = DirectCast((info.GetValue("XmlSchema", GetType(String))), String) 
        If (strSchema IsNot Nothing) Then 
            Dim ds As New DataSet() 
            ds.ReadXmlSchema(New XmlTextReader(New System.IO.StringReader(strSchema))) 
            If (ds.Tables("Product") IsNot Nothing) Then 
                Me.Tables.Add(New ProductDataTable(ds.Tables("Product"))) 
            End If 
            Me.DataSetName = ds.DataSetName 
            Me.Prefix = ds.Prefix 
            Me.[Namespace] = ds.[Namespace] 
            Me.Locale = ds.Locale 
            Me.CaseSensitive = ds.CaseSensitive 
            Me.EnforceConstraints = ds.EnforceConstraints 
            Me.Merge(ds, False, System.Data.MissingSchemaAction.Add) 
            Me.InitVars() 
        Else 
            Me.InitClass() 
        End If 
        Me.GetSerializationData(info, context) 
    End Sub 
    
    Private Sub InitClass() 
        Me.DataSetName = "ProductDataSet" 
        _tableProduct = New ProductDataTable() 
        Me.Tables.Add(_tableProduct) 
        Me.ExtendedProperties.Add("DataAdapterName", "ProductDataAdapter") 
        Me.ExtendedProperties.Add("ObjectName", "Product") 
        Me.ExtendedProperties.Add("ObjectDescription", "Product") 
        Me.ExtendedProperties.Add("NameSpace", "") 
    End Sub 
    
    Public Overloads Overrides Function Clone() As DataSet 
        Dim cln As ProductDataSet = DirectCast((MyBase.Clone()), ProductDataSet) 
        cln.InitVars() 
        Return cln 
    End Function 
    
    Friend Sub InitVars() 
        _tableProduct = DirectCast((Me.Tables("Product")), ProductDataTable) 
        If _tableProduct IsNot Nothing Then 
            _tableProduct.InitVars() 
        End If 
    End Sub 
    
    Protected Overloads Overrides Function ShouldSerializeTables() As Boolean 
        Return False 
    End Function 
    
    Protected Overloads Overrides Function ShouldSerializeRelations() As Boolean 
        Return False 
    End Function 
    
    Private Function ShouldSerializeProduct() As Boolean 
        Return False 
    End Function 
   
    Public Delegate Sub ProductRowChangeEventHandler(ByVal sender As Object, ByVal e As ProductRowChangeEventArgs) 
    
    <Serializable()> _ 
    Public Class ProductDataTable 
        Inherits DataTable 
        Implements System.Collections.IEnumerable 
			Private _columnProductId AS DataColumn
			Private _columnCategoryId AS DataColumn
			Private _columnName AS DataColumn
			Private _columnDescn AS DataColumn
			Private _columnImage AS DataColumn
        
        Friend Sub New() 
            MyBase.New("Product") 
            Me.InitClass() 
        End Sub 
        
        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext) 
            MyBase.New(info, context) 
            Me.InitVars() 
        End Sub 
        
        Friend Sub New(ByVal table As DataTable) 
            MyBase.New(table.TableName) 
            If table.CaseSensitive <> table.DataSet.CaseSensitive Then 
                Me.CaseSensitive = table.CaseSensitive 
            End If 
            If table.Locale.ToString() <> table.DataSet.Locale.ToString() Then 
                Me.Locale = table.Locale 
            End If 
            If table.[Namespace] <> table.DataSet.[Namespace] Then 
                Me.[Namespace] = table.[Namespace] 
            End If 
            Me.Prefix = table.Prefix 
            Me.MinimumCapacity = table.MinimumCapacity 
            Me.DisplayExpression = table.DisplayExpression 
        End Sub 
        
        Public ReadOnly Property Count() As Integer 
            Get 
                Return Me.Rows.Count 
            End Get 
        End Property 
        
		Public ReadOnly Property ProductIdColumn
			get
				return _columnProductId
			END GET
		END Property
			
		Public ReadOnly Property CategoryIdColumn
			get
				return _columnCategoryId
			END GET
		END Property
			
		Public ReadOnly Property NameColumn
			get
				return _columnName
			END GET
		END Property
			
		Public ReadOnly Property DescnColumn
			get
				return _columnDescn
			END GET
		END Property
			
		Public ReadOnly Property ImageColumn
			get
				return _columnImage
			END GET
		END Property
			
   
        Public Default ReadOnly Property Item(ByVal index As Integer) As ProductRow 
            Get 
                Return DirectCast((Me.Rows(index)), ProductRow) 
            End Get 
        End Property 
        
        Public Event ProductRowChanged As ProductRowChangeEventHandler 
        Public Event ProductRowChanging As ProductRowChangeEventHandler 
        Public Event ProductRowDeleted As ProductRowChangeEventHandler 
        Public Event ProductRowDeleting As ProductRowChangeEventHandler 
        
        Public Sub AddProductRow(ByVal row As ProductRow) 
            Me.Rows.Add(row) 
        End Sub 
       
        Public Function AddProductRow( _
				ByVal productId AS String, _
							ByVal categoryId AS String, _
							ByVal name AS String, _
							ByVal descn AS String, _
							ByVal image AS String _
			) AS ProductRow
            Dim rowProductRow As ProductRow = DirectCast((Me.NewRow()), ProductRow) 
				rowProductRow("ProductId") = productId
				rowProductRow("CategoryId") = categoryId
				rowProductRow("Name") = name
				rowProductRow("Descn") = descn
				rowProductRow("Image") = image
            Me.Rows.Add(rowProductRow) 
            Return rowProductRow 
        End Function 
        
			Public FUNCTION FindByProductId(ByVal productId AS String) AS ProductRow
				return (DirectCast((Me.Rows.Find(new object() {productId})),ProductRow))
			END Function
			
        
        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me.Rows.GetEnumerator() 
        End Function 
        
        Public Overloads Overrides Function Clone() As DataTable 
            Dim cln As ProductDataTable = DirectCast((MyBase.Clone()), ProductDataTable) 
            cln.InitVars() 
            Return cln 
        End Function 
        
        Friend Sub InitVars() 
				_columnProductId = Me.Columns("ProductId")
				_columnCategoryId = Me.Columns("CategoryId")
				_columnName = Me.Columns("Name")
				_columnDescn = Me.Columns("Descn")
				_columnImage = Me.Columns("Image")
        End Sub 
        
        Public Sub InitClass() 
				_columnProductId = new DataColumn("ProductId", GetType(String), "", MappingType.Element)
				_columnProductId.AllowDBNull = false
				_columnProductId.Caption = "Product Id"
				_columnProductId.MaxLength = 10
				_columnProductId.Unique = true
				_columnProductId.DefaultValue = Convert.DBNull
				_columnProductId.ExtendedProperties.Add("IsKey", "true")
				_columnProductId.ExtendedProperties.Add("ReadOnly", "false")
				_columnProductId.ExtendedProperties.Add("Description", "Product Id")
				_columnProductId.ExtendedProperties.Add("Length", "10")
				_columnProductId.ExtendedProperties.Add("Decimals", "0")
				_columnProductId.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnProductId)
				
				_columnCategoryId = new DataColumn("CategoryId", GetType(String), "", MappingType.Element)
				_columnCategoryId.AllowDBNull = false
				_columnCategoryId.Caption = "Category Id"
				_columnCategoryId.MaxLength = 10
				_columnCategoryId.Unique = false
				_columnCategoryId.DefaultValue = Convert.DBNull
				_columnCategoryId.ExtendedProperties.Add("IsKey", "false")
				_columnCategoryId.ExtendedProperties.Add("ReadOnly", "false")
				_columnCategoryId.ExtendedProperties.Add("Description", "Category Id")
				_columnCategoryId.ExtendedProperties.Add("Length", "10")
				_columnCategoryId.ExtendedProperties.Add("Decimals", "0")
				_columnCategoryId.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnCategoryId)
				
				_columnName = new DataColumn("Name", GetType(String), "", MappingType.Element)
				_columnName.AllowDBNull = true
				_columnName.Caption = "Name"
				_columnName.MaxLength = 80
				_columnName.Unique = false
				_columnName.DefaultValue = Convert.DBNull
				_columnName.ExtendedProperties.Add("IsKey", "false")
				_columnName.ExtendedProperties.Add("ReadOnly", "false")
				_columnName.ExtendedProperties.Add("Description", "Name")
				_columnName.ExtendedProperties.Add("Length", "80")
				_columnName.ExtendedProperties.Add("Decimals", "0")
				_columnName.ExtendedProperties.Add("AllowDBNulls", "true")
				Me.Columns.Add(_columnName)
				
				_columnDescn = new DataColumn("Descn", GetType(String), "", MappingType.Element)
				_columnDescn.AllowDBNull = true
				_columnDescn.Caption = "Descn"
				_columnDescn.MaxLength = 255
				_columnDescn.Unique = false
				_columnDescn.DefaultValue = Convert.DBNull
				_columnDescn.ExtendedProperties.Add("IsKey", "false")
				_columnDescn.ExtendedProperties.Add("ReadOnly", "false")
				_columnDescn.ExtendedProperties.Add("Description", "Descn")
				_columnDescn.ExtendedProperties.Add("Length", "255")
				_columnDescn.ExtendedProperties.Add("Decimals", "0")
				_columnDescn.ExtendedProperties.Add("AllowDBNulls", "true")
				Me.Columns.Add(_columnDescn)
				
				_columnImage = new DataColumn("Image", GetType(String), "", MappingType.Element)
				_columnImage.AllowDBNull = true
				_columnImage.Caption = "Image"
				_columnImage.MaxLength = 80
				_columnImage.Unique = false
				_columnImage.DefaultValue = Convert.DBNull
				_columnImage.ExtendedProperties.Add("IsKey", "false")
				_columnImage.ExtendedProperties.Add("ReadOnly", "false")
				_columnImage.ExtendedProperties.Add("Description", "Image")
				_columnImage.ExtendedProperties.Add("Length", "80")
				_columnImage.ExtendedProperties.Add("Decimals", "0")
				_columnImage.ExtendedProperties.Add("AllowDBNulls", "true")
				Me.Columns.Add(_columnImage)
				
				Me.PrimaryKey = new DataColumn() {_columnProductId}
        End Sub 
        
        Public Function NewProductRow() As ProductRow 
            Dim rowProductRow As ProductRow = DirectCast((Me.NewRow()), ProductRow) 
            Return rowProductRow 
        End Function 
        
        Protected Overloads Overrides Function NewRowFromBuilder(ByVal builder As DataRowBuilder) As DataRow 
            Return New ProductRow(builder) 
        End Function 
        
        Protected Overloads Overrides Function GetRowType() As Type 
            Return GetType(ProductRow) 
        End Function 
        
        Protected Overloads Overrides Sub OnRowChanged(ByVal e As DataRowChangeEventArgs) 
            MyBase.OnRowChanged(e) 
                RaiseEvent ProductRowChanged(Me, New ProductRowChangeEventArgs(DirectCast((e.Row), ProductRow), e.Action)) 
        End Sub 
        
        Protected Overloads Overrides Sub OnRowChanging(ByVal e As DataRowChangeEventArgs) 
            MyBase.OnRowChanging(e)
                RaiseEvent ProductRowChanging(Me, New ProductRowChangeEventArgs(DirectCast((e.Row), ProductRow), e.Action)) 
        End Sub 
        
        Protected Overloads Overrides Sub OnRowDeleted(ByVal e As DataRowChangeEventArgs) 
            MyBase.OnRowDeleted(e) 
            RaiseEvent ProductRowDeleted(Me, New ProductRowChangeEventArgs(DirectCast((e.Row), ProductRow), e.Action)) 
        End Sub 
        
        Protected Overloads Overrides Sub OnRowDeleting(ByVal e As DataRowChangeEventArgs) 
            MyBase.OnRowDeleting(e) 
            RaiseEvent ProductRowDeleting(Me, New ProductRowChangeEventArgs(DirectCast((e.Row), ProductRow), e.Action)) 
        End Sub 
        
        Public Sub RemoveProductRow(ByVal row As ProductRow) 
            Me.Rows.Remove(row) 
        End Sub 
    End Class 
   
    Public Class ProductRow 
        Inherits DataRow 
        Private _tableProduct As ProductDataTable 
        
        Friend Sub New(ByVal rb As DataRowBuilder) 
            MyBase.New(rb) 
            _tableProduct = DirectCast((Me.Table), ProductDataTable) 
        End Sub 
        
		''' <summary>
		''' Gets or sets the value of ProductId property
		''' </summary>
		Public Property ProductId As String
			get
				try
					return (DirectCast((Me(_tableProduct.ProductIdColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value ProductId because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableProduct.ProductIdColumn) = value
			END SET
		End Property
			
		Public Function IsProductIdNull() AS Boolean
			return Me.IsNull(_tableProduct.ProductIdColumn)
		END Function
			
		public Sub SetProductIdNull()
			Me(_tableProduct.ProductIdColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of CategoryId property
		''' </summary>
		Public Property CategoryId As String
			get
				try
					return (DirectCast((Me(_tableProduct.CategoryIdColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value CategoryId because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableProduct.CategoryIdColumn) = value
			END SET
		End Property
			
		Public Function IsCategoryIdNull() AS Boolean
			return Me.IsNull(_tableProduct.CategoryIdColumn)
		END Function
			
		public Sub SetCategoryIdNull()
			Me(_tableProduct.CategoryIdColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of Name property
		''' </summary>
		Public Property Name As String
			get
				try
					return (DirectCast((Me(_tableProduct.NameColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value Name because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableProduct.NameColumn) = value
			END SET
		End Property
			
		Public Function IsNameNull() AS Boolean
			return Me.IsNull(_tableProduct.NameColumn)
		END Function
			
		public Sub SetNameNull()
			Me(_tableProduct.NameColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of Descn property
		''' </summary>
		Public Property Descn As String
			get
				try
					return (DirectCast((Me(_tableProduct.DescnColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value Descn because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableProduct.DescnColumn) = value
			END SET
		End Property
			
		Public Function IsDescnNull() AS Boolean
			return Me.IsNull(_tableProduct.DescnColumn)
		END Function
			
		public Sub SetDescnNull()
			Me(_tableProduct.DescnColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of Image property
		''' </summary>
		Public Property Image As String
			get
				try
					return (DirectCast((Me(_tableProduct.ImageColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value Image because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableProduct.ImageColumn) = value
			END SET
		End Property
			
		Public Function IsImageNull() AS Boolean
			return Me.IsNull(_tableProduct.ImageColumn)
		END Function
			
		public Sub SetImageNull()
			Me(_tableProduct.ImageColumn) = Convert.DBNull
		END SUB
			
    End Class 
    
    Public Class ProductRowChangeEventArgs 
        Inherits EventArgs 
        Private _eventRow As ProductRow 
        Private _eventAction As System.Data.DataRowAction 
        
        Public Sub New(ByVal row As ProductRow, ByVal action As DataRowAction) 
            _eventRow = row 
            _eventAction = action 
        End Sub 
        
        Public ReadOnly Property Row() As ProductRow 
            Get 
                Return _eventRow 
            End Get 
        End Property 
        
        Public ReadOnly Property Action() As DataRowAction 
            Get 
                Return _eventAction 
            End Get 
        End Property 
    End Class 
End Class 
#End Region 

#Region "ProductDataAdapter" 
Public Class ProductDataAdapter 
    Inherits MarshalByRefObject 
    Implements IDataAdapter 
    #Region "Member Variables" 
    Private _connection As IDbConnection 
    Private _transaction As IDbTransaction 
    Private _command As IDbCommand 
    Private _reader As IDataReader 
    Private _connectionTimeout As Integer = 30 
    Private _commandTimeout As Integer = 30 
    Private _connectionStringKey As String 
    Private _autoCloseConnection As Boolean = True 
    Private _autoCommitTransaction As Boolean = True 
    Private _convertEmptyValuesToDBNull As Boolean = True 
    'private IDataParameter() _fillDataParameters 
    #End Region 
    
    #Region "Constructors" 
    Public Sub New() 
        _connectionStringKey = "ConnectionString" 
    End Sub 
    
    Public Sub New(ByVal connectionStringKey As String) 
        _connectionStringKey = connectionStringKey + "_ConnectionString" 
    End Sub 
    
    Public Sub New(ByVal connection As IDbConnection) 
        Me.Connection = connection 
    End Sub 
    
    Public Sub New(ByVal transaction As IDbTransaction) 
        Me.Transaction = transaction 
    End Sub 
    #End Region 
    
    #Region "Properties" 
    Public Property Connection() As IDbConnection 
        Get 
            If _connection Is Nothing Then 
                _connection = New SqlConnection() 
                    '_connection.ConnectionTimeout = Me.ConnectionTimeout 
                _connection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings(Me.ConnectionStringKey) 
            End If 
            Return _connection 
        End Get 
        Set 
            _connection = value 
            ' if the connection is passed in then it should be up to the owner to close the connection. 
            _autoCloseConnection = False 
        End Set 
    End Property 
    
    Public Property Transaction() As IDbTransaction 
        Get 
            Return _transaction 
        End Get 
        Set 
            _transaction = value 
            _connection = _transaction.Connection 
            ' if the connection is passed in then it should be up to the owner to close the connection. 
            _autoCloseConnection = False 
            ' if the transaction is passed in then it should be up to the owner of the transaction to commit. 
            _autoCommitTransaction = False 
        End Set 
    End Property 
    
    Public Property AutoCloseConnection() As Boolean 
        Get 
            Return _autoCloseConnection 
        End Get 
        Set 
            _autoCloseConnection = value 
        End Set 
    End Property 
    
    Public Property AutoCommitTransaction() As Boolean 
        Get 
            Return _autoCommitTransaction 
        End Get 
        Set 
            _autoCommitTransaction = value 
        End Set 
    End Property 
    
    Public Property ConvertEmptyValuesToDBNull() As Boolean 
        Get 
            Return _convertEmptyValuesToDBNull 
        End Get 
        Set 
            _convertEmptyValuesToDBNull = value 
        End Set 
    End Property 
    
    Public ReadOnly Property ConnectionStringKey() As String 
        Get 
            Return _connectionStringKey 
        End Get 
    End Property 
    
    Public Property ConnectionTimeout() As Integer 
        Get 
            Return _connectionTimeout 
        End Get 
        Set 
            _connectionTimeout = value 
        End Set 
    End Property 
    
    Public Property CommandTimeout() As Integer 
        Get 
            Return _commandTimeout 
        End Get 
        Set 
            _commandTimeout = value 
        End Set 
    End Property 
    
    Public Property MissingMappingAction() As MissingMappingAction Implements IDataAdapter.MissingMappingAction
        Get 
            Return MissingMappingAction.Passthrough 
        End Get 
        Set 
        End Set 
    End Property 
    
    Public Property MissingSchemaAction() As MissingSchemaAction Implements IDataAdapter.MissingSchemaAction
        Get 
            Return MissingSchemaAction.Ignore 
        End Get 
        Set 
        End Set 
    End Property 
    
    Public ReadOnly Property TableMappings() As ITableMappingCollection Implements IDataAdapter.TableMappings
        Get 
            Dim tableMaps As System.Data.Common.DataTableMappingCollection 
            tableMaps = New System.Data.Common.DataTableMappingCollection() 
            Return tableMaps 
        End Get 
    End Property 
    #End Region 
    
    #Region "Helper Methods" 
    Private Function GetCommand() As IDbCommand 
        If Me.Connection IsNot Nothing Then 
            _command = Me.Connection.CreateCommand() 
            _command.CommandTimeout = Me.CommandTimeout 
            _command.CommandType = CommandType.Text 
            _command.Connection = Me.Connection 
            If _transaction IsNot Nothing Then 
                _command.Transaction = _transaction 
            End If 
            
            Return _command 
        Else 
            Throw New InvalidOperationException("You must have a valid Connection object before calling GetCommand.") 
        End If 
    End Function 
    
    Private Sub OpenConnection() 
        If Me.Connection IsNot Nothing Then 
            If Me.Connection.State = ConnectionState.Closed Then 
                _connection.Open() 
            End If 
        Else 
            Throw New InvalidOperationException("You must have a valid Connection object before calling GetCommand.") 
        End If 
    End Sub 
    
    Private Sub Cleanup() 
        Try 
            If _reader IsNot Nothing Then 
                If Not _reader.IsClosed Then 
                    _reader.Close() 
                End If 
                _reader.Dispose() 
                _reader = Nothing 
            End If 
            
            If _command IsNot Nothing Then 
                _command.Dispose() 
                _command = Nothing 
            End If 
            
            If _connection IsNot Nothing AndAlso Me.AutoCloseConnection = True Then 
                If _connection.State = ConnectionState.Open Then 
                    _connection.Close() 
                End If 
                _connection.Dispose() 
                _connection = Nothing 
            End If 
        Catch 
        End Try 
    End Sub 
    #End Region 
    
    #Region "CreateParameter" 
    Public Function CreateParameter(ByVal name As String, ByVal type As DbType, ByVal value As Object) As IDbDataParameter 
        Dim prm As IDbDataParameter = _command.CreateParameter() 
        prm.Direction = ParameterDirection.Input 
        prm.ParameterName = name 
        prm.DbType = type 
        prm.Value = Me.PrepareParameterValue(value) 
        
        Return prm 
    End Function 
    
    Public Function CreateParameter(ByVal name As String, ByVal type As DbType, ByVal value As Object, ByVal size As Integer) As IDbDataParameter 
        Dim prm As IDbDataParameter = _command.CreateParameter() 
        prm.Direction = ParameterDirection.Input 
        prm.ParameterName = name 
        prm.DbType = type 
        prm.Size = size 
        prm.Value = Me.PrepareParameterValue(value) 
        
        Return prm 
    End Function 
    
    Public Function CreateParameter(ByVal name As String, ByVal type As DbType, ByVal value As Object, ByVal direction As ParameterDirection) As IDbDataParameter 
        Dim prm As IDbDataParameter = _command.CreateParameter() 
        prm.Direction = direction 
        prm.ParameterName = name 
        prm.DbType = type 
        prm.Value = Me.PrepareParameterValue(value) 
        
        Return prm 
    End Function 
    
    Public Function CreateParameter(ByVal name As String, ByVal type As DbType, ByVal value As Object, ByVal size As Integer, ByVal direction As ParameterDirection) As IDbDataParameter 
        Dim prm As IDbDataParameter = _command.CreateParameter() 
        prm.Direction = direction 
        prm.ParameterName = name 
        prm.DbType = type 
        prm.Size = size 
        prm.Value = Me.PrepareParameterValue(value) 
        
        Return prm 
    End Function 
    
    Private Function PrepareParameterValue(ByVal value As Object) As Object 
        Return PrepareParameterValue(value, False) 
    End Function 
    
    Private Function PrepareParameterValue(ByVal value As Object, ByVal convertZeroToDBNull As Boolean) As Object 
        If Not _convertEmptyValuesToDBNull Then 
            Return value 
        End If 
        
        Select Case value.GetType().ToString() 
            Case "System.String" 
                If Convert.ToString(value) = [String].Empty Then 
                    Return DBNull.Value 
                Else 
                    Return value 
                End If 
            Case "System.Guid" 
                If New Guid(Convert.ToString(value)) = Guid.Empty Then 
                    Return DBNull.Value 
                Else 
                    Return value 
                End If 
            Case "System.DateTime" 
                If Convert.ToDateTime(value) = DateTime.MinValue Then 
                    Return DBNull.Value 
                Else 
                    Return value 
                End If 
            Case "System.Int16" 
                If Convert.ToInt16(value) = 0 Then 
                    If convertZeroToDBNull Then 
                        Return DBNull.Value 
                    Else 
                        Return value 
                    End If 
                Else 
                    Return value 
                End If 
            Case "System.Int32" 
                If Convert.ToInt32(value) = 0 Then 
                    If convertZeroToDBNull Then 
                        Return DBNull.Value 
                    Else 
                        Return value 
                    End If 
                Else 
                    Return value 
                End If 
            Case "System.Int64" 
                If Convert.ToInt64(value) = 0 Then 
                    If convertZeroToDBNull Then 
                        Return DBNull.Value 
                    Else 
                        Return value 
                    End If 
                Else 
                    Return value 
                End If 
            Case "System.Single" 
                If Convert.ToSingle(value) = 0 Then 
                    If convertZeroToDBNull Then 
                        Return DBNull.Value 
                    Else 
                        Return value 
                    End If 
                Else 
                    Return value 
                End If 
            Case "System.Double" 
                If Convert.ToDouble(value) = 0 Then 
                    If convertZeroToDBNull Then 
                        Return DBNull.Value 
                    Else 
                        Return value 
                    End If 
                Else 
                    Return value 
                End If 
            Case "System.Decimal" 
                If Convert.ToDecimal(value) = 0 Then 
                    If convertZeroToDBNull Then 
                        Return DBNull.Value 
                    Else 
                        Return value 
                    End If 
                Else 
                    Return value 
                End If 
            Case Else 
                Return value 
        End Select 
    End Function 
    #End Region 
    
    #Region "Fill Methods" 
    Public Function FillSchema(ByVal dataSet As DataSet, ByVal schemaType As SchemaType) As DataTable() Implements IDataAdapter.FillSchema
        Dim dataTables As DataTable() 
        dataTables = New DataTable(dataSet.Tables.Count - 1) {} 
        dataSet.Tables.CopyTo(dataTables, dataSet.Tables.Count) 
        Return dataTables 
    End Function 
    
    Public Function Fill(ByVal dataSet As ProductDataSet, ByVal dataRecord As IDataRecord) As Integer 
        Return Fill(dataSet, DirectCast(dataRecord("ProductId"),String))
    End Function 
    
    Public Function Fill(ByVal dataSet As ProductDataSet, ByVal dataRow As DataRow) As Integer 
        Return Fill(dataSet, DirectCast(dataRow("ProductId"),String)) 
    End Function 
    
    Public Function Fill(ByVal dataSet As ProductDataSet,  ByVal productId AS String) As Integer 
        Try 
            _command = Me.GetCommand() 
            _command.CommandText = _
					"SELECT " & _
						"[ProductId]," & _
						"[CategoryId]," & _
						"[Name]," & _
						"[Descn]," & _
						"[Image]" & _
					"FROM " & _
						"[Product]" & _
					" WHERE " & _
						"[ProductId] = @ProductId" & _
				_command.Parameters.Add(Me.CreateParameter("@ProductId", DbType.String, productId))
            Me.OpenConnection() 
            _reader = _command.ExecuteReader(CommandBehavior.CloseConnection Or CommandBehavior.SingleResult Or CommandBehavior.SingleRow) 
            If _reader.Read() Then 
                Dim row As ProductDataSet.ProductRow = dataSet.Product.NewProductRow() 
                Me.PopulateProductDataRow(_reader, row) 
                dataSet.Product.AddProductRow(row) 
                dataSet.AcceptChanges() 
                
                Return 1 
            Else 
                Throw New ProductNotFoundException() 
            End If 
        Catch e As Exception 
            System.Diagnostics.Debug.WriteLine(e.ToString()) 
            Return 0 
        Finally 
            Me.Cleanup() 
        End Try 
    End Function 
    
    Private Sub PopulateProductDataRow(ByVal reader As IDataReader, ByVal row As ProductDataSet.ProductRow) 
			IF NOT reader.IsDBNull(0) THEN 
			row.ProductId = reader.GetString(0) 
			END IF
			IF NOT reader.IsDBNull(1) THEN 
			row.CategoryId = reader.GetString(1) 
			END IF
			IF NOT reader.IsDBNull(2) THEN 
			row.Name = reader.GetString(2) 
			END IF
			IF NOT reader.IsDBNull(3) THEN 
			row.Descn = reader.GetString(3) 
			END IF
			IF NOT reader.IsDBNull(4) THEN 
			row.Image = reader.GetString(4) 
			END IF
    End Sub 
    
    Public Function Fill(ByVal dataSet As DataSet) As Integer Implements IDataAdapter.Fill
        Dim pageDataSet As ProductDataSet = TryCast(dataSet, ProductDataSet) 
        If pageDataSet IsNot Nothing Then 
            Return Me.Fill(pageDataSet) 
        Else 
            Throw New ApplicationException() 
        End If 
    End Function 
    
    Public Function Fill(ByVal dataSet As ProductDataSet, ByVal columns As String(), ByVal values As String(), ByVal types As DbType()) As Integer 
        Try 
            Dim recordcount As Integer = 0 
            _command = Me.GetCommand() 
            _command.CommandText = _
					"SELECT " & _
						"[ProductId], " & _
						"[CategoryId], " & _
						"[Name], " & _
						"[Descn], " & _
						"[Image] " & _
					"FROM " & _
						"[Product]" & _
					" WHERE "
            For i As Integer = 0 To columns.Length - 1 
                
                _command.CommandText += columns(i) + " = " + (IF(types(i) = DbType.AnsiString,"'" + values(i) + "'",values(i))) 
                If i < columns.Length - 1 Then 
                    _command.CommandText += " AND " 
                End If 
            Next 
            For i As Integer = 0 To columns.Length - 1 
                _command.Parameters.Add(Me.CreateParameter("@" + columns(i), types(i), columns(i))) 
            Next 
            Me.OpenConnection() 
            _reader = _command.ExecuteReader(CommandBehavior.CloseConnection Or CommandBehavior.SingleResult) 
            While _reader.Read() 
                Dim row As ProductDataSet.ProductRow = dataSet.Product.NewProductRow() 
                Me.PopulateProductDataRow(_reader, row) 
                dataSet.Product.AddProductRow(row) 
                
                recordcount += 1 
            End While 
            dataSet.AcceptChanges() 
            
            Return recordcount 
        Catch e As Exception 
            System.Diagnostics.Debug.WriteLine(e.ToString()) 
            Return 0 
        Finally 
            Me.Cleanup() 
        End Try 
    End Function 
    
    Public Function Fill(ByVal dataSet As ProductDataSet) As Integer 
        Try 
            Dim recordcount As Integer = 0 
            _command = Me.GetCommand() 
            _command.CommandText = _
					"SELECT " & _
						"[ProductId]," & _
						"[CategoryId]," & _
						"[Name]," & _
						"[Descn]," & _
						"[Image]" & _
					"FROM " & _
						"[Product]"
            Me.OpenConnection() 
            _reader = _command.ExecuteReader(CommandBehavior.CloseConnection Or CommandBehavior.SingleResult) 
            While _reader.Read() 
                Dim row As ProductDataSet.ProductRow = dataSet.Product.NewProductRow() 
                Me.PopulateProductDataRow(_reader, row) 
                dataSet.Product.AddProductRow(row) 
                
                recordcount += 1 
            End While 
            dataSet.AcceptChanges() 
            
            Return recordcount 
        Catch e As Exception 
            System.Diagnostics.Debug.WriteLine(e.ToString()) 
            Return 0 
        Finally 
            Me.Cleanup() 
        End Try 
    End Function 
		Public Function FillByName(ByVal dataSet As ProductDataSet, name AS String) AS Integer
			Try
				DIM recordcount AS INTEGER = 0
				_command = Me.GetCommand()
				_command.CommandText = _
					"SELECT " & _
						"[ProductId]," & _
						"[CategoryId]," & _
						"[Name]," & _
						"[Descn]," & _
						"[Image]" & _
					"FROM " & _
						"[Product] " & _
					" WHERE" & _
						"[Name] = @Name" & _
					" "
				_command.Parameters.Add(Me.CreateParameter("@Name", DbType.String, name))
				Me.OpenConnection()
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection OR CommandBehavior.SingleResult)
				while (_reader.Read())
					Dim row AS ProductDataSet.ProductRow = dataSet.Product.NewProductRow()
					Me.PopulateProductDataRow(_reader, row)
					dataSet.Product.AddProductRow(row)
					
					recordcount += 1
				END While
	
				dataSet.AcceptChanges()
				
				return recordcount
			
			CATCH e AS Exception
				System.Diagnostics.Debug.WriteLine(e.ToString())
				return 0
			
			FINALLY
				Me.Cleanup()
			END TRY
		END FUNCTION
		Public Function FillByCategoryId(ByVal dataSet As ProductDataSet, categoryId AS String) AS Integer
			Try
				DIM recordcount AS INTEGER = 0
				_command = Me.GetCommand()
				_command.CommandText = _
					"SELECT " & _
						"[ProductId]," & _
						"[CategoryId]," & _
						"[Name]," & _
						"[Descn]," & _
						"[Image]" & _
					"FROM " & _
						"[Product] " & _
					" WHERE" & _
						"[CategoryId] = @CategoryId" & _
					" "
				_command.Parameters.Add(Me.CreateParameter("@CategoryId", DbType.String, categoryId))
				Me.OpenConnection()
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection OR CommandBehavior.SingleResult)
				while (_reader.Read())
					Dim row AS ProductDataSet.ProductRow = dataSet.Product.NewProductRow()
					Me.PopulateProductDataRow(_reader, row)
					dataSet.Product.AddProductRow(row)
					
					recordcount += 1
				END While
	
				dataSet.AcceptChanges()
				
				return recordcount
			
			CATCH e AS Exception
				System.Diagnostics.Debug.WriteLine(e.ToString())
				return 0
			
			FINALLY
				Me.Cleanup()
			END TRY
		END FUNCTION
		Public Function FillByCategoryIdAndName(ByVal dataSet As ProductDataSet, categoryId AS String, name AS String) AS Integer
			Try
				DIM recordcount AS INTEGER = 0
				_command = Me.GetCommand()
				_command.CommandText = _
					"SELECT " & _
						"[ProductId]," & _
						"[CategoryId]," & _
						"[Name]," & _
						"[Descn]," & _
						"[Image]" & _
					"FROM " & _
						"[Product] " & _
					" WHERE" & _
						"[CategoryId] = @CategoryId" & _
						" AND [Name] = @Name" & _
					" "
				_command.Parameters.Add(Me.CreateParameter("@CategoryId", DbType.String, categoryId))
				_command.Parameters.Add(Me.CreateParameter("@Name", DbType.String, name))
				Me.OpenConnection()
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection OR CommandBehavior.SingleResult)
				while (_reader.Read())
					Dim row AS ProductDataSet.ProductRow = dataSet.Product.NewProductRow()
					Me.PopulateProductDataRow(_reader, row)
					dataSet.Product.AddProductRow(row)
					
					recordcount += 1
				END While
	
				dataSet.AcceptChanges()
				
				return recordcount
			
			CATCH e AS Exception
				System.Diagnostics.Debug.WriteLine(e.ToString())
				return 0
			
			FINALLY
				Me.Cleanup()
			END TRY
		END FUNCTION
		Public Function FillByCategoryIdAndProductIdAndName(ByVal dataSet As ProductDataSet, categoryId AS String, productId AS String, name AS String) AS Integer
			Try
				DIM recordcount AS INTEGER = 0
				_command = Me.GetCommand()
				_command.CommandText = _
					"SELECT " & _
						"[ProductId]," & _
						"[CategoryId]," & _
						"[Name]," & _
						"[Descn]," & _
						"[Image]" & _
					"FROM " & _
						"[Product] " & _
					" WHERE" & _
						"[CategoryId] = @CategoryId" & _
						" AND [ProductId] = @ProductId" & _
						" AND [Name] = @Name" & _
					" "
				_command.Parameters.Add(Me.CreateParameter("@CategoryId", DbType.String, categoryId))
				_command.Parameters.Add(Me.CreateParameter("@ProductId", DbType.String, productId))
				_command.Parameters.Add(Me.CreateParameter("@Name", DbType.String, name))
				Me.OpenConnection()
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection OR CommandBehavior.SingleResult)
				while (_reader.Read())
					Dim row AS ProductDataSet.ProductRow = dataSet.Product.NewProductRow()
					Me.PopulateProductDataRow(_reader, row)
					dataSet.Product.AddProductRow(row)
					
					recordcount += 1
				END While
	
				dataSet.AcceptChanges()
				
				return recordcount
			
			CATCH e AS Exception
				System.Diagnostics.Debug.WriteLine(e.ToString())
				return 0
			
			FINALLY
				Me.Cleanup()
			END TRY
		END FUNCTION
		Public Function FillByProductId(ByVal dataSet As ProductDataSet, productId AS String) AS Integer
			Try
				DIM recordcount AS INTEGER = 0
				_command = Me.GetCommand()
				_command.CommandText = _
					"SELECT " & _
						"[ProductId]," & _
						"[CategoryId]," & _
						"[Name]," & _
						"[Descn]," & _
						"[Image]" & _
					"FROM " & _
						"[Product] " & _
					" WHERE" & _
						"[ProductId] = @ProductId" & _
					" "
				_command.Parameters.Add(Me.CreateParameter("@ProductId", DbType.String, productId))
				Me.OpenConnection()
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection OR CommandBehavior.SingleResult)
				while (_reader.Read())
					Dim row AS ProductDataSet.ProductRow = dataSet.Product.NewProductRow()
					Me.PopulateProductDataRow(_reader, row)
					dataSet.Product.AddProductRow(row)
					
					recordcount += 1
				END While
	
				dataSet.AcceptChanges()
				
				return recordcount
			
			CATCH e AS Exception
				System.Diagnostics.Debug.WriteLine(e.ToString())
				return 0
			
			FINALLY
				Me.Cleanup()
			END TRY
		END FUNCTION
		
    Public Function GetFillParameters() As IDataParameter() Implements IDataAdapter.GetFillParameters
        ' not sure if I should create a ProductId parameter here or not. 
        Return Nothing 
        '_fillDataParameters 
    End Function 
    #End Region 
    
    #Region "Update Methods" 
    Public Function Update(ByVal dataSet As DataSet) As Integer Implements IDataAdapter.Update
        Dim pageDataSet As ProductDataSet = TryCast(dataSet, ProductDataSet) 
        If pageDataSet IsNot Nothing Then 
            Return Me.Update(pageDataSet) 
        Else 
            Throw New ApplicationException() 
        End If 
    End Function 
    
    Public Function Update(ByVal dataSet As ProductDataSet) As Integer 
        If dataSet IsNot Nothing Then 
            Try 
                Dim updatedRowCount As Integer = 0 
                
                For Each row As ProductDataSet.ProductRow In dataSet.Product 
                    Select Case row.RowState 
                        Case DataRowState.Added 
                            OnProductUpdating(New ProductEventArgs(row, StatementType.Insert)) 
                            _command = Me.GetCommand() 
                            _command.CommandText = _
								"INSERT INTO [Product] ( " & _
								"[ProductId], " & _
								"[CategoryId], " & _
								"[Name], " & _
								"[Descn], " & _
								"[Image] " & _
								") VALUES ( " & _
								"@ProductId," & _
								"@CategoryId," & _
								"@Name," & _
								"@Descn," & _
								"@Image" & _
								")"
								
								_command.Parameters.Add(Me.CreateParameter("@ProductId", DbType.String, IF(row.IsProductIdNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ProductId,Object))))
								_command.Parameters.Add(Me.CreateParameter("@CategoryId", DbType.String, IF(row.IsCategoryIdNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.CategoryId,Object))))
								_command.Parameters.Add(Me.CreateParameter("@Name", DbType.String, IF(row.IsNameNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.Name,Object))))
								_command.Parameters.Add(Me.CreateParameter("@Descn", DbType.String, IF(row.IsDescnNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.Descn,Object))))
								_command.Parameters.Add(Me.CreateParameter("@Image", DbType.String, IF(row.IsImageNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.Image,Object))))
                            Me.OpenConnection() 
                            _command.ExecuteNonQuery() 
                            OnProductUpdated(New ProductEventArgs(row, StatementType.Insert)) 
                            
                            updatedRowCount += 1 
                            Exit Select 
                        Case DataRowState.Modified 
                            OnProductUpdating(New ProductEventArgs(row, StatementType.Update)) 
                            _command = Me.GetCommand() 
                            _command.CommandText = _
									"UPDATE [Product] SET " & _
										"[CategoryId] = @CategoryId," & _
										"[Name] = @Name," & _
										"[Descn] = @Descn," & _
										"[Image] = @Image" & _
									" WHERE " & _
										"[ProductId] = @ProductId" & _
										" "
								_command.Parameters.Add(Me.CreateParameter("@ProductId", DbType.String, IF(row.IsProductIdNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ProductId,Object))))
								_command.Parameters.Add(Me.CreateParameter("@CategoryId", DbType.String, IF(row.IsCategoryIdNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.CategoryId,Object))))
								_command.Parameters.Add(Me.CreateParameter("@Name", DbType.String, IF(row.IsNameNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.Name,Object))))
								_command.Parameters.Add(Me.CreateParameter("@Descn", DbType.String, IF(row.IsDescnNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.Descn,Object))))
								_command.Parameters.Add(Me.CreateParameter("@Image", DbType.String, IF(row.IsImageNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.Image,Object))))
                            Me.OpenConnection() 
                            _command.ExecuteNonQuery() 
                            OnProductUpdated(New ProductEventArgs(row, StatementType.Update)) 
                            
                            updatedRowCount += 1 
                            Exit Select 
                        Case DataRowState.Deleted 
                            OnProductUpdating(New ProductEventArgs(row, StatementType.Delete)) 
                            _command = Me.GetCommand() 
                            _command.CommandText = _
									"DELETE FROM [Product]" & _
									" WHERE " & _
										"[ProductId] = @ProductId" & _
									" "
								_command.Parameters.Add(Me.CreateParameter("@ProductId", DbType.String, row(dataSet.Product.ProductIdColumn, DataRowVersion.Original)))
                            Me.OpenConnection() 
                            _command.ExecuteNonQuery() 
                            OnProductUpdated(New ProductEventArgs(row, StatementType.Delete)) 
                            
                            updatedRowCount += 1 
                            Exit Select 
                    End Select 
                Next 
                dataSet.AcceptChanges() 
                
                Return updatedRowCount 
            Catch e As Exception 
                System.Diagnostics.Debug.WriteLine(e.ToString()) 
                Return 0 
            Finally 
                Me.Cleanup() 
            End Try 
        Else 
            Me.Cleanup() 
            Throw New ArgumentException("DataSet null") 
        End If 
    End Function 
    #End Region 
    
    #Region "Events" 
    Public Delegate Sub ProductUpdateEventHandler(ByVal sender As Object, ByVal e As ProductEventArgs) 
    
    Public Event ProductUpdated As ProductUpdateEventHandler 
    Private Sub OnProductUpdated(ByVal e As ProductEventArgs)  
        RaiseEvent ProductUpdated(Me, e) 
    End Sub 
    
    Public Event ProductUpdating As ProductUpdateEventHandler 
    Private Sub OnProductUpdating(ByVal e As ProductEventArgs) 
      	RaiseEvent ProductUpdating(Me, e) 
    End Sub 
    
    Public Class ProductEventArgs 
        Inherits EventArgs 
        Private _statementType As StatementType 
        Private _dataRow As ProductDataSet.ProductRow 
        
        Public Sub New(ByVal row As ProductDataSet.ProductRow, ByVal statementType As StatementType) 
            _dataRow = row 
            _statementType = statementType 
        End Sub 
        
        Public ReadOnly Property StatementType() As StatementType 
            Get 
                Return _statementType 
            End Get 
        End Property 
        
        
        Public Property Row() As ProductDataSet.ProductRow 
            Get 
                Return _dataRow 
            End Get 
            Set 
                _dataRow = value 
            End Set 
        End Property 
    End Class 
    #End Region 
    
    #Region "Custom Exceptions" 
    <Serializable()> _ 
    Public Class ProductNotFoundException 
        Inherits ApplicationException 
        Public Sub New() 
        End Sub 
        
        Public Sub New(ByVal message As String) 
            MyBase.New(message) 
        End Sub 
        
        Public Sub New(ByVal message As String, ByVal inner As Exception) 
            MyBase.New(message, inner) 
        End Sub 
        
        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext) 
            MyBase.New(info, context) 
        End Sub 
    End Class 
    
    <Serializable()> _ 
    Public Class ForeignKeyNotFoundException 
        Inherits ApplicationException 
        Public Sub New() 
        End Sub 
        
        Public Sub New(ByVal message As String) 
            MyBase.New(message) 
        End Sub 
        
        Public Sub New(ByVal message As String, ByVal inner As Exception) 
            MyBase.New(message, inner) 
        End Sub 
        
        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext) 
            MyBase.New(info, context) 
        End Sub 
    End Class 
    
    <Serializable()> _ 
    Public Class ProductDataLockedException 
        Inherits ApplicationException 
        Public Sub New() 
        End Sub 
        
        Public Sub New(ByVal message As String) 
            MyBase.New(message) 
        End Sub 
        
        Public Sub New(ByVal message As String, ByVal inner As Exception) 
            MyBase.New(message, inner) 
        End Sub 
        
        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext) 
            MyBase.New(info, context) 
        End Sub 
    End Class 
    
    <Serializable()> _ 
    Public Class ProductDataChangedException 
        Inherits ApplicationException 
        Public Sub New() 
        End Sub 
        
        Public Sub New(ByVal message As String) 
            MyBase.New(message) 
        End Sub 
        
        Public Sub New(ByVal message As String, ByVal inner As Exception) 
            MyBase.New(message, inner) 
        End Sub 
        
        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext) 
            MyBase.New(info, context) 
        End Sub 
    End Class 
    
    <Serializable()> _ 
    Public Class ProductDuplicateKeyException 
        Inherits ApplicationException 
        Public Sub New() 
        End Sub 
        
        Public Sub New(ByVal message As String) 
            MyBase.New(message) 
        End Sub 
        
        Public Sub New(ByVal message As String, ByVal inner As Exception) 
            MyBase.New(message, inner) 
        End Sub 
        
        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext) 
            MyBase.New(info, context) 
        End Sub 
    End Class 
    
    <Serializable()> _ 
    Public Class ProductDataDeletedException 
        Inherits ApplicationException 
        Public Sub New() 
        End Sub 
        
        Public Sub New(ByVal message As String) 
            MyBase.New(message) 
        End Sub 
        
        Public Sub New(ByVal message As String, ByVal inner As Exception) 
            MyBase.New(message, inner) 
        End Sub 
        
        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext) 
            MyBase.New(info, context) 
        End Sub 
    End Class 
    #End Region 
End Class 
#End Region 

END NAMESPACE
