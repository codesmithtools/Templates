Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient

Namespace VSIntegrationSample
#Region "Product"
''' <summary>
''' This object represents the properties and methods of a Product.
''' </summary>
Public Class Product
	Private _id As System.String
	Private Dim _categoryId As System.String = String.Empty
	Private Dim _name As System.String = String.Empty
	Private Dim _descn As System.String = String.Empty
	Private Dim _image As System.String = String.Empty

    Public Sub New()
    End Sub

    Public Sub New(ByVal id As System.String)
        Dim sql As New SqlService()
        sql.AddParameter("@ProductId", SqlDbType.VarChar, id)
        Dim reader As SqlDataReader = sql.ExecuteSqlReader("SELECT * FROM Product WHERE ProductId = @ProductId")

        If reader.Read() Then
            Me.LoadFromReader(reader)
            reader.Close()
        Else
            If Not reader.IsClosed Then
                reader.Close()
            End If
            Throw New ApplicationException("Product does not exist.")
        End If
    End Sub

    Public Sub New(ByVal reader As SqlDataReader)
        Me.LoadFromReader(reader)
    End Sub

    Protected Sub LoadFromReader(ByVal reader As SqlDataReader)
        If Not IsNothing(reader) AndAlso Not reader.IsClosed Then
            _id = reader.GetString(0)
			If Not reader.IsDBNull(1) Then
                _categoryId = reader.GetString(1)
            End If
			If Not reader.IsDBNull(2) Then
                _name = reader.GetString(2)
            End If
			If Not reader.IsDBNull(3) Then
                _descn = reader.GetString(3)
            End If
			If Not reader.IsDBNull(4) Then
                _image = reader.GetString(4)
            End If
        End If
    End Sub

    Public Sub Delete()
        Product.Delete(Id)
    End Sub

    Public Sub Update()
        Dim sql As New SqlService()
        Dim queryParameters As New StringBuilder()

        sql.AddParameter("@ProductId", SqlDbType.VarChar, Id)
        queryParameters.Append("ProductId = @ProductId")

		sql.AddParameter("@CategoryId", SqlDbType.VarChar, CategoryId)
        queryParameters.Append(", CategoryId = @CategoryId")
		sql.AddParameter("@Name", SqlDbType.VarChar, Name)
        queryParameters.Append(", Name = @Name")
		sql.AddParameter("@Descn", SqlDbType.VarChar, Descn)
        queryParameters.Append(", Descn = @Descn")
		sql.AddParameter("@Image", SqlDbType.VarChar, Image)
        queryParameters.Append(", Image = @Image")

        Dim query As String = [String].Format("Update Product Set {0} Where ProductId = @ProductId", queryParameters.ToString())
        Dim reader As SqlDataReader = sql.ExecuteSqlReader(query)
    End Sub

    Public Sub Create()
        Dim sql As New SqlService()
        Dim queryParameters As New StringBuilder()

        sql.AddParameter("@ProductId", SqlDbType.VarChar, Id)
        queryParameters.Append("@ProductId")

		sql.AddParameter("@CategoryId", SqlDbType.VarChar, CategoryId)
        queryParameters.Append(", @CategoryId")
		sql.AddParameter("@Name", SqlDbType.VarChar, Name)
        queryParameters.Append(", @Name")
		sql.AddParameter("@Descn", SqlDbType.VarChar, Descn)
        queryParameters.Append(", @Descn")
		sql.AddParameter("@Image", SqlDbType.VarChar, Image)
        queryParameters.Append(", @Image")

        Dim query As String = [String].Format("Insert Into Product ({0}) Values ({1})", queryParameters.ToString().Replace("@", ""), queryParameters.ToString())
        Dim reader As SqlDataReader = sql.ExecuteSqlReader(query)
    End Sub

    Public Shared Function NewProduct(ByVal id As System.String) As Product
        Dim newEntity As New Product()
        newEntity._id = id

        Return newEntity
    End Function

#Region "Public Properties"
    Public Property Id() As  System.String
        Get
            Return _id
        End Get
		Set(ByVal value As System.String)
            _id = value
        End Set
    End Property
	
	Public Property CategoryId() As System.String
        Get
            Return _categoryId
        End Get
        Set(ByVal value As System.String)
            _categoryId = value
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
	
	Public Property Descn() As System.String
        Get
            Return _descn
        End Get
        Set(ByVal value As System.String)
            _descn = value
        End Set
    End Property
	
	Public Property Image() As System.String
        Get
            Return _image
        End Get
        Set(ByVal value As System.String)
            _image = value
        End Set
    End Property
	
#End Region

    Public Shared Function GetProduct(ByVal id As String) As Product
        Return New Product(id)
    End Function
	
	Public Shared Sub Delete(ByVal id As System.String)
        Dim sql As New SqlService()
        sql.AddParameter("@ProductId", SqlDbType.VarChar, id)

        Dim reader As SqlDataReader = sql.ExecuteSqlReader("Delete Product Where ProductId = @ProductId")
    End Sub
	
End Class
#End Region
End Namespace

