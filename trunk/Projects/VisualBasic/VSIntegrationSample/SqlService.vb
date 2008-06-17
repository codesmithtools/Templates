'------------------------------------------------------------------------------
'
' Copyright (c) 2002-2008 CodeSmith Tools, LLC.  All rights reserved.
' 
' The terms of use for this software are contained in the file
' named sourcelicense.txt, which can be found in the root of this distribution.
' By using this software in any fashion, you are agreeing to be bound by the
' terms of this license.
' 
' You must not remove this notice, or any other, from this software.
'
'------------------------------------------------------------------------------

Imports System
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Collections
Imports System.Diagnostics
Imports System.Configuration
Imports System.Xml
Imports System.Text
Imports System.Globalization
Imports System.Collections.Specialized

'[DebuggerStepThrough]
Public Class SqlService
#Region "Protected Member Variables"
    Protected _connectionString As String = [String].Empty
    Protected _parameterCollection As SqlParameterCollection
    Protected _parameters As New ArrayList()
    Protected _isSingleRow As Boolean = False
    Protected _convertEmptyValuesToDbNull As Boolean = True
    Protected _convertMinValuesToDbNull As Boolean = True
    Protected _convertMaxValuesToDbNull As Boolean = False
    Protected _autoCloseConnection As Boolean = True
    Protected _connection As SqlConnection
    Protected _transaction As SqlTransaction
    Protected _commandTimeout As Integer = 30
#End Region

#Region "Contructors"
    Public Sub New()
        _connectionString = "ConnectionString"
    End Sub

    Public Sub New(ByVal connectionString As String)
        _connectionString = connectionString
    End Sub

    Public Sub New(ByVal server As String, ByVal database As String, ByVal user As String, ByVal password As String)
        Me.ConnectionString = "Server=" + server + ";Database=" + database + ";User ID=" + user + ";Password=" + password + ";"
    End Sub

    Public Sub New(ByVal server As String, ByVal database As String)
        Me.ConnectionString = "Server=" + server + ";Database=" + database + ";Integrated Security=true;"
    End Sub

    Public Sub New(ByVal connection As SqlConnection)
        Me.Connection = connection
        Me.AutoCloseConnection = False
    End Sub
#End Region

#Region "Properties"
    Public Property ConnectionString() As String
        Get
            Return _connectionString
        End Get
        Set(ByVal value As String)
            _connectionString = value
        End Set
    End Property

    Public Property CommandTimeout() As Integer
        Get
            Return _commandTimeout
        End Get
        Set(ByVal value As Integer)
            _commandTimeout = value
        End Set
    End Property

    Public Property IsSingleRow() As Boolean
        Get
            Return _isSingleRow
        End Get
        Set(ByVal value As Boolean)
            _isSingleRow = value
        End Set
    End Property

    Public Property AutoCloseConnection() As Boolean
        Get
            Return _autoCloseConnection
        End Get
        Set(ByVal value As Boolean)
            _autoCloseConnection = value
        End Set
    End Property

    Public Property Connection() As SqlConnection
        Get
            Return _connection
        End Get
        Set(ByVal value As SqlConnection)
            _connection = value
            Me.ConnectionString = _connection.ConnectionString
        End Set
    End Property

    Public Property Transaction() As SqlTransaction
        Get
            Return _transaction
        End Get
        Set(ByVal value As SqlTransaction)
            _transaction = value
        End Set
    End Property

    Public Property ConvertEmptyValuesToDbNull() As Boolean
        Get
            Return _convertEmptyValuesToDbNull
        End Get
        Set(ByVal value As Boolean)
            _convertEmptyValuesToDbNull = value
        End Set
    End Property

    Public Property ConvertMinValuesToDbNull() As Boolean
        Get
            Return _convertMinValuesToDbNull
        End Get
        Set(ByVal value As Boolean)
            _convertMinValuesToDbNull = value
        End Set
    End Property

    Public Property ConvertMaxValuesToDbNull() As Boolean
        Get
            Return _convertMaxValuesToDbNull
        End Get
        Set(ByVal value As Boolean)
            _convertMaxValuesToDbNull = value
        End Set
    End Property

    Public ReadOnly Property Parameters() As SqlParameterCollection
        Get
            Return _parameterCollection
        End Get
    End Property

    Public ReadOnly Property ReturnValue() As Integer
        Get
            If _parameterCollection.Contains("@ReturnValue") Then
                Return DirectCast(_parameterCollection("@ReturnValue").Value, Integer)
            Else
                Throw New Exception("You must call the AddReturnValueParameter method before executing your request.")
            End If
        End Get
    End Property
#End Region

#Region "Execute Methods"
    Public Sub ExecuteSql(ByVal sql As String)
        Dim cmd As New SqlCommand()
        Me.Connect()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.CommandText = sql
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandType = CommandType.Text
        cmd.ExecuteNonQuery()
        cmd.Dispose()

        If Me.AutoCloseConnection Then
            Me.Disconnect()
        End If
    End Sub

    Public Function ExecuteSqlReader(ByVal sql As String) As SqlDataReader
        Dim reader As SqlDataReader
        Dim cmd As New SqlCommand()
        Me.Connect()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.CommandText = sql
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandType = CommandType.Text
        Me.CopyParameters(cmd)

        Dim behavior As CommandBehavior = CommandBehavior.[Default]

        If Me.AutoCloseConnection Then
            behavior = behavior Or CommandBehavior.CloseConnection
        End If
        If _isSingleRow Then
            behavior = behavior Or CommandBehavior.SingleRow
        End If

        reader = cmd.ExecuteReader(behavior)
        cmd.Dispose()

        Return reader
    End Function

    Public Function ExecuteSqlXmlReader(ByVal sql As String) As XmlReader
        Dim reader As XmlReader
        Dim cmd As New SqlCommand()
        Me.Connect()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.CommandText = sql
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandType = CommandType.Text

        reader = cmd.ExecuteXmlReader()
        cmd.Dispose()

        Return reader
    End Function

    Public Function ExecuteSqlDataSet(ByVal sql As String) As DataSet
        Dim cmd As New SqlCommand()
        Me.Connect()
        Dim da As New SqlDataAdapter()
        Dim ds As New DataSet()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandText = sql
        cmd.CommandType = CommandType.Text

        da.SelectCommand = cmd

        da.Fill(ds)
        da.Dispose()
        cmd.Dispose()

        If Me.AutoCloseConnection Then
            Me.Disconnect()
        End If

        Return ds
    End Function

    Public Function ExecuteSqlDataSet(ByVal sql As String, ByVal tableName As String) As DataSet
        Dim cmd As New SqlCommand()
        Me.Connect()
        Dim da As New SqlDataAdapter()
        Dim ds As New DataSet()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandText = sql
        cmd.CommandType = CommandType.Text

        da.SelectCommand = cmd

        da.Fill(ds, tableName)
        da.Dispose()
        cmd.Dispose()

        If Me.AutoCloseConnection Then
            Me.Disconnect()
        End If

        Return ds
    End Function

    Public Sub ExecuteSqlDataSet(ByRef dataSet As DataSet, ByVal sql As String, ByVal tableName As String)
        Dim cmd As New SqlCommand()
        Me.Connect()
        Dim da As New SqlDataAdapter()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandText = sql
        cmd.CommandType = CommandType.Text

        da.SelectCommand = cmd

        da.Fill(dataSet, tableName)
        da.Dispose()
        cmd.Dispose()

        If Me.AutoCloseConnection Then
            Me.Disconnect()
        End If
    End Sub

    Public Function ExecuteSPDataSet(ByVal procedureName As String) As DataSet
        Dim cmd As New SqlCommand()
        Me.Connect()
        Dim da As New SqlDataAdapter()
        Dim ds As New DataSet()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.CommandText = procedureName
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandType = CommandType.StoredProcedure
        Me.CopyParameters(cmd)

        da.SelectCommand = cmd

        da.Fill(ds)

        _parameterCollection = cmd.Parameters
        da.Dispose()
        cmd.Dispose()

        If Me.AutoCloseConnection Then
            Me.Disconnect()
        End If

        Return ds
    End Function

    Public Function ExecuteSPDataSet(ByVal procedureName As String, ByVal tableName As String) As DataSet
        Dim cmd As New SqlCommand()
        Me.Connect()
        Dim da As New SqlDataAdapter()
        Dim ds As New DataSet()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.CommandText = procedureName
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandType = CommandType.StoredProcedure
        Me.CopyParameters(cmd)

        da.SelectCommand = cmd

        da.Fill(ds, tableName)

        _parameterCollection = cmd.Parameters
        da.Dispose()
        cmd.Dispose()

        If Me.AutoCloseConnection Then
            Me.Disconnect()
        End If

        Return ds
    End Function

    Public Sub ExecuteSPDataSet(ByRef dataSet As DataSet, ByVal procedureName As String, ByVal tableName As String)
        Dim cmd As New SqlCommand()
        Me.Connect()
        Dim da As New SqlDataAdapter()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.CommandText = procedureName
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandType = CommandType.StoredProcedure
        Me.CopyParameters(cmd)

        da.SelectCommand = cmd

        da.Fill(dataSet, tableName)

        _parameterCollection = cmd.Parameters
        da.Dispose()
        cmd.Dispose()

        If Me.AutoCloseConnection Then
            Me.Disconnect()
        End If
    End Sub

    Public Sub ExecuteSP(ByVal procedureName As String)
        Dim cmd As New SqlCommand()
        Me.Connect()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.CommandText = procedureName
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandType = CommandType.StoredProcedure
        Me.CopyParameters(cmd)

        cmd.ExecuteNonQuery()

        _parameterCollection = cmd.Parameters
        cmd.Dispose()

        If Me.AutoCloseConnection Then
            Me.Disconnect()
        End If
    End Sub

    Public Function ExecuteSPReader(ByVal procedureName As String) As SqlDataReader
        Dim reader As SqlDataReader
        Dim cmd As New SqlCommand()
        Me.Connect()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.CommandText = procedureName
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandType = CommandType.StoredProcedure
        Me.CopyParameters(cmd)

        Dim behavior As CommandBehavior = CommandBehavior.[Default]

        If Me.AutoCloseConnection Then
            behavior = behavior Or CommandBehavior.CloseConnection
        End If
        If _isSingleRow Then
            behavior = behavior Or CommandBehavior.SingleRow
        End If

        reader = cmd.ExecuteReader(behavior)

        _parameterCollection = cmd.Parameters
        cmd.Dispose()

        Return reader
    End Function

    Public Function ExecuteSPXmlReader(ByVal procedureName As String) As XmlReader
        Dim reader As XmlReader
        Dim cmd As New SqlCommand()
        Me.Connect()

        cmd.CommandTimeout = Me.CommandTimeout
        cmd.CommandText = procedureName
        cmd.Connection = _connection
        If Not IsNothing(_transaction) Then
            cmd.Transaction = _transaction
        End If
        cmd.CommandType = CommandType.StoredProcedure
        Me.CopyParameters(cmd)

        reader = cmd.ExecuteXmlReader()

        _parameterCollection = cmd.Parameters
        cmd.Dispose()

        Return reader
    End Function
#End Region

#Region "AddParameter"
    Public Function AddParameter(ByVal name As String, ByVal type As SqlDbType, ByVal value As Object) As SqlParameter
        Dim prm As New SqlParameter()
        prm.Direction = ParameterDirection.Input
        prm.ParameterName = name
        prm.SqlDbType = type
        prm.Value = Me.PrepareSqlValue(value)

        _parameters.Add(prm)

        Return prm
    End Function

    Public Function AddParameter(ByVal name As String, ByVal type As SqlDbType, ByVal value As Object, ByVal convertZeroToDBNull As Boolean) As SqlParameter
        Dim prm As New SqlParameter()
        prm.Direction = ParameterDirection.Input
        prm.ParameterName = name
        prm.SqlDbType = type
        prm.Value = Me.PrepareSqlValue(value, convertZeroToDBNull)

        _parameters.Add(prm)

        Return prm
    End Function

    Public Function AddParameter(ByVal name As String, ByVal type As DbType, ByVal value As Object, ByVal convertZeroToDBNull As Boolean) As SqlParameter
        Dim prm As New SqlParameter()
        prm.Direction = ParameterDirection.Input
        prm.ParameterName = name
        prm.DbType = type
        prm.Value = Me.PrepareSqlValue(value, convertZeroToDBNull)

        _parameters.Add(prm)

        Return prm
    End Function

    Public Function AddParameter(ByVal name As String, ByVal type As SqlDbType, ByVal value As Object, ByVal size As Integer) As SqlParameter
        Dim prm As New SqlParameter()
        prm.Direction = ParameterDirection.Input
        prm.ParameterName = name
        prm.SqlDbType = type
        prm.Size = size
        prm.Value = Me.PrepareSqlValue(value)

        _parameters.Add(prm)

        Return prm
    End Function

    Public Function AddParameter(ByVal name As String, ByVal type As SqlDbType, ByVal value As Object, ByVal direction As ParameterDirection) As SqlParameter
        Dim prm As New SqlParameter()
        prm.Direction = direction
        prm.ParameterName = name
        prm.SqlDbType = type
        prm.Value = Me.PrepareSqlValue(value)

        _parameters.Add(prm)

        Return prm
    End Function

    Public Function AddParameter(ByVal name As String, ByVal type As SqlDbType, ByVal value As Object, ByVal size As Integer, ByVal direction As ParameterDirection) As SqlParameter
        Dim prm As New SqlParameter()
        prm.Direction = direction
        prm.ParameterName = name
        prm.SqlDbType = type
        prm.Size = size
        prm.Value = Me.PrepareSqlValue(value)

        _parameters.Add(prm)

        Return prm
    End Function

    Public Sub AddParameter(ByVal parameter As SqlParameter)
        _parameters.Add(parameter)
    End Sub
#End Region

#Region "Specialized AddParameter Methods"
    Public Function AddOutputParameter(ByVal name As String, ByVal type As SqlDbType) As SqlParameter
        Dim prm As New SqlParameter()
        prm.Direction = ParameterDirection.Output
        prm.ParameterName = name
        prm.SqlDbType = type

        _parameters.Add(prm)

        Return prm
    End Function

    Public Function AddOutputParameter(ByVal name As String, ByVal type As SqlDbType, ByVal size As Integer) As SqlParameter
        Dim prm As New SqlParameter()
        prm.Direction = ParameterDirection.Output
        prm.ParameterName = name
        prm.SqlDbType = type
        prm.Size = size

        _parameters.Add(prm)

        Return prm
    End Function

    Public Function AddReturnValueParameter() As SqlParameter
        Dim prm As New SqlParameter()
        prm.Direction = ParameterDirection.ReturnValue
        prm.ParameterName = "@ReturnValue"
        prm.SqlDbType = SqlDbType.Int

        _parameters.Add(prm)

        Return prm
    End Function

    Public Function AddStreamParameter(ByVal name As String, ByVal value As Stream) As SqlParameter
        Return Me.AddStreamParameter(name, value, SqlDbType.Image)
    End Function

    Public Function AddStreamParameter(ByVal name As String, ByVal value As Stream, ByVal type As SqlDbType) As SqlParameter
        Dim prm As New SqlParameter()
        prm.Direction = ParameterDirection.Input
        prm.ParameterName = name
        prm.SqlDbType = type

        value.Position = 0
        Dim data As Byte() = New Byte(value.Length) {}
        value.Read(data, 0, System.Convert.ToInt32(value.Length))
        prm.Value = data

        _parameters.Add(prm)

        Return prm
    End Function

    Public Function AddTextParameter(ByVal name As String, ByVal value As String) As SqlParameter
        Dim prm As New SqlParameter()
        prm.Direction = ParameterDirection.Input
        prm.ParameterName = name
        prm.SqlDbType = SqlDbType.Text
        prm.Value = Me.PrepareSqlValue(value)

        _parameters.Add(prm)

        Return prm
    End Function
#End Region

#Region "Private Methods"
    Private Sub CopyParameters(ByVal command As SqlCommand)
        Dim i As Integer = 0
        While i < _parameters.Count
            command.Parameters.Add(_parameters(i))
            System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
        End While
    End Sub

    Private Function PrepareSqlValue(ByVal value As Object) As Object
        Return Me.PrepareSqlValue(value, False)
    End Function

    Private Function PrepareSqlValue(ByVal value As Object, ByVal convertZeroToDBNull As Boolean) As Object
        If TypeOf value Is String Then
            If Me.ConvertEmptyValuesToDbNull AndAlso DirectCast(value, String) = [String].Empty Then
                Return DBNull.Value
            Else
                Return value
            End If
        ElseIf TypeOf value Is Guid Then
            If Me.ConvertEmptyValuesToDbNull AndAlso DirectCast(value, Guid) = Guid.Empty Then
                Return DBNull.Value
            Else
                Return value
            End If
        ElseIf TypeOf value Is DateTime Then
            If (Me.ConvertMinValuesToDbNull AndAlso DirectCast(value, DateTime) = DateTime.MinValue) OrElse (Me.ConvertMaxValuesToDbNull AndAlso DirectCast(value, DateTime) = DateTime.MaxValue) Then
                Return DBNull.Value
            Else
                Return value
            End If
        ElseIf TypeOf value Is Int16 Then
            If (Me.ConvertMinValuesToDbNull AndAlso DirectCast(value, Int16) = Int16.MinValue) OrElse (Me.ConvertMaxValuesToDbNull AndAlso DirectCast(value, Int16) = Int16.MaxValue) OrElse (convertZeroToDBNull AndAlso DirectCast(value, Int16) = 0) Then
                Return DBNull.Value
            Else
                Return value
            End If
        ElseIf TypeOf value Is Int32 Then
            If (Me.ConvertMinValuesToDbNull AndAlso DirectCast(value, Int32) = Int32.MinValue) OrElse (Me.ConvertMaxValuesToDbNull AndAlso DirectCast(value, Int32) = Int32.MaxValue) OrElse (convertZeroToDBNull AndAlso DirectCast(value, Int32) = 0) Then
                Return DBNull.Value
            Else
                Return value
            End If
        ElseIf TypeOf value Is Int64 Then
            If (Me.ConvertMinValuesToDbNull AndAlso DirectCast(value, Int64) = Int64.MinValue) OrElse (Me.ConvertMaxValuesToDbNull AndAlso DirectCast(value, Int64) = Int64.MaxValue) OrElse (convertZeroToDBNull AndAlso DirectCast(value, Int64) = 0) Then
                Return DBNull.Value
            Else
                Return value
            End If
        ElseIf TypeOf value Is Single Then
            If (Me.ConvertMinValuesToDbNull AndAlso DirectCast(value, Single) = [Single].MinValue) OrElse (Me.ConvertMaxValuesToDbNull AndAlso DirectCast(value, Single) = [Single].MaxValue) OrElse (convertZeroToDBNull AndAlso DirectCast(value, Single) = 0) Then
                Return DBNull.Value
            Else
                Return value
            End If
        ElseIf TypeOf value Is Double Then
            If (Me.ConvertMinValuesToDbNull AndAlso DirectCast(value, Double) = [Double].MinValue) OrElse (Me.ConvertMaxValuesToDbNull AndAlso DirectCast(value, Double) = [Double].MaxValue) OrElse (convertZeroToDBNull AndAlso DirectCast(value, Double) = 0) Then
                Return DBNull.Value
            Else
                Return value
            End If
        ElseIf TypeOf value Is Decimal Then
            If (Me.ConvertMinValuesToDbNull AndAlso DirectCast(value, Decimal) = [Decimal].MinValue) OrElse (Me.ConvertMaxValuesToDbNull AndAlso DirectCast(value, Decimal) = [Decimal].MaxValue) OrElse (convertZeroToDBNull AndAlso DirectCast(value, Decimal) = 0) Then
                Return DBNull.Value
            Else
                Return value
            End If
        Else
            Return value
        End If
    End Function

    Private Function ParseConfigString(ByVal config As String) As Hashtable
        Dim attributes As New Hashtable(10, New CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture), New CaseInsensitiveComparer(CultureInfo.InvariantCulture))
        Dim keyValuePairs As String() = config.Split(";"c)
        Dim i As Integer = 0
        While i < keyValuePairs.Length
            Dim keyValuePair As String() = keyValuePairs(i).Split("="c)
            If keyValuePair.Length = 2 Then
                attributes.Add(keyValuePair(0).Trim(), keyValuePair(1).Trim())
            Else
                attributes.Add(keyValuePairs(i).Trim(), Nothing)
            End If
            System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
        End While

        Return attributes
    End Function
#End Region

#Region "Public Methods"
    Public Sub Connect()
        If Not IsNothing(_connection) Then
            If _connection.State <> ConnectionState.Open Then
                _connection.Open()
            End If
        Else
            If _connectionString <> [String].Empty Then
                Dim initKeys As New StringCollection()
                initKeys.AddRange(New String() {"ARITHABORT", "ANSI_NULLS", "ANSI_WARNINGS", "ARITHIGNORE", "ANSI_DEFAULTS", "ANSI_NULL_DFLT_OFF", _
                 "ANSI_NULL_DFLT_ON", "ANSI_PADDING", "ANSI_WARNINGS"})

                Dim initStatements As New StringBuilder()
                Dim connectionString As New StringBuilder()

                Dim attribs As Hashtable = Me.ParseConfigString(_connectionString)
                For Each key As String In attribs.Keys
                    If initKeys.Contains(key.Trim().ToUpper()) Then
                        initStatements.AppendFormat("SET {0} {1};", key, attribs(key))
                    ElseIf key.Trim().Length > 0 Then
                        connectionString.AppendFormat("{0}={1};", key, attribs(key))
                    End If
                Next

                _connection = New SqlConnection(connectionString.ToString())
                _connection.Open()

                If initStatements.Length > 0 Then
                    Dim cmd As New SqlCommand()
                    cmd.CommandTimeout = Me.CommandTimeout
                    cmd.CommandText = initStatements.ToString()
                    cmd.Connection = _connection
                    cmd.CommandType = CommandType.Text
                    cmd.ExecuteNonQuery()
                    cmd.Dispose()
                End If
            Else
                Throw New InvalidOperationException("You must set a connection object or specify a connection string before calling Connect.")
            End If
        End If
    End Sub

    Public Sub Disconnect()
        If Not IsNothing(_connection) AndAlso (_connection.State <> ConnectionState.Closed) Then
            _connection.Close()
        End If

        If Not IsNothing(_connection) Then
            _connection.Dispose()
        End If
        If Not IsNothing(_transaction) Then
            _transaction.Dispose()
        End If

        _transaction = Nothing
        _connection = Nothing
    End Sub

    Public Sub BeginTransaction()
        If Not IsNothing(_connection) Then
            _transaction = _connection.BeginTransaction()
        Else
            Throw New InvalidOperationException("You must have a valid connection object before calling BeginTransaction.")
        End If
    End Sub

    Public Sub CommitTransaction()
        If Not IsNothing(_transaction) Then
            Try
                _transaction.Commit()
            Catch generatedExceptionName As Exception
                ' TODO: We need to handle this situation.  Maybe just write a log entry or something.
                Throw
            End Try
        Else
            Throw New InvalidOperationException("You must call BeginTransaction before calling CommitTransaction.")
        End If
    End Sub

    Public Sub RollbackTransaction()

        If Not IsNothing(_transaction) Then
            Try
                _transaction.Rollback()
            Catch generatedExceptionName As Exception
                ' TODO: We need to handle this situation.  Maybe just write a log entry or something.
                Throw
            End Try
        Else
            Throw New InvalidOperationException("You must call BeginTransaction before calling RollbackTransaction.")
        End If
    End Sub

    Public Sub Reset()
        If Not IsNothing(_parameters) Then
            _parameters.Clear()
        End If

        If Not IsNothing(_parameterCollection) Then
            _parameterCollection = Nothing
        End If
    End Sub
#End Region
End Class

