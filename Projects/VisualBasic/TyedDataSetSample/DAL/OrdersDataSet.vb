
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

#Region "OrdersDataSet" 
<Serializable()> _ 
<DesignerCategoryAttribute("code")> _ 
<System.Diagnostics.DebuggerStepThrough()> _ 
<ToolboxItem(True)> _ 
Public Class OrdersDataSet 
    Inherits DataSet 
    Private _tableOrders As OrdersDataTable 
    
    <DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)> _ 
    Public ReadOnly Property Orders() As OrdersDataTable 
        Get 
            Return Me._tableOrders 
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
        If (ds.Tables("Orders") IsNot Nothing) Then 
            Me.Tables.Add(New OrdersDataTable(ds.Tables("Orders"))) 
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
            If (ds.Tables("Orders") IsNot Nothing) Then 
                Me.Tables.Add(New OrdersDataTable(ds.Tables("Orders"))) 
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
        Me.DataSetName = "OrdersDataSet" 
        _tableOrders = New OrdersDataTable() 
        Me.Tables.Add(_tableOrders) 
        Me.ExtendedProperties.Add("DataAdapterName", "OrdersDataAdapter") 
        Me.ExtendedProperties.Add("ObjectName", "Orders") 
        Me.ExtendedProperties.Add("ObjectDescription", "Orders") 
        Me.ExtendedProperties.Add("NameSpace", "") 
    End Sub 
    
    Public Overloads Overrides Function Clone() As DataSet 
        Dim cln As OrdersDataSet = DirectCast((MyBase.Clone()), OrdersDataSet) 
        cln.InitVars() 
        Return cln 
    End Function 
    
    Friend Sub InitVars() 
        _tableOrders = DirectCast((Me.Tables("Orders")), OrdersDataTable) 
        If _tableOrders IsNot Nothing Then 
            _tableOrders.InitVars() 
        End If 
    End Sub 
    
    Protected Overloads Overrides Function ShouldSerializeTables() As Boolean 
        Return False 
    End Function 
    
    Protected Overloads Overrides Function ShouldSerializeRelations() As Boolean 
        Return False 
    End Function 
    
    Private Function ShouldSerializeOrders() As Boolean 
        Return False 
    End Function 
   
    Public Delegate Sub OrdersRowChangeEventHandler(ByVal sender As Object, ByVal e As OrdersRowChangeEventArgs) 
    
    <Serializable()> _ 
    Public Class OrdersDataTable 
        Inherits DataTable 
        Implements System.Collections.IEnumerable 
			Private _columnOrderId AS DataColumn
			Private _columnUserId AS DataColumn
			Private _columnOrderDate AS DataColumn
			Private _columnShipAddr1 AS DataColumn
			Private _columnShipAddr2 AS DataColumn
			Private _columnShipCity AS DataColumn
			Private _columnShipState AS DataColumn
			Private _columnShipZip AS DataColumn
			Private _columnShipCountry AS DataColumn
			Private _columnBillAddr1 AS DataColumn
			Private _columnBillAddr2 AS DataColumn
			Private _columnBillCity AS DataColumn
			Private _columnBillState AS DataColumn
			Private _columnBillZip AS DataColumn
			Private _columnBillCountry AS DataColumn
			Private _columnCourier AS DataColumn
			Private _columnTotalPrice AS DataColumn
			Private _columnBillToFirstName AS DataColumn
			Private _columnBillToLastName AS DataColumn
			Private _columnShipToFirstName AS DataColumn
			Private _columnShipToLastName AS DataColumn
			Private _columnAuthorizationNumber AS DataColumn
			Private _columnLocale AS DataColumn
        
        Friend Sub New() 
            MyBase.New("Orders") 
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
        
		Public ReadOnly Property OrderIdColumn
			get
				return _columnOrderId
			END GET
		END Property
			
		Public ReadOnly Property UserIdColumn
			get
				return _columnUserId
			END GET
		END Property
			
		Public ReadOnly Property OrderDateColumn
			get
				return _columnOrderDate
			END GET
		END Property
			
		Public ReadOnly Property ShipAddr1Column
			get
				return _columnShipAddr1
			END GET
		END Property
			
		Public ReadOnly Property ShipAddr2Column
			get
				return _columnShipAddr2
			END GET
		END Property
			
		Public ReadOnly Property ShipCityColumn
			get
				return _columnShipCity
			END GET
		END Property
			
		Public ReadOnly Property ShipStateColumn
			get
				return _columnShipState
			END GET
		END Property
			
		Public ReadOnly Property ShipZipColumn
			get
				return _columnShipZip
			END GET
		END Property
			
		Public ReadOnly Property ShipCountryColumn
			get
				return _columnShipCountry
			END GET
		END Property
			
		Public ReadOnly Property BillAddr1Column
			get
				return _columnBillAddr1
			END GET
		END Property
			
		Public ReadOnly Property BillAddr2Column
			get
				return _columnBillAddr2
			END GET
		END Property
			
		Public ReadOnly Property BillCityColumn
			get
				return _columnBillCity
			END GET
		END Property
			
		Public ReadOnly Property BillStateColumn
			get
				return _columnBillState
			END GET
		END Property
			
		Public ReadOnly Property BillZipColumn
			get
				return _columnBillZip
			END GET
		END Property
			
		Public ReadOnly Property BillCountryColumn
			get
				return _columnBillCountry
			END GET
		END Property
			
		Public ReadOnly Property CourierColumn
			get
				return _columnCourier
			END GET
		END Property
			
		Public ReadOnly Property TotalPriceColumn
			get
				return _columnTotalPrice
			END GET
		END Property
			
		Public ReadOnly Property BillToFirstNameColumn
			get
				return _columnBillToFirstName
			END GET
		END Property
			
		Public ReadOnly Property BillToLastNameColumn
			get
				return _columnBillToLastName
			END GET
		END Property
			
		Public ReadOnly Property ShipToFirstNameColumn
			get
				return _columnShipToFirstName
			END GET
		END Property
			
		Public ReadOnly Property ShipToLastNameColumn
			get
				return _columnShipToLastName
			END GET
		END Property
			
		Public ReadOnly Property AuthorizationNumberColumn
			get
				return _columnAuthorizationNumber
			END GET
		END Property
			
		Public ReadOnly Property LocaleColumn
			get
				return _columnLocale
			END GET
		END Property
			
   
        Public Default ReadOnly Property Item(ByVal index As Integer) As OrdersRow 
            Get 
                Return DirectCast((Me.Rows(index)), OrdersRow) 
            End Get 
        End Property 
        
        Public Event OrdersRowChanged As OrdersRowChangeEventHandler 
        Public Event OrdersRowChanging As OrdersRowChangeEventHandler 
        Public Event OrdersRowDeleted As OrdersRowChangeEventHandler 
        Public Event OrdersRowDeleting As OrdersRowChangeEventHandler 
        
        Public Sub AddOrdersRow(ByVal row As OrdersRow) 
            Me.Rows.Add(row) 
        End Sub 
       
        Public Function AddOrdersRow( _
				ByVal orderId AS Integer, _
							ByVal userId AS String, _
							ByVal orderDate AS DateTime, _
							ByVal shipAddr1 AS String, _
							ByVal shipAddr2 AS String, _
							ByVal shipCity AS String, _
							ByVal shipState AS String, _
							ByVal shipZip AS String, _
							ByVal shipCountry AS String, _
							ByVal billAddr1 AS String, _
							ByVal billAddr2 AS String, _
							ByVal billCity AS String, _
							ByVal billState AS String, _
							ByVal billZip AS String, _
							ByVal billCountry AS String, _
							ByVal courier AS String, _
							ByVal totalPrice AS Decimal, _
							ByVal billToFirstName AS String, _
							ByVal billToLastName AS String, _
							ByVal shipToFirstName AS String, _
							ByVal shipToLastName AS String, _
							ByVal authorizationNumber AS Integer, _
							ByVal locale AS String _
			) AS OrdersRow
            Dim rowOrdersRow As OrdersRow = DirectCast((Me.NewRow()), OrdersRow) 
				rowOrdersRow("OrderId") = orderId
				rowOrdersRow("UserId") = userId
				rowOrdersRow("OrderDate") = orderDate
				rowOrdersRow("ShipAddr1") = shipAddr1
				rowOrdersRow("ShipAddr2") = shipAddr2
				rowOrdersRow("ShipCity") = shipCity
				rowOrdersRow("ShipState") = shipState
				rowOrdersRow("ShipZip") = shipZip
				rowOrdersRow("ShipCountry") = shipCountry
				rowOrdersRow("BillAddr1") = billAddr1
				rowOrdersRow("BillAddr2") = billAddr2
				rowOrdersRow("BillCity") = billCity
				rowOrdersRow("BillState") = billState
				rowOrdersRow("BillZip") = billZip
				rowOrdersRow("BillCountry") = billCountry
				rowOrdersRow("Courier") = courier
				rowOrdersRow("TotalPrice") = totalPrice
				rowOrdersRow("BillToFirstName") = billToFirstName
				rowOrdersRow("BillToLastName") = billToLastName
				rowOrdersRow("ShipToFirstName") = shipToFirstName
				rowOrdersRow("ShipToLastName") = shipToLastName
				rowOrdersRow("AuthorizationNumber") = authorizationNumber
				rowOrdersRow("Locale") = locale
            Me.Rows.Add(rowOrdersRow) 
            Return rowOrdersRow 
        End Function 
        
			Public FUNCTION FindByOrderId(ByVal orderId AS Integer) AS OrdersRow
				return (DirectCast((Me.Rows.Find(new object() {orderId})),OrdersRow))
			END Function
			
        
        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me.Rows.GetEnumerator() 
        End Function 
        
        Public Overloads Overrides Function Clone() As DataTable 
            Dim cln As OrdersDataTable = DirectCast((MyBase.Clone()), OrdersDataTable) 
            cln.InitVars() 
            Return cln 
        End Function 
        
        Friend Sub InitVars() 
				_columnOrderId = Me.Columns("OrderId")
				_columnUserId = Me.Columns("UserId")
				_columnOrderDate = Me.Columns("OrderDate")
				_columnShipAddr1 = Me.Columns("ShipAddr1")
				_columnShipAddr2 = Me.Columns("ShipAddr2")
				_columnShipCity = Me.Columns("ShipCity")
				_columnShipState = Me.Columns("ShipState")
				_columnShipZip = Me.Columns("ShipZip")
				_columnShipCountry = Me.Columns("ShipCountry")
				_columnBillAddr1 = Me.Columns("BillAddr1")
				_columnBillAddr2 = Me.Columns("BillAddr2")
				_columnBillCity = Me.Columns("BillCity")
				_columnBillState = Me.Columns("BillState")
				_columnBillZip = Me.Columns("BillZip")
				_columnBillCountry = Me.Columns("BillCountry")
				_columnCourier = Me.Columns("Courier")
				_columnTotalPrice = Me.Columns("TotalPrice")
				_columnBillToFirstName = Me.Columns("BillToFirstName")
				_columnBillToLastName = Me.Columns("BillToLastName")
				_columnShipToFirstName = Me.Columns("ShipToFirstName")
				_columnShipToLastName = Me.Columns("ShipToLastName")
				_columnAuthorizationNumber = Me.Columns("AuthorizationNumber")
				_columnLocale = Me.Columns("Locale")
        End Sub 
        
        Public Sub InitClass() 
				_columnOrderId = new DataColumn("OrderId", GetType(Integer), "", MappingType.Element)
				_columnOrderId.AllowDBNull = false
				_columnOrderId.Caption = "Order Id"
				_columnOrderId.Unique = true
				_columnOrderId.DefaultValue = Convert.DBNull
				_columnOrderId.ExtendedProperties.Add("IsKey", "true")
				_columnOrderId.ExtendedProperties.Add("ReadOnly", "false")
				_columnOrderId.ExtendedProperties.Add("Description", "Order Id")
				_columnOrderId.ExtendedProperties.Add("Decimals", "0")
				_columnOrderId.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnOrderId)
				
				_columnUserId = new DataColumn("UserId", GetType(String), "", MappingType.Element)
				_columnUserId.AllowDBNull = false
				_columnUserId.Caption = "User Id"
				_columnUserId.MaxLength = 20
				_columnUserId.Unique = false
				_columnUserId.DefaultValue = Convert.DBNull
				_columnUserId.ExtendedProperties.Add("IsKey", "false")
				_columnUserId.ExtendedProperties.Add("ReadOnly", "false")
				_columnUserId.ExtendedProperties.Add("Description", "User Id")
				_columnUserId.ExtendedProperties.Add("Length", "20")
				_columnUserId.ExtendedProperties.Add("Decimals", "0")
				_columnUserId.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnUserId)
				
				_columnOrderDate = new DataColumn("OrderDate", GetType(DateTime), "", MappingType.Element)
				_columnOrderDate.AllowDBNull = false
				_columnOrderDate.Caption = "Order Date"
				_columnOrderDate.Unique = false
				_columnOrderDate.DefaultValue = Convert.DBNull
				_columnOrderDate.ExtendedProperties.Add("IsKey", "false")
				_columnOrderDate.ExtendedProperties.Add("ReadOnly", "false")
				_columnOrderDate.ExtendedProperties.Add("Description", "Order Date")
				_columnOrderDate.ExtendedProperties.Add("Decimals", "0")
				_columnOrderDate.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnOrderDate)
				
				_columnShipAddr1 = new DataColumn("ShipAddr1", GetType(String), "", MappingType.Element)
				_columnShipAddr1.AllowDBNull = false
				_columnShipAddr1.Caption = "Ship Addr1"
				_columnShipAddr1.MaxLength = 80
				_columnShipAddr1.Unique = false
				_columnShipAddr1.DefaultValue = Convert.DBNull
				_columnShipAddr1.ExtendedProperties.Add("IsKey", "false")
				_columnShipAddr1.ExtendedProperties.Add("ReadOnly", "false")
				_columnShipAddr1.ExtendedProperties.Add("Description", "Ship Addr1")
				_columnShipAddr1.ExtendedProperties.Add("Length", "80")
				_columnShipAddr1.ExtendedProperties.Add("Decimals", "0")
				_columnShipAddr1.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnShipAddr1)
				
				_columnShipAddr2 = new DataColumn("ShipAddr2", GetType(String), "", MappingType.Element)
				_columnShipAddr2.AllowDBNull = true
				_columnShipAddr2.Caption = "Ship Addr2"
				_columnShipAddr2.MaxLength = 80
				_columnShipAddr2.Unique = false
				_columnShipAddr2.DefaultValue = Convert.DBNull
				_columnShipAddr2.ExtendedProperties.Add("IsKey", "false")
				_columnShipAddr2.ExtendedProperties.Add("ReadOnly", "false")
				_columnShipAddr2.ExtendedProperties.Add("Description", "Ship Addr2")
				_columnShipAddr2.ExtendedProperties.Add("Length", "80")
				_columnShipAddr2.ExtendedProperties.Add("Decimals", "0")
				_columnShipAddr2.ExtendedProperties.Add("AllowDBNulls", "true")
				Me.Columns.Add(_columnShipAddr2)
				
				_columnShipCity = new DataColumn("ShipCity", GetType(String), "", MappingType.Element)
				_columnShipCity.AllowDBNull = false
				_columnShipCity.Caption = "Ship City"
				_columnShipCity.MaxLength = 80
				_columnShipCity.Unique = false
				_columnShipCity.DefaultValue = Convert.DBNull
				_columnShipCity.ExtendedProperties.Add("IsKey", "false")
				_columnShipCity.ExtendedProperties.Add("ReadOnly", "false")
				_columnShipCity.ExtendedProperties.Add("Description", "Ship City")
				_columnShipCity.ExtendedProperties.Add("Length", "80")
				_columnShipCity.ExtendedProperties.Add("Decimals", "0")
				_columnShipCity.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnShipCity)
				
				_columnShipState = new DataColumn("ShipState", GetType(String), "", MappingType.Element)
				_columnShipState.AllowDBNull = false
				_columnShipState.Caption = "Ship State"
				_columnShipState.MaxLength = 80
				_columnShipState.Unique = false
				_columnShipState.DefaultValue = Convert.DBNull
				_columnShipState.ExtendedProperties.Add("IsKey", "false")
				_columnShipState.ExtendedProperties.Add("ReadOnly", "false")
				_columnShipState.ExtendedProperties.Add("Description", "Ship State")
				_columnShipState.ExtendedProperties.Add("Length", "80")
				_columnShipState.ExtendedProperties.Add("Decimals", "0")
				_columnShipState.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnShipState)
				
				_columnShipZip = new DataColumn("ShipZip", GetType(String), "", MappingType.Element)
				_columnShipZip.AllowDBNull = false
				_columnShipZip.Caption = "Ship Zip"
				_columnShipZip.MaxLength = 20
				_columnShipZip.Unique = false
				_columnShipZip.DefaultValue = Convert.DBNull
				_columnShipZip.ExtendedProperties.Add("IsKey", "false")
				_columnShipZip.ExtendedProperties.Add("ReadOnly", "false")
				_columnShipZip.ExtendedProperties.Add("Description", "Ship Zip")
				_columnShipZip.ExtendedProperties.Add("Length", "20")
				_columnShipZip.ExtendedProperties.Add("Decimals", "0")
				_columnShipZip.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnShipZip)
				
				_columnShipCountry = new DataColumn("ShipCountry", GetType(String), "", MappingType.Element)
				_columnShipCountry.AllowDBNull = false
				_columnShipCountry.Caption = "Ship Country"
				_columnShipCountry.MaxLength = 20
				_columnShipCountry.Unique = false
				_columnShipCountry.DefaultValue = Convert.DBNull
				_columnShipCountry.ExtendedProperties.Add("IsKey", "false")
				_columnShipCountry.ExtendedProperties.Add("ReadOnly", "false")
				_columnShipCountry.ExtendedProperties.Add("Description", "Ship Country")
				_columnShipCountry.ExtendedProperties.Add("Length", "20")
				_columnShipCountry.ExtendedProperties.Add("Decimals", "0")
				_columnShipCountry.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnShipCountry)
				
				_columnBillAddr1 = new DataColumn("BillAddr1", GetType(String), "", MappingType.Element)
				_columnBillAddr1.AllowDBNull = false
				_columnBillAddr1.Caption = "Bill Addr1"
				_columnBillAddr1.MaxLength = 80
				_columnBillAddr1.Unique = false
				_columnBillAddr1.DefaultValue = Convert.DBNull
				_columnBillAddr1.ExtendedProperties.Add("IsKey", "false")
				_columnBillAddr1.ExtendedProperties.Add("ReadOnly", "false")
				_columnBillAddr1.ExtendedProperties.Add("Description", "Bill Addr1")
				_columnBillAddr1.ExtendedProperties.Add("Length", "80")
				_columnBillAddr1.ExtendedProperties.Add("Decimals", "0")
				_columnBillAddr1.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnBillAddr1)
				
				_columnBillAddr2 = new DataColumn("BillAddr2", GetType(String), "", MappingType.Element)
				_columnBillAddr2.AllowDBNull = true
				_columnBillAddr2.Caption = "Bill Addr2"
				_columnBillAddr2.MaxLength = 80
				_columnBillAddr2.Unique = false
				_columnBillAddr2.DefaultValue = Convert.DBNull
				_columnBillAddr2.ExtendedProperties.Add("IsKey", "false")
				_columnBillAddr2.ExtendedProperties.Add("ReadOnly", "false")
				_columnBillAddr2.ExtendedProperties.Add("Description", "Bill Addr2")
				_columnBillAddr2.ExtendedProperties.Add("Length", "80")
				_columnBillAddr2.ExtendedProperties.Add("Decimals", "0")
				_columnBillAddr2.ExtendedProperties.Add("AllowDBNulls", "true")
				Me.Columns.Add(_columnBillAddr2)
				
				_columnBillCity = new DataColumn("BillCity", GetType(String), "", MappingType.Element)
				_columnBillCity.AllowDBNull = false
				_columnBillCity.Caption = "Bill City"
				_columnBillCity.MaxLength = 80
				_columnBillCity.Unique = false
				_columnBillCity.DefaultValue = Convert.DBNull
				_columnBillCity.ExtendedProperties.Add("IsKey", "false")
				_columnBillCity.ExtendedProperties.Add("ReadOnly", "false")
				_columnBillCity.ExtendedProperties.Add("Description", "Bill City")
				_columnBillCity.ExtendedProperties.Add("Length", "80")
				_columnBillCity.ExtendedProperties.Add("Decimals", "0")
				_columnBillCity.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnBillCity)
				
				_columnBillState = new DataColumn("BillState", GetType(String), "", MappingType.Element)
				_columnBillState.AllowDBNull = false
				_columnBillState.Caption = "Bill State"
				_columnBillState.MaxLength = 80
				_columnBillState.Unique = false
				_columnBillState.DefaultValue = Convert.DBNull
				_columnBillState.ExtendedProperties.Add("IsKey", "false")
				_columnBillState.ExtendedProperties.Add("ReadOnly", "false")
				_columnBillState.ExtendedProperties.Add("Description", "Bill State")
				_columnBillState.ExtendedProperties.Add("Length", "80")
				_columnBillState.ExtendedProperties.Add("Decimals", "0")
				_columnBillState.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnBillState)
				
				_columnBillZip = new DataColumn("BillZip", GetType(String), "", MappingType.Element)
				_columnBillZip.AllowDBNull = false
				_columnBillZip.Caption = "Bill Zip"
				_columnBillZip.MaxLength = 20
				_columnBillZip.Unique = false
				_columnBillZip.DefaultValue = Convert.DBNull
				_columnBillZip.ExtendedProperties.Add("IsKey", "false")
				_columnBillZip.ExtendedProperties.Add("ReadOnly", "false")
				_columnBillZip.ExtendedProperties.Add("Description", "Bill Zip")
				_columnBillZip.ExtendedProperties.Add("Length", "20")
				_columnBillZip.ExtendedProperties.Add("Decimals", "0")
				_columnBillZip.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnBillZip)
				
				_columnBillCountry = new DataColumn("BillCountry", GetType(String), "", MappingType.Element)
				_columnBillCountry.AllowDBNull = false
				_columnBillCountry.Caption = "Bill Country"
				_columnBillCountry.MaxLength = 20
				_columnBillCountry.Unique = false
				_columnBillCountry.DefaultValue = Convert.DBNull
				_columnBillCountry.ExtendedProperties.Add("IsKey", "false")
				_columnBillCountry.ExtendedProperties.Add("ReadOnly", "false")
				_columnBillCountry.ExtendedProperties.Add("Description", "Bill Country")
				_columnBillCountry.ExtendedProperties.Add("Length", "20")
				_columnBillCountry.ExtendedProperties.Add("Decimals", "0")
				_columnBillCountry.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnBillCountry)
				
				_columnCourier = new DataColumn("Courier", GetType(String), "", MappingType.Element)
				_columnCourier.AllowDBNull = false
				_columnCourier.Caption = "Courier"
				_columnCourier.MaxLength = 80
				_columnCourier.Unique = false
				_columnCourier.DefaultValue = Convert.DBNull
				_columnCourier.ExtendedProperties.Add("IsKey", "false")
				_columnCourier.ExtendedProperties.Add("ReadOnly", "false")
				_columnCourier.ExtendedProperties.Add("Description", "Courier")
				_columnCourier.ExtendedProperties.Add("Length", "80")
				_columnCourier.ExtendedProperties.Add("Decimals", "0")
				_columnCourier.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnCourier)
				
				_columnTotalPrice = new DataColumn("TotalPrice", GetType(Decimal), "", MappingType.Element)
				_columnTotalPrice.AllowDBNull = false
				_columnTotalPrice.Caption = "Total Price"
				_columnTotalPrice.Unique = false
				_columnTotalPrice.DefaultValue = Convert.DBNull
				_columnTotalPrice.ExtendedProperties.Add("IsKey", "false")
				_columnTotalPrice.ExtendedProperties.Add("ReadOnly", "false")
				_columnTotalPrice.ExtendedProperties.Add("Description", "Total Price")
				_columnTotalPrice.ExtendedProperties.Add("Decimals", "0")
				_columnTotalPrice.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnTotalPrice)
				
				_columnBillToFirstName = new DataColumn("BillToFirstName", GetType(String), "", MappingType.Element)
				_columnBillToFirstName.AllowDBNull = false
				_columnBillToFirstName.Caption = "Bill To First Name"
				_columnBillToFirstName.MaxLength = 80
				_columnBillToFirstName.Unique = false
				_columnBillToFirstName.DefaultValue = Convert.DBNull
				_columnBillToFirstName.ExtendedProperties.Add("IsKey", "false")
				_columnBillToFirstName.ExtendedProperties.Add("ReadOnly", "false")
				_columnBillToFirstName.ExtendedProperties.Add("Description", "Bill To First Name")
				_columnBillToFirstName.ExtendedProperties.Add("Length", "80")
				_columnBillToFirstName.ExtendedProperties.Add("Decimals", "0")
				_columnBillToFirstName.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnBillToFirstName)
				
				_columnBillToLastName = new DataColumn("BillToLastName", GetType(String), "", MappingType.Element)
				_columnBillToLastName.AllowDBNull = false
				_columnBillToLastName.Caption = "Bill To Last Name"
				_columnBillToLastName.MaxLength = 80
				_columnBillToLastName.Unique = false
				_columnBillToLastName.DefaultValue = Convert.DBNull
				_columnBillToLastName.ExtendedProperties.Add("IsKey", "false")
				_columnBillToLastName.ExtendedProperties.Add("ReadOnly", "false")
				_columnBillToLastName.ExtendedProperties.Add("Description", "Bill To Last Name")
				_columnBillToLastName.ExtendedProperties.Add("Length", "80")
				_columnBillToLastName.ExtendedProperties.Add("Decimals", "0")
				_columnBillToLastName.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnBillToLastName)
				
				_columnShipToFirstName = new DataColumn("ShipToFirstName", GetType(String), "", MappingType.Element)
				_columnShipToFirstName.AllowDBNull = false
				_columnShipToFirstName.Caption = "Ship To First Name"
				_columnShipToFirstName.MaxLength = 80
				_columnShipToFirstName.Unique = false
				_columnShipToFirstName.DefaultValue = Convert.DBNull
				_columnShipToFirstName.ExtendedProperties.Add("IsKey", "false")
				_columnShipToFirstName.ExtendedProperties.Add("ReadOnly", "false")
				_columnShipToFirstName.ExtendedProperties.Add("Description", "Ship To First Name")
				_columnShipToFirstName.ExtendedProperties.Add("Length", "80")
				_columnShipToFirstName.ExtendedProperties.Add("Decimals", "0")
				_columnShipToFirstName.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnShipToFirstName)
				
				_columnShipToLastName = new DataColumn("ShipToLastName", GetType(String), "", MappingType.Element)
				_columnShipToLastName.AllowDBNull = false
				_columnShipToLastName.Caption = "Ship To Last Name"
				_columnShipToLastName.MaxLength = 80
				_columnShipToLastName.Unique = false
				_columnShipToLastName.DefaultValue = Convert.DBNull
				_columnShipToLastName.ExtendedProperties.Add("IsKey", "false")
				_columnShipToLastName.ExtendedProperties.Add("ReadOnly", "false")
				_columnShipToLastName.ExtendedProperties.Add("Description", "Ship To Last Name")
				_columnShipToLastName.ExtendedProperties.Add("Length", "80")
				_columnShipToLastName.ExtendedProperties.Add("Decimals", "0")
				_columnShipToLastName.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnShipToLastName)
				
				_columnAuthorizationNumber = new DataColumn("AuthorizationNumber", GetType(Integer), "", MappingType.Element)
				_columnAuthorizationNumber.AllowDBNull = false
				_columnAuthorizationNumber.Caption = "Authorization Number"
				_columnAuthorizationNumber.Unique = false
				_columnAuthorizationNumber.DefaultValue = Convert.DBNull
				_columnAuthorizationNumber.ExtendedProperties.Add("IsKey", "false")
				_columnAuthorizationNumber.ExtendedProperties.Add("ReadOnly", "false")
				_columnAuthorizationNumber.ExtendedProperties.Add("Description", "Authorization Number")
				_columnAuthorizationNumber.ExtendedProperties.Add("Decimals", "0")
				_columnAuthorizationNumber.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnAuthorizationNumber)
				
				_columnLocale = new DataColumn("Locale", GetType(String), "", MappingType.Element)
				_columnLocale.AllowDBNull = false
				_columnLocale.Caption = "Locale"
				_columnLocale.MaxLength = 20
				_columnLocale.Unique = false
				_columnLocale.DefaultValue = Convert.DBNull
				_columnLocale.ExtendedProperties.Add("IsKey", "false")
				_columnLocale.ExtendedProperties.Add("ReadOnly", "false")
				_columnLocale.ExtendedProperties.Add("Description", "Locale")
				_columnLocale.ExtendedProperties.Add("Length", "20")
				_columnLocale.ExtendedProperties.Add("Decimals", "0")
				_columnLocale.ExtendedProperties.Add("AllowDBNulls", "false")
				Me.Columns.Add(_columnLocale)
				
				Me.PrimaryKey = new DataColumn() {_columnOrderId}
        End Sub 
        
        Public Function NewOrdersRow() As OrdersRow 
            Dim rowOrdersRow As OrdersRow = DirectCast((Me.NewRow()), OrdersRow) 
            Return rowOrdersRow 
        End Function 
        
        Protected Overloads Overrides Function NewRowFromBuilder(ByVal builder As DataRowBuilder) As DataRow 
            Return New OrdersRow(builder) 
        End Function 
        
        Protected Overloads Overrides Function GetRowType() As Type 
            Return GetType(OrdersRow) 
        End Function 
        
        Protected Overloads Overrides Sub OnRowChanged(ByVal e As DataRowChangeEventArgs) 
            MyBase.OnRowChanged(e) 
                RaiseEvent OrdersRowChanged(Me, New OrdersRowChangeEventArgs(DirectCast((e.Row), OrdersRow), e.Action)) 
        End Sub 
        
        Protected Overloads Overrides Sub OnRowChanging(ByVal e As DataRowChangeEventArgs) 
            MyBase.OnRowChanging(e)
                RaiseEvent OrdersRowChanging(Me, New OrdersRowChangeEventArgs(DirectCast((e.Row), OrdersRow), e.Action)) 
        End Sub 
        
        Protected Overloads Overrides Sub OnRowDeleted(ByVal e As DataRowChangeEventArgs) 
            MyBase.OnRowDeleted(e) 
            RaiseEvent OrdersRowDeleted(Me, New OrdersRowChangeEventArgs(DirectCast((e.Row), OrdersRow), e.Action)) 
        End Sub 
        
        Protected Overloads Overrides Sub OnRowDeleting(ByVal e As DataRowChangeEventArgs) 
            MyBase.OnRowDeleting(e) 
            RaiseEvent OrdersRowDeleting(Me, New OrdersRowChangeEventArgs(DirectCast((e.Row), OrdersRow), e.Action)) 
        End Sub 
        
        Public Sub RemoveOrdersRow(ByVal row As OrdersRow) 
            Me.Rows.Remove(row) 
        End Sub 
    End Class 
   
    Public Class OrdersRow 
        Inherits DataRow 
        Private _tableOrders As OrdersDataTable 
        
        Friend Sub New(ByVal rb As DataRowBuilder) 
            MyBase.New(rb) 
            _tableOrders = DirectCast((Me.Table), OrdersDataTable) 
        End Sub 
        
		''' <summary>
		''' Gets or sets the value of OrderId property
		''' </summary>
		Public Property OrderId As Integer
			get
				try
					return (DirectCast((Me(_tableOrders.OrderIdColumn)),Integer))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value OrderId because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.OrderIdColumn) = value
			END SET
		End Property
			
		Public Function IsOrderIdNull() AS Boolean
			return Me.IsNull(_tableOrders.OrderIdColumn)
		END Function
			
		public Sub SetOrderIdNull()
			Me(_tableOrders.OrderIdColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of UserId property
		''' </summary>
		Public Property UserId As String
			get
				try
					return (DirectCast((Me(_tableOrders.UserIdColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value UserId because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.UserIdColumn) = value
			END SET
		End Property
			
		Public Function IsUserIdNull() AS Boolean
			return Me.IsNull(_tableOrders.UserIdColumn)
		END Function
			
		public Sub SetUserIdNull()
			Me(_tableOrders.UserIdColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of OrderDate property
		''' </summary>
		Public Property OrderDate As DateTime
			get
				try
					return (DirectCast((Me(_tableOrders.OrderDateColumn)),DateTime))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value OrderDate because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.OrderDateColumn) = value
			END SET
		End Property
			
		Public Function IsOrderDateNull() AS Boolean
			return Me.IsNull(_tableOrders.OrderDateColumn)
		END Function
			
		public Sub SetOrderDateNull()
			Me(_tableOrders.OrderDateColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of ShipAddr1 property
		''' </summary>
		Public Property ShipAddr1 As String
			get
				try
					return (DirectCast((Me(_tableOrders.ShipAddr1Column)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value ShipAddr1 because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.ShipAddr1Column) = value
			END SET
		End Property
			
		Public Function IsShipAddr1Null() AS Boolean
			return Me.IsNull(_tableOrders.ShipAddr1Column)
		END Function
			
		public Sub SetShipAddr1Null()
			Me(_tableOrders.ShipAddr1Column) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of ShipAddr2 property
		''' </summary>
		Public Property ShipAddr2 As String
			get
				try
					return (DirectCast((Me(_tableOrders.ShipAddr2Column)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value ShipAddr2 because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.ShipAddr2Column) = value
			END SET
		End Property
			
		Public Function IsShipAddr2Null() AS Boolean
			return Me.IsNull(_tableOrders.ShipAddr2Column)
		END Function
			
		public Sub SetShipAddr2Null()
			Me(_tableOrders.ShipAddr2Column) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of ShipCity property
		''' </summary>
		Public Property ShipCity As String
			get
				try
					return (DirectCast((Me(_tableOrders.ShipCityColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value ShipCity because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.ShipCityColumn) = value
			END SET
		End Property
			
		Public Function IsShipCityNull() AS Boolean
			return Me.IsNull(_tableOrders.ShipCityColumn)
		END Function
			
		public Sub SetShipCityNull()
			Me(_tableOrders.ShipCityColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of ShipState property
		''' </summary>
		Public Property ShipState As String
			get
				try
					return (DirectCast((Me(_tableOrders.ShipStateColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value ShipState because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.ShipStateColumn) = value
			END SET
		End Property
			
		Public Function IsShipStateNull() AS Boolean
			return Me.IsNull(_tableOrders.ShipStateColumn)
		END Function
			
		public Sub SetShipStateNull()
			Me(_tableOrders.ShipStateColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of ShipZip property
		''' </summary>
		Public Property ShipZip As String
			get
				try
					return (DirectCast((Me(_tableOrders.ShipZipColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value ShipZip because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.ShipZipColumn) = value
			END SET
		End Property
			
		Public Function IsShipZipNull() AS Boolean
			return Me.IsNull(_tableOrders.ShipZipColumn)
		END Function
			
		public Sub SetShipZipNull()
			Me(_tableOrders.ShipZipColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of ShipCountry property
		''' </summary>
		Public Property ShipCountry As String
			get
				try
					return (DirectCast((Me(_tableOrders.ShipCountryColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value ShipCountry because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.ShipCountryColumn) = value
			END SET
		End Property
			
		Public Function IsShipCountryNull() AS Boolean
			return Me.IsNull(_tableOrders.ShipCountryColumn)
		END Function
			
		public Sub SetShipCountryNull()
			Me(_tableOrders.ShipCountryColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of BillAddr1 property
		''' </summary>
		Public Property BillAddr1 As String
			get
				try
					return (DirectCast((Me(_tableOrders.BillAddr1Column)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value BillAddr1 because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.BillAddr1Column) = value
			END SET
		End Property
			
		Public Function IsBillAddr1Null() AS Boolean
			return Me.IsNull(_tableOrders.BillAddr1Column)
		END Function
			
		public Sub SetBillAddr1Null()
			Me(_tableOrders.BillAddr1Column) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of BillAddr2 property
		''' </summary>
		Public Property BillAddr2 As String
			get
				try
					return (DirectCast((Me(_tableOrders.BillAddr2Column)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value BillAddr2 because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.BillAddr2Column) = value
			END SET
		End Property
			
		Public Function IsBillAddr2Null() AS Boolean
			return Me.IsNull(_tableOrders.BillAddr2Column)
		END Function
			
		public Sub SetBillAddr2Null()
			Me(_tableOrders.BillAddr2Column) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of BillCity property
		''' </summary>
		Public Property BillCity As String
			get
				try
					return (DirectCast((Me(_tableOrders.BillCityColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value BillCity because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.BillCityColumn) = value
			END SET
		End Property
			
		Public Function IsBillCityNull() AS Boolean
			return Me.IsNull(_tableOrders.BillCityColumn)
		END Function
			
		public Sub SetBillCityNull()
			Me(_tableOrders.BillCityColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of BillState property
		''' </summary>
		Public Property BillState As String
			get
				try
					return (DirectCast((Me(_tableOrders.BillStateColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value BillState because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.BillStateColumn) = value
			END SET
		End Property
			
		Public Function IsBillStateNull() AS Boolean
			return Me.IsNull(_tableOrders.BillStateColumn)
		END Function
			
		public Sub SetBillStateNull()
			Me(_tableOrders.BillStateColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of BillZip property
		''' </summary>
		Public Property BillZip As String
			get
				try
					return (DirectCast((Me(_tableOrders.BillZipColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value BillZip because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.BillZipColumn) = value
			END SET
		End Property
			
		Public Function IsBillZipNull() AS Boolean
			return Me.IsNull(_tableOrders.BillZipColumn)
		END Function
			
		public Sub SetBillZipNull()
			Me(_tableOrders.BillZipColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of BillCountry property
		''' </summary>
		Public Property BillCountry As String
			get
				try
					return (DirectCast((Me(_tableOrders.BillCountryColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value BillCountry because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.BillCountryColumn) = value
			END SET
		End Property
			
		Public Function IsBillCountryNull() AS Boolean
			return Me.IsNull(_tableOrders.BillCountryColumn)
		END Function
			
		public Sub SetBillCountryNull()
			Me(_tableOrders.BillCountryColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of Courier property
		''' </summary>
		Public Property Courier As String
			get
				try
					return (DirectCast((Me(_tableOrders.CourierColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value Courier because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.CourierColumn) = value
			END SET
		End Property
			
		Public Function IsCourierNull() AS Boolean
			return Me.IsNull(_tableOrders.CourierColumn)
		END Function
			
		public Sub SetCourierNull()
			Me(_tableOrders.CourierColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of TotalPrice property
		''' </summary>
		Public Property TotalPrice As Decimal
			get
				try
					return (DirectCast((Me(_tableOrders.TotalPriceColumn)),Decimal))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value TotalPrice because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.TotalPriceColumn) = value
			END SET
		End Property
			
		Public Function IsTotalPriceNull() AS Boolean
			return Me.IsNull(_tableOrders.TotalPriceColumn)
		END Function
			
		public Sub SetTotalPriceNull()
			Me(_tableOrders.TotalPriceColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of BillToFirstName property
		''' </summary>
		Public Property BillToFirstName As String
			get
				try
					return (DirectCast((Me(_tableOrders.BillToFirstNameColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value BillToFirstName because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.BillToFirstNameColumn) = value
			END SET
		End Property
			
		Public Function IsBillToFirstNameNull() AS Boolean
			return Me.IsNull(_tableOrders.BillToFirstNameColumn)
		END Function
			
		public Sub SetBillToFirstNameNull()
			Me(_tableOrders.BillToFirstNameColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of BillToLastName property
		''' </summary>
		Public Property BillToLastName As String
			get
				try
					return (DirectCast((Me(_tableOrders.BillToLastNameColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value BillToLastName because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.BillToLastNameColumn) = value
			END SET
		End Property
			
		Public Function IsBillToLastNameNull() AS Boolean
			return Me.IsNull(_tableOrders.BillToLastNameColumn)
		END Function
			
		public Sub SetBillToLastNameNull()
			Me(_tableOrders.BillToLastNameColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of ShipToFirstName property
		''' </summary>
		Public Property ShipToFirstName As String
			get
				try
					return (DirectCast((Me(_tableOrders.ShipToFirstNameColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value ShipToFirstName because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.ShipToFirstNameColumn) = value
			END SET
		End Property
			
		Public Function IsShipToFirstNameNull() AS Boolean
			return Me.IsNull(_tableOrders.ShipToFirstNameColumn)
		END Function
			
		public Sub SetShipToFirstNameNull()
			Me(_tableOrders.ShipToFirstNameColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of ShipToLastName property
		''' </summary>
		Public Property ShipToLastName As String
			get
				try
					return (DirectCast((Me(_tableOrders.ShipToLastNameColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value ShipToLastName because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.ShipToLastNameColumn) = value
			END SET
		End Property
			
		Public Function IsShipToLastNameNull() AS Boolean
			return Me.IsNull(_tableOrders.ShipToLastNameColumn)
		END Function
			
		public Sub SetShipToLastNameNull()
			Me(_tableOrders.ShipToLastNameColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of AuthorizationNumber property
		''' </summary>
		Public Property AuthorizationNumber As Integer
			get
				try
					return (DirectCast((Me(_tableOrders.AuthorizationNumberColumn)),Integer))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value AuthorizationNumber because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.AuthorizationNumberColumn) = value
			END SET
		End Property
			
		Public Function IsAuthorizationNumberNull() AS Boolean
			return Me.IsNull(_tableOrders.AuthorizationNumberColumn)
		END Function
			
		public Sub SetAuthorizationNumberNull()
			Me(_tableOrders.AuthorizationNumberColumn) = Convert.DBNull
		END SUB
			
		''' <summary>
		''' Gets or sets the value of Locale property
		''' </summary>
		Public Property Locale As String
			get
				try
					return (DirectCast((Me(_tableOrders.LocaleColumn)),String))
				catch exception AS InvalidCastException
					throw new StrongTypingException("Cannot get value Locale because it is DBNull.", exception)
				End Try
			End Get
			set
				Me(_tableOrders.LocaleColumn) = value
			END SET
		End Property
			
		Public Function IsLocaleNull() AS Boolean
			return Me.IsNull(_tableOrders.LocaleColumn)
		END Function
			
		public Sub SetLocaleNull()
			Me(_tableOrders.LocaleColumn) = Convert.DBNull
		END SUB
			
    End Class 
    
    Public Class OrdersRowChangeEventArgs 
        Inherits EventArgs 
        Private _eventRow As OrdersRow 
        Private _eventAction As System.Data.DataRowAction 
        
        Public Sub New(ByVal row As OrdersRow, ByVal action As DataRowAction) 
            _eventRow = row 
            _eventAction = action 
        End Sub 
        
        Public ReadOnly Property Row() As OrdersRow 
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

#Region "OrdersDataAdapter" 
Public Class OrdersDataAdapter 
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
    
    Public Function Fill(ByVal dataSet As OrdersDataSet, ByVal dataRecord As IDataRecord) As Integer 
        Return Fill(dataSet, DirectCast(dataRecord("OrderId"),Integer))
    End Function 
    
    Public Function Fill(ByVal dataSet As OrdersDataSet, ByVal dataRow As DataRow) As Integer 
        Return Fill(dataSet, DirectCast(dataRow("OrderId"),Integer)) 
    End Function 
    
    Public Function Fill(ByVal dataSet As OrdersDataSet,  ByVal orderId AS Integer) As Integer 
        Try 
            _command = Me.GetCommand() 
            _command.CommandText = _
					"SELECT " & _
						"[OrderId]," & _
						"[UserId]," & _
						"[OrderDate]," & _
						"[ShipAddr1]," & _
						"[ShipAddr2]," & _
						"[ShipCity]," & _
						"[ShipState]," & _
						"[ShipZip]," & _
						"[ShipCountry]," & _
						"[BillAddr1]," & _
						"[BillAddr2]," & _
						"[BillCity]," & _
						"[BillState]," & _
						"[BillZip]," & _
						"[BillCountry]," & _
						"[Courier]," & _
						"[TotalPrice]," & _
						"[BillToFirstName]," & _
						"[BillToLastName]," & _
						"[ShipToFirstName]," & _
						"[ShipToLastName]," & _
						"[AuthorizationNumber]," & _
						"[Locale]" & _
					"FROM " & _
						"[Orders]" & _
					" WHERE " & _
						"[OrderId] = @OrderId" & _
				_command.Parameters.Add(Me.CreateParameter("@OrderId", DbType.Int32, orderId))
            Me.OpenConnection() 
            _reader = _command.ExecuteReader(CommandBehavior.CloseConnection Or CommandBehavior.SingleResult Or CommandBehavior.SingleRow) 
            If _reader.Read() Then 
                Dim row As OrdersDataSet.OrdersRow = dataSet.Orders.NewOrdersRow() 
                Me.PopulateOrdersDataRow(_reader, row) 
                dataSet.Orders.AddOrdersRow(row) 
                dataSet.AcceptChanges() 
                
                Return 1 
            Else 
                Throw New OrdersNotFoundException() 
            End If 
        Catch e As Exception 
            System.Diagnostics.Debug.WriteLine(e.ToString()) 
            Return 0 
        Finally 
            Me.Cleanup() 
        End Try 
    End Function 
    
    Private Sub PopulateOrdersDataRow(ByVal reader As IDataReader, ByVal row As OrdersDataSet.OrdersRow) 
			IF NOT reader.IsDBNull(0) THEN 
			row.OrderId = reader.GetInt32(0) 
			END IF
			IF NOT reader.IsDBNull(1) THEN 
			row.UserId = reader.GetString(1) 
			END IF
			IF NOT reader.IsDBNull(2) THEN 
			row.OrderDate = reader.GetDateTime(2) 
			END IF
			IF NOT reader.IsDBNull(3) THEN 
			row.ShipAddr1 = reader.GetString(3) 
			END IF
			IF NOT reader.IsDBNull(4) THEN 
			row.ShipAddr2 = reader.GetString(4) 
			END IF
			IF NOT reader.IsDBNull(5) THEN 
			row.ShipCity = reader.GetString(5) 
			END IF
			IF NOT reader.IsDBNull(6) THEN 
			row.ShipState = reader.GetString(6) 
			END IF
			IF NOT reader.IsDBNull(7) THEN 
			row.ShipZip = reader.GetString(7) 
			END IF
			IF NOT reader.IsDBNull(8) THEN 
			row.ShipCountry = reader.GetString(8) 
			END IF
			IF NOT reader.IsDBNull(9) THEN 
			row.BillAddr1 = reader.GetString(9) 
			END IF
			IF NOT reader.IsDBNull(10) THEN 
			row.BillAddr2 = reader.GetString(10) 
			END IF
			IF NOT reader.IsDBNull(11) THEN 
			row.BillCity = reader.GetString(11) 
			END IF
			IF NOT reader.IsDBNull(12) THEN 
			row.BillState = reader.GetString(12) 
			END IF
			IF NOT reader.IsDBNull(13) THEN 
			row.BillZip = reader.GetString(13) 
			END IF
			IF NOT reader.IsDBNull(14) THEN 
			row.BillCountry = reader.GetString(14) 
			END IF
			IF NOT reader.IsDBNull(15) THEN 
			row.Courier = reader.GetString(15) 
			END IF
			IF NOT reader.IsDBNull(16) THEN 
			row.TotalPrice = reader.GetDecimal(16) 
			END IF
			IF NOT reader.IsDBNull(17) THEN 
			row.BillToFirstName = reader.GetString(17) 
			END IF
			IF NOT reader.IsDBNull(18) THEN 
			row.BillToLastName = reader.GetString(18) 
			END IF
			IF NOT reader.IsDBNull(19) THEN 
			row.ShipToFirstName = reader.GetString(19) 
			END IF
			IF NOT reader.IsDBNull(20) THEN 
			row.ShipToLastName = reader.GetString(20) 
			END IF
			IF NOT reader.IsDBNull(21) THEN 
			row.AuthorizationNumber = reader.GetInt32(21) 
			END IF
			IF NOT reader.IsDBNull(22) THEN 
			row.Locale = reader.GetString(22) 
			END IF
    End Sub 
    
    Public Function Fill(ByVal dataSet As DataSet) As Integer Implements IDataAdapter.Fill
        Dim pageDataSet As OrdersDataSet = TryCast(dataSet, OrdersDataSet) 
        If pageDataSet IsNot Nothing Then 
            Return Me.Fill(pageDataSet) 
        Else 
            Throw New ApplicationException() 
        End If 
    End Function 
    
    Public Function Fill(ByVal dataSet As OrdersDataSet, ByVal columns As String(), ByVal values As String(), ByVal types As DbType()) As Integer 
        Try 
            Dim recordcount As Integer = 0 
            _command = Me.GetCommand() 
            _command.CommandText = _
					"SELECT " & _
						"[OrderId], " & _
						"[UserId], " & _
						"[OrderDate], " & _
						"[ShipAddr1], " & _
						"[ShipAddr2], " & _
						"[ShipCity], " & _
						"[ShipState], " & _
						"[ShipZip], " & _
						"[ShipCountry], " & _
						"[BillAddr1], " & _
						"[BillAddr2], " & _
						"[BillCity], " & _
						"[BillState], " & _
						"[BillZip], " & _
						"[BillCountry], " & _
						"[Courier], " & _
						"[TotalPrice], " & _
						"[BillToFirstName], " & _
						"[BillToLastName], " & _
						"[ShipToFirstName], " & _
						"[ShipToLastName], " & _
						"[AuthorizationNumber], " & _
						"[Locale] " & _
					"FROM " & _
						"[Orders]" & _
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
                Dim row As OrdersDataSet.OrdersRow = dataSet.Orders.NewOrdersRow() 
                Me.PopulateOrdersDataRow(_reader, row) 
                dataSet.Orders.AddOrdersRow(row) 
                
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
    
    Public Function Fill(ByVal dataSet As OrdersDataSet) As Integer 
        Try 
            Dim recordcount As Integer = 0 
            _command = Me.GetCommand() 
            _command.CommandText = _
					"SELECT " & _
						"[OrderId]," & _
						"[UserId]," & _
						"[OrderDate]," & _
						"[ShipAddr1]," & _
						"[ShipAddr2]," & _
						"[ShipCity]," & _
						"[ShipState]," & _
						"[ShipZip]," & _
						"[ShipCountry]," & _
						"[BillAddr1]," & _
						"[BillAddr2]," & _
						"[BillCity]," & _
						"[BillState]," & _
						"[BillZip]," & _
						"[BillCountry]," & _
						"[Courier]," & _
						"[TotalPrice]," & _
						"[BillToFirstName]," & _
						"[BillToLastName]," & _
						"[ShipToFirstName]," & _
						"[ShipToLastName]," & _
						"[AuthorizationNumber]," & _
						"[Locale]" & _
					"FROM " & _
						"[Orders]"
            Me.OpenConnection() 
            _reader = _command.ExecuteReader(CommandBehavior.CloseConnection Or CommandBehavior.SingleResult) 
            While _reader.Read() 
                Dim row As OrdersDataSet.OrdersRow = dataSet.Orders.NewOrdersRow() 
                Me.PopulateOrdersDataRow(_reader, row) 
                dataSet.Orders.AddOrdersRow(row) 
                
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
		Public Function FillByOrderId(ByVal dataSet As OrdersDataSet, orderId AS Integer) AS Integer
			Try
				DIM recordcount AS INTEGER = 0
				_command = Me.GetCommand()
				_command.CommandText = _
					"SELECT " & _
						"[OrderId]," & _
						"[UserId]," & _
						"[OrderDate]," & _
						"[ShipAddr1]," & _
						"[ShipAddr2]," & _
						"[ShipCity]," & _
						"[ShipState]," & _
						"[ShipZip]," & _
						"[ShipCountry]," & _
						"[BillAddr1]," & _
						"[BillAddr2]," & _
						"[BillCity]," & _
						"[BillState]," & _
						"[BillZip]," & _
						"[BillCountry]," & _
						"[Courier]," & _
						"[TotalPrice]," & _
						"[BillToFirstName]," & _
						"[BillToLastName]," & _
						"[ShipToFirstName]," & _
						"[ShipToLastName]," & _
						"[AuthorizationNumber]," & _
						"[Locale]" & _
					"FROM " & _
						"[Orders] " & _
					" WHERE" & _
						"[OrderId] = @OrderId" & _
					" "
				_command.Parameters.Add(Me.CreateParameter("@OrderId", DbType.Int32, orderId))
				Me.OpenConnection()
				_reader = _command.ExecuteReader(CommandBehavior.CloseConnection OR CommandBehavior.SingleResult)
				while (_reader.Read())
					Dim row AS OrdersDataSet.OrdersRow = dataSet.Orders.NewOrdersRow()
					Me.PopulateOrdersDataRow(_reader, row)
					dataSet.Orders.AddOrdersRow(row)
					
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
        ' not sure if I should create a OrdersId parameter here or not. 
        Return Nothing 
        '_fillDataParameters 
    End Function 
    #End Region 
    
    #Region "Update Methods" 
    Public Function Update(ByVal dataSet As DataSet) As Integer Implements IDataAdapter.Update
        Dim pageDataSet As OrdersDataSet = TryCast(dataSet, OrdersDataSet) 
        If pageDataSet IsNot Nothing Then 
            Return Me.Update(pageDataSet) 
        Else 
            Throw New ApplicationException() 
        End If 
    End Function 
    
    Public Function Update(ByVal dataSet As OrdersDataSet) As Integer 
        If dataSet IsNot Nothing Then 
            Try 
                Dim updatedRowCount As Integer = 0 
                
                For Each row As OrdersDataSet.OrdersRow In dataSet.Orders 
                    Select Case row.RowState 
                        Case DataRowState.Added 
                            OnOrdersUpdating(New OrdersEventArgs(row, StatementType.Insert)) 
                            _command = Me.GetCommand() 
                            _command.CommandText = _
								"INSERT INTO [Orders] ( " & _
								"[UserId], " & _
								"[OrderDate], " & _
								"[ShipAddr1], " & _
								"[ShipAddr2], " & _
								"[ShipCity], " & _
								"[ShipState], " & _
								"[ShipZip], " & _
								"[ShipCountry], " & _
								"[BillAddr1], " & _
								"[BillAddr2], " & _
								"[BillCity], " & _
								"[BillState], " & _
								"[BillZip], " & _
								"[BillCountry], " & _
								"[Courier], " & _
								"[TotalPrice], " & _
								"[BillToFirstName], " & _
								"[BillToLastName], " & _
								"[ShipToFirstName], " & _
								"[ShipToLastName], " & _
								"[AuthorizationNumber], " & _
								"[Locale] " & _
								") VALUES ( " & _
								"@UserId," & _
								"@OrderDate," & _
								"@ShipAddr1," & _
								"@ShipAddr2," & _
								"@ShipCity," & _
								"@ShipState," & _
								"@ShipZip," & _
								"@ShipCountry," & _
								"@BillAddr1," & _
								"@BillAddr2," & _
								"@BillCity," & _
								"@BillState," & _
								"@BillZip," & _
								"@BillCountry," & _
								"@Courier," & _
								"@TotalPrice," & _
								"@BillToFirstName," & _
								"@BillToLastName," & _
								"@ShipToFirstName," & _
								"@ShipToLastName," & _
								"@AuthorizationNumber," & _
								"@Locale" & _
								")"
								
								_command.Parameters.Add(Me.CreateParameter("@UserId", DbType.String, IF(row.IsUserIdNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.UserId,Object))))
								_command.Parameters.Add(Me.CreateParameter("@OrderDate", DbType.DateTime, IF(row.IsOrderDateNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.OrderDate,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipAddr1", DbType.String, IF(row.IsShipAddr1Null() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipAddr1,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipAddr2", DbType.String, IF(row.IsShipAddr2Null() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipAddr2,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipCity", DbType.String, IF(row.IsShipCityNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipCity,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipState", DbType.String, IF(row.IsShipStateNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipState,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipZip", DbType.String, IF(row.IsShipZipNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipZip,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipCountry", DbType.String, IF(row.IsShipCountryNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipCountry,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillAddr1", DbType.String, IF(row.IsBillAddr1Null() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillAddr1,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillAddr2", DbType.String, IF(row.IsBillAddr2Null() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillAddr2,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillCity", DbType.String, IF(row.IsBillCityNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillCity,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillState", DbType.String, IF(row.IsBillStateNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillState,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillZip", DbType.String, IF(row.IsBillZipNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillZip,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillCountry", DbType.String, IF(row.IsBillCountryNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillCountry,Object))))
								_command.Parameters.Add(Me.CreateParameter("@Courier", DbType.String, IF(row.IsCourierNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.Courier,Object))))
								_command.Parameters.Add(Me.CreateParameter("@TotalPrice", DbType.Decimal, IF(row.IsTotalPriceNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.TotalPrice,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillToFirstName", DbType.String, IF(row.IsBillToFirstNameNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillToFirstName,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillToLastName", DbType.String, IF(row.IsBillToLastNameNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillToLastName,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipToFirstName", DbType.String, IF(row.IsShipToFirstNameNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipToFirstName,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipToLastName", DbType.String, IF(row.IsShipToLastNameNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipToLastName,Object))))
								_command.Parameters.Add(Me.CreateParameter("@AuthorizationNumber", DbType.Int32, IF(row.IsAuthorizationNumberNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.AuthorizationNumber,Object))))
								_command.Parameters.Add(Me.CreateParameter("@Locale", DbType.String, IF(row.IsLocaleNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.Locale,Object))))
                            Me.OpenConnection() 
                            _command.ExecuteNonQuery() 
                            OnOrdersUpdated(New OrdersEventArgs(row, StatementType.Insert)) 
                            
                            updatedRowCount += 1 
                            Exit Select 
                        Case DataRowState.Modified 
                            OnOrdersUpdating(New OrdersEventArgs(row, StatementType.Update)) 
                            _command = Me.GetCommand() 
                            _command.CommandText = _
									"UPDATE [Orders] SET " & _
										"[UserId] = @UserId," & _
										"[OrderDate] = @OrderDate," & _
										"[ShipAddr1] = @ShipAddr1," & _
										"[ShipAddr2] = @ShipAddr2," & _
										"[ShipCity] = @ShipCity," & _
										"[ShipState] = @ShipState," & _
										"[ShipZip] = @ShipZip," & _
										"[ShipCountry] = @ShipCountry," & _
										"[BillAddr1] = @BillAddr1," & _
										"[BillAddr2] = @BillAddr2," & _
										"[BillCity] = @BillCity," & _
										"[BillState] = @BillState," & _
										"[BillZip] = @BillZip," & _
										"[BillCountry] = @BillCountry," & _
										"[Courier] = @Courier," & _
										"[TotalPrice] = @TotalPrice," & _
										"[BillToFirstName] = @BillToFirstName," & _
										"[BillToLastName] = @BillToLastName," & _
										"[ShipToFirstName] = @ShipToFirstName," & _
										"[ShipToLastName] = @ShipToLastName," & _
										"[AuthorizationNumber] = @AuthorizationNumber," & _
										"[Locale] = @Locale" & _
									" WHERE " & _
										"[OrderId] = @OrderId" & _
										" "
								_command.Parameters.Add(Me.CreateParameter("@OrderId", DbType.Int32, IF(row.IsOrderIdNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.OrderId,Object))))
								_command.Parameters.Add(Me.CreateParameter("@UserId", DbType.String, IF(row.IsUserIdNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.UserId,Object))))
								_command.Parameters.Add(Me.CreateParameter("@OrderDate", DbType.DateTime, IF(row.IsOrderDateNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.OrderDate,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipAddr1", DbType.String, IF(row.IsShipAddr1Null() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipAddr1,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipAddr2", DbType.String, IF(row.IsShipAddr2Null() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipAddr2,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipCity", DbType.String, IF(row.IsShipCityNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipCity,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipState", DbType.String, IF(row.IsShipStateNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipState,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipZip", DbType.String, IF(row.IsShipZipNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipZip,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipCountry", DbType.String, IF(row.IsShipCountryNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipCountry,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillAddr1", DbType.String, IF(row.IsBillAddr1Null() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillAddr1,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillAddr2", DbType.String, IF(row.IsBillAddr2Null() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillAddr2,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillCity", DbType.String, IF(row.IsBillCityNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillCity,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillState", DbType.String, IF(row.IsBillStateNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillState,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillZip", DbType.String, IF(row.IsBillZipNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillZip,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillCountry", DbType.String, IF(row.IsBillCountryNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillCountry,Object))))
								_command.Parameters.Add(Me.CreateParameter("@Courier", DbType.String, IF(row.IsCourierNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.Courier,Object))))
								_command.Parameters.Add(Me.CreateParameter("@TotalPrice", DbType.Decimal, IF(row.IsTotalPriceNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.TotalPrice,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillToFirstName", DbType.String, IF(row.IsBillToFirstNameNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillToFirstName,Object))))
								_command.Parameters.Add(Me.CreateParameter("@BillToLastName", DbType.String, IF(row.IsBillToLastNameNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.BillToLastName,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipToFirstName", DbType.String, IF(row.IsShipToFirstNameNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipToFirstName,Object))))
								_command.Parameters.Add(Me.CreateParameter("@ShipToLastName", DbType.String, IF(row.IsShipToLastNameNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.ShipToLastName,Object))))
								_command.Parameters.Add(Me.CreateParameter("@AuthorizationNumber", DbType.Int32, IF(row.IsAuthorizationNumberNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.AuthorizationNumber,Object))))
								_command.Parameters.Add(Me.CreateParameter("@Locale", DbType.String, IF(row.IsLocaleNull() , DirectCast(DBNull.Value,Object) , DirectCast(row.Locale,Object))))
                            Me.OpenConnection() 
                            _command.ExecuteNonQuery() 
                            OnOrdersUpdated(New OrdersEventArgs(row, StatementType.Update)) 
                            
                            updatedRowCount += 1 
                            Exit Select 
                        Case DataRowState.Deleted 
                            OnOrdersUpdating(New OrdersEventArgs(row, StatementType.Delete)) 
                            _command = Me.GetCommand() 
                            _command.CommandText = _
									"DELETE FROM [Orders]" & _
									" WHERE " & _
										"[OrderId] = @OrderId" & _
									" "
								_command.Parameters.Add(Me.CreateParameter("@OrderId", DbType.Int32, row(dataSet.Orders.OrderIdColumn, DataRowVersion.Original)))
                            Me.OpenConnection() 
                            _command.ExecuteNonQuery() 
                            OnOrdersUpdated(New OrdersEventArgs(row, StatementType.Delete)) 
                            
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
    Public Delegate Sub OrdersUpdateEventHandler(ByVal sender As Object, ByVal e As OrdersEventArgs) 
    
    Public Event OrdersUpdated As OrdersUpdateEventHandler 
    Private Sub OnOrdersUpdated(ByVal e As OrdersEventArgs)  
        RaiseEvent OrdersUpdated(Me, e) 
    End Sub 
    
    Public Event OrdersUpdating As OrdersUpdateEventHandler 
    Private Sub OnOrdersUpdating(ByVal e As OrdersEventArgs) 
      	RaiseEvent OrdersUpdating(Me, e) 
    End Sub 
    
    Public Class OrdersEventArgs 
        Inherits EventArgs 
        Private _statementType As StatementType 
        Private _dataRow As OrdersDataSet.OrdersRow 
        
        Public Sub New(ByVal row As OrdersDataSet.OrdersRow, ByVal statementType As StatementType) 
            _dataRow = row 
            _statementType = statementType 
        End Sub 
        
        Public ReadOnly Property StatementType() As StatementType 
            Get 
                Return _statementType 
            End Get 
        End Property 
        
        
        Public Property Row() As OrdersDataSet.OrdersRow 
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
    Public Class OrdersNotFoundException 
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
    Public Class OrdersDataLockedException 
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
    Public Class OrdersDataChangedException 
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
    Public Class OrdersDuplicateKeyException 
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
    Public Class OrdersDataDeletedException 
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
