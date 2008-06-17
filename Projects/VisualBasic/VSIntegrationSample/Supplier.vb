Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient

Namespace VSIntegrationSample
#Region "Supplier"
''' <summary>
''' This object represents the properties and methods of a Supplier.
''' </summary>
Public Class Supplier
	Private _id As System.Int32
	Private Dim _name As System.String = String.Empty
	Private Dim _status As System.String = String.Empty
	Private Dim _addr1 As System.String = String.Empty
	Private Dim _addr2 As System.String = String.Empty
	Private Dim _city As System.String = String.Empty
	Private Dim _state As System.String = String.Empty
	Private Dim _zip As System.String = String.Empty
	Private Dim _phone As System.String = String.Empty

    Public Sub New()
    End Sub

    Public Sub New(ByVal id As System.Int32)
        Dim sql As New SqlService()
        sql.AddParameter("@SuppId", SqlDbType.VarChar, id)
        Dim reader As SqlDataReader = sql.ExecuteSqlReader("SELECT * FROM Supplier WHERE SuppId = @SuppId")

        If reader.Read() Then
            Me.LoadFromReader(reader)
            reader.Close()
        Else
            If Not reader.IsClosed Then
                reader.Close()
            End If
            Throw New ApplicationException("Supplier does not exist.")
        End If
    End Sub

    Public Sub New(ByVal reader As SqlDataReader)
        Me.LoadFromReader(reader)
    End Sub

    Protected Sub LoadFromReader(ByVal reader As SqlDataReader)
        If Not IsNothing(reader) AndAlso Not reader.IsClosed Then
            _id = reader.GetInt32(0)
			If Not reader.IsDBNull(1) Then
                _name = reader.GetString(1)
            End If
			If Not reader.IsDBNull(2) Then
                _status = reader.GetString(2)
            End If
			If Not reader.IsDBNull(3) Then
                _addr1 = reader.GetString(3)
            End If
			If Not reader.IsDBNull(4) Then
                _addr2 = reader.GetString(4)
            End If
			If Not reader.IsDBNull(5) Then
                _city = reader.GetString(5)
            End If
			If Not reader.IsDBNull(6) Then
                _state = reader.GetString(6)
            End If
			If Not reader.IsDBNull(7) Then
                _zip = reader.GetString(7)
            End If
			If Not reader.IsDBNull(8) Then
                _phone = reader.GetString(8)
            End If
        End If
    End Sub

    Public Sub Delete()
        Supplier.Delete(Id)
    End Sub

    Public Sub Update()
        Dim sql As New SqlService()
        Dim queryParameters As New StringBuilder()

        sql.AddParameter("@SuppId", SqlDbType.Int32, Id)
        queryParameters.Append("SuppId = @SuppId")

		sql.AddParameter("@Name", SqlDbType.String, Name)
        queryParameters.Append(", Name = @Name")
		sql.AddParameter("@Status", SqlDbType.String, Status)
        queryParameters.Append(", Status = @Status")
		sql.AddParameter("@Addr1", SqlDbType.String, Addr1)
        queryParameters.Append(", Addr1 = @Addr1")
		sql.AddParameter("@Addr2", SqlDbType.String, Addr2)
        queryParameters.Append(", Addr2 = @Addr2")
		sql.AddParameter("@City", SqlDbType.String, City)
        queryParameters.Append(", City = @City")
		sql.AddParameter("@State", SqlDbType.String, State)
        queryParameters.Append(", State = @State")
		sql.AddParameter("@Zip", SqlDbType.String, Zip)
        queryParameters.Append(", Zip = @Zip")
		sql.AddParameter("@Phone", SqlDbType.String, Phone)
        queryParameters.Append(", Phone = @Phone")

        Dim query As String = [String].Format("Update Supplier Set {0} Where SuppId = @SuppId", queryParameters.ToString())
        Dim reader As SqlDataReader = sql.ExecuteSqlReader(query)
    End Sub

    Public Sub Create()
        Dim sql As New SqlService()
        Dim queryParameters As New StringBuilder()

        sql.AddParameter("@SuppId", SqlDbType.Int32, Id)
        queryParameters.Append("@SuppId")

		sql.AddParameter("@Name", SqlDbType.String, Name)
        queryParameters.Append(", @Name")
		sql.AddParameter("@Status", SqlDbType.String, Status)
        queryParameters.Append(", @Status")
		sql.AddParameter("@Addr1", SqlDbType.String, Addr1)
        queryParameters.Append(", @Addr1")
		sql.AddParameter("@Addr2", SqlDbType.String, Addr2)
        queryParameters.Append(", @Addr2")
		sql.AddParameter("@City", SqlDbType.String, City)
        queryParameters.Append(", @City")
		sql.AddParameter("@State", SqlDbType.String, State)
        queryParameters.Append(", @State")
		sql.AddParameter("@Zip", SqlDbType.String, Zip)
        queryParameters.Append(", @Zip")
		sql.AddParameter("@Phone", SqlDbType.String, Phone)
        queryParameters.Append(", @Phone")

        Dim query As String = [String].Format("Insert Into Supplier ({0}) Values ({1})", queryParameters.ToString().Replace("@", ""), queryParameters.ToString())
        Dim reader As SqlDataReader = sql.ExecuteSqlReader(query)
    End Sub

    Public Shared Function NewSupplier(ByVal id As System.Int32) As Supplier
        Dim newEntity As New Supplier()
        newEntity._id = id

        Return newEntity
    End Function

#Region "Public Properties"
    Public Property Id() As  System.Int32
        Get
            Return _id
        End Get
		Set(ByVal value As System.Int32)
            _id = value
        End Set
    End Property
	
	Public Property Name() As System.String
        Get
            Return _name
        End Get
        Set(ByVal value As System.String)
            _name = value
        End Set
    End Property
	
	Public Property Status() As System.String
        Get
            Return _status
        End Get
        Set(ByVal value As System.String)
            _status = value
        End Set
    End Property
	
	Public Property Addr1() As System.String
        Get
            Return _addr1
        End Get
        Set(ByVal value As System.String)
            _addr1 = value
        End Set
    End Property
	
	Public Property Addr2() As System.String
        Get
            Return _addr2
        End Get
        Set(ByVal value As System.String)
            _addr2 = value
        End Set
    End Property
	
	Public Property City() As System.String
        Get
            Return _city
        End Get
        Set(ByVal value As System.String)
            _city = value
        End Set
    End Property
	
	Public Property State() As System.String
        Get
            Return _state
        End Get
        Set(ByVal value As System.String)
            _state = value
        End Set
    End Property
	
	Public Property Zip() As System.String
        Get
            Return _zip
        End Get
        Set(ByVal value As System.String)
            _zip = value
        End Set
    End Property
	
	Public Property Phone() As System.String
        Get
            Return _phone
        End Get
        Set(ByVal value As System.String)
            _phone = value
        End Set
    End Property
	
#End Region

    Public Shared Function GetSupplier(ByVal id As String) As Supplier
        Return New Supplier(id)
    End Function
	
	Public Shared Sub Delete(ByVal id As System.Int32)
        Dim sql As New SqlService()
        sql.AddParameter("@SuppId", SqlDbType.Int32, id)

        Dim reader As SqlDataReader = sql.ExecuteSqlReader("Delete Supplier Where SuppId = @SuppId")
    End Sub
	
End Class
#End Region
End Namespace

