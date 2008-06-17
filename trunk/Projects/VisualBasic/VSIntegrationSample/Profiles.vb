Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient

Namespace VSIntegrationSample
#Region "Profile"
''' <summary>
''' This object represents the properties and methods of a Profiles.
''' </summary>
Public Class Profile
	Private _id As System.Int32
	Private Dim _username As System.String = String.Empty
	Private Dim _applicationName As System.String = String.Empty
	Private Dim _isAnonymous As System.Boolean
	Private Dim _lastActivityDate As System.DateTime
	Private Dim _lastUpdatedDate As System.DateTime

    Public Sub New()
    End Sub

    Public Sub New(ByVal id As System.Int32)
        Dim sql As New SqlService()
        sql.AddParameter("@UniqueID", SqlDbType.VarChar, id)
        Dim reader As SqlDataReader = sql.ExecuteSqlReader("SELECT * FROM Profiles WHERE UniqueID = @UniqueID")

        If reader.Read() Then
            Me.LoadFromReader(reader)
            reader.Close()
        Else
            If Not reader.IsClosed Then
                reader.Close()
            End If
            Throw New ApplicationException("Profiles does not exist.")
        End If
    End Sub

    Public Sub New(ByVal reader As SqlDataReader)
        Me.LoadFromReader(reader)
    End Sub

    Protected Sub LoadFromReader(ByVal reader As SqlDataReader)
        If Not IsNothing(reader) AndAlso Not reader.IsClosed Then
            _id = reader.GetInt32(0)
			If Not reader.IsDBNull(1) Then
                _username = reader.GetString(1)
            End If
			If Not reader.IsDBNull(2) Then
                _applicationName = reader.GetString(2)
            End If
			If Not reader.IsDBNull(3) Then
                _isAnonymous = reader.GetBoolean(3)
            End If
			If Not reader.IsDBNull(4) Then
                _lastActivityDate = reader.GetDateTime(4)
            End If
			If Not reader.IsDBNull(5) Then
                _lastUpdatedDate = reader.GetDateTime(5)
            End If
        End If
    End Sub

    Public Sub Delete()
        Profile.Delete(Id)
    End Sub

    Public Sub Update()
        Dim sql As New SqlService()
        Dim queryParameters As New StringBuilder()

        sql.AddParameter("@UniqueID", SqlDbType.Int32, Id)
        queryParameters.Append("UniqueID = @UniqueID")

		sql.AddParameter("@Username", SqlDbType.String, Username)
        queryParameters.Append(", Username = @Username")
		sql.AddParameter("@ApplicationName", SqlDbType.String, ApplicationName)
        queryParameters.Append(", ApplicationName = @ApplicationName")
		sql.AddParameter("@IsAnonymous", SqlDbType.Boolean, IsAnonymous)
        queryParameters.Append(", IsAnonymous = @IsAnonymous")
		sql.AddParameter("@LastActivityDate", SqlDbType.DateTime, LastActivityDate)
        queryParameters.Append(", LastActivityDate = @LastActivityDate")
		sql.AddParameter("@LastUpdatedDate", SqlDbType.DateTime, LastUpdatedDate)
        queryParameters.Append(", LastUpdatedDate = @LastUpdatedDate")

        Dim query As String = [String].Format("Update Profiles Set {0} Where UniqueID = @UniqueID", queryParameters.ToString())
        Dim reader As SqlDataReader = sql.ExecuteSqlReader(query)
    End Sub

    Public Sub Create()
        Dim sql As New SqlService()
        Dim queryParameters As New StringBuilder()

        sql.AddParameter("@UniqueID", SqlDbType.Int32, Id)
        queryParameters.Append("@UniqueID")

		sql.AddParameter("@Username", SqlDbType.String, Username)
        queryParameters.Append(", @Username")
		sql.AddParameter("@ApplicationName", SqlDbType.String, ApplicationName)
        queryParameters.Append(", @ApplicationName")
		sql.AddParameter("@IsAnonymous", SqlDbType.Boolean, IsAnonymous)
        queryParameters.Append(", @IsAnonymous")
		sql.AddParameter("@LastActivityDate", SqlDbType.DateTime, LastActivityDate)
        queryParameters.Append(", @LastActivityDate")
		sql.AddParameter("@LastUpdatedDate", SqlDbType.DateTime, LastUpdatedDate)
        queryParameters.Append(", @LastUpdatedDate")

        Dim query As String = [String].Format("Insert Into Profiles ({0}) Values ({1})", queryParameters.ToString().Replace("@", ""), queryParameters.ToString())
        Dim reader As SqlDataReader = sql.ExecuteSqlReader(query)
    End Sub

    Public Shared Function NewProfile(ByVal id As System.Int32) As Profile
        Dim newEntity As New Profile()
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
	
	Public Property Username() As System.String
        Get
            Return _username
        End Get
        Set(ByVal value As System.String)
            _username = value
        End Set
    End Property
	
	Public Property ApplicationName() As System.String
        Get
            Return _applicationName
        End Get
        Set(ByVal value As System.String)
            _applicationName = value
        End Set
    End Property
	
	Public Property IsAnonymous() As System.Boolean
        Get
            Return _isAnonymous
        End Get
        Set(ByVal value As System.Boolean)
            _isAnonymous = value
        End Set
    End Property
	
	Public Property LastActivityDate() As System.DateTime
        Get
            Return _lastActivityDate
        End Get
        Set(ByVal value As System.DateTime)
            _lastActivityDate = value
        End Set
    End Property
	
	Public Property LastUpdatedDate() As System.DateTime
        Get
            Return _lastUpdatedDate
        End Get
        Set(ByVal value As System.DateTime)
            _lastUpdatedDate = value
        End Set
    End Property
	
#End Region

    Public Shared Function GetProfile(ByVal id As String) As Profile
        Return New Profile(id)
    End Function
	
	Public Shared Sub Delete(ByVal id As System.Int32)
        Dim sql As New SqlService()
        sql.AddParameter("@UniqueID", SqlDbType.Int32, id)

        Dim reader As SqlDataReader = sql.ExecuteSqlReader("Delete Profiles Where UniqueID = @UniqueID")
    End Sub
	
End Class
#End Region
End Namespace

