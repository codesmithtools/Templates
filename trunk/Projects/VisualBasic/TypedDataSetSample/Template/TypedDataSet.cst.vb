Imports System 
Imports System.Text 
Imports System.ComponentModel 
Imports CodeSmith.Engine 
Imports System.Data 
Imports SchemaExplorer
Imports Microsoft.VisualBasic
Imports System.Collections.Generic

Namespace CodeSmith.BaseTemplates
    Public Class SqlCodeTemplate
        Inherits CodeTemplate

        Public Function GetCamelCaseName(ByVal value As String) As String
            Return value.Substring(0, 1).ToLower() + value.Substring(1)
        End Function

        Public Function GetSpacedName(ByVal value As String) As String
            Dim spacedName As New StringBuilder()
            For i As Integer = 0 To value.Length - 1

                If i > 0 AndAlso i < value.Length - 1 AndAlso value.Substring(i, 1).ToUpper() = value.Substring(i, 1) Then
                    spacedName.Append(" ")
                End If
                spacedName.Append(value(i))
            Next

            Return spacedName.ToString()
        End Function

        Public Function GetClassName(ByVal value As String) As String
            Return value.Replace(" ", "")
        End Function

        Public Function GetMemberVariableName(ByVal value As String) As String
            Dim memberVariableName As String = "_" + GetCamelCaseName(value)

            Return memberVariableName
        End Function

        Public Function GetSqlParameterExtraParams(ByVal statementPrefix As String, ByVal column As ColumnSchema) As String
            If SizeMatters(column) AndAlso PrecisionMatters(column) Then
                Return ");" & Chr(13) & "" & Chr(10) & "" + statementPrefix + "prm.Scale = " + column.Scale + ";" & Chr(13) & "" & Chr(10) & "" + statementPrefix + "prm.Precision = " + column.Precision + ";"
            ElseIf SizeMatters(column) Then
                Return ", " + column.Size + ");"
            Else
                Return ");"
            End If
        End Function

        Public Function SizeMatters(ByVal column As ColumnSchema) As Boolean
            Select Case column.DataType
                Case DbType.[String], DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.[Decimal]
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Public Function PrecisionMatters(ByVal column As ColumnSchema) As Boolean
            Select Case column.DataType
                Case DbType.[Decimal]
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Public Function GetSqlReaderAssignmentStatement(ByVal column As ColumnSchema, ByVal index As Integer) As String
            Dim statement As String = "if (!reader.IsDBNull(" + index.ToString() + ")) "
            statement += GetMemberVariableName(column.Name) + " = "

            If column.Name.EndsWith("TypeCode") Then
                statement += "(" + column.Name + ")"
            End If

            statement += "reader." + GetReaderMethod(column) + "(" + index.ToString() + ");"

            Return statement
        End Function

        Public Function GetValidateStatements(ByVal table As TableSchema, ByVal statementPrefix As String) As String
            Dim statements As String = ""

            For Each column As ColumnSchema In table.Columns
                If IncludeEmptyCheck(column) Then
                    statements += "" & Chr(13) & "" & Chr(10) & "" + statementPrefix + "if (" + GetMemberVariableName(column.Name) + " == " + GetMemberVariableDefaultValue(column) + ") this.ValidationErrors.Add(new ValidationError(ValidationTypeCode.Required, """ + table.Name + """, """ + column.Name + """, """ + column.Name + " is required.""));"
                End If
                If IncludeMaxLengthCheck(column) Then
                    statements += "" & Chr(13) & "" & Chr(10) & "" + statementPrefix + "if (" + GetMemberVariableName(column.Name) + ".Length > " + column.Size.ToString() + ") this.ValidationErrors.Add(new ValidationError(ValidationTypeCode.MaxLength, """ + table.Name + """, """ + column.Name + """, """ + column.Name + " is too long.""));"
                End If
            Next

            Return statements.Substring(statementPrefix.Length + 2)
        End Function

        Public Function GetPropertyName(ByVal column As ColumnSchema) As String
            Dim propertyName As String = column.Name

            If propertyName = column.Table.Name + "Name" Then
                Return "Name"
            End If
            If propertyName = column.Table.Name + "Description" Then
                Return "Description"
            End If

            If propertyName.EndsWith("TypeCode") Then
                propertyName = propertyName.Substring(0, propertyName.Length - 4)
            End If

            Return propertyName
        End Function

       
        Public Function GetReaderMethod(ByVal column As ColumnSchema) As String
            Select Case column.DataType
                Case DbType.[Byte]
                    Return "GetByte"
                Case DbType.Int16
                    Return "GetInt16"
                Case DbType.Int32
                    Return "GetInt32"
                Case DbType.Int64
                    Return "GetInt64"
                Case DbType.AnsiStringFixedLength, DbType.AnsiString, DbType.[String], DbType.StringFixedLength
                    Return "GetString"
                Case DbType.[Boolean]
                    Return "GetBoolean"
                Case DbType.Guid
                    Return "GetGuid"
                Case DbType.Currency, DbType.[Decimal]
                    Return "GetDecimal"
                Case DbType.DateTime, DbType.[Date]
                    Return "GetDateTime"
                Case Else
                    Return "__SQL__" + column.DataType
            End Select
        End Function

        Public Function GetMemberVariableDefaultValue(ByVal column As ColumnSchema) As String
            Select Case column.DataType
                Case DbType.Guid
                    Return "Guid.Empty"
                Case DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.[String], DbType.StringFixedLength
                    Return "String.Empty"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function IncludeMaxLengthCheck(ByVal column As ColumnSchema) As Boolean
            Select Case column.DataType
                Case DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.[String], DbType.StringFixedLength
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Public Function IncludeEmptyCheck(ByVal column As ColumnSchema) As Boolean
            If column.IsPrimaryKeyMember OrElse column.AllowDBNull OrElse column.Name.EndsWith("TypeCode") Then
                Return False
            End If

            Select Case column.DataType
                Case DbType.Guid
                    Return True
                Case DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.[String], DbType.StringFixedLength
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Public Function GetSqlParameterStatement(ByVal column As ColumnSchema) As String
            Return GetSqlParameterStatement(column, False)
        End Function

        Public Function GetSqlParameterStatement(ByVal column As ColumnSchema, ByVal isOutput As Boolean) As String
            Dim param As String = "@" + column.Name + " " + column.NativeType

            Select Case column.DataType
                Case DbType.[Decimal]
                    param += "(" + column.Precision + ", " + column.Scale + ")"
                    Exit Select
                Case DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.[String], DbType.StringFixedLength
                    If column.Size > 0 Then
                        param += "(" + column.Size + ")"
                    End If
                    Exit Select
            End Select

            If isOutput Then
                param += " OUTPUT"
            End If

            Return param
        End Function

#Region "SearchCriteria Class"

        Public Class SearchCriteria
#Region "Static Content"

            Public Shared Function GetAllSearchCriteria(ByVal table As TableSchema, ByVal extendedProperty As String) As List(Of SearchCriteria)
                Dim tsc As New TableSearchCriteria(table, extendedProperty)
                Return tsc.GetAllSearchCriteria()
            End Function
            Public Shared Function GetAllSearchCriteria(ByVal table As TableSchema) As List(Of SearchCriteria)
                Dim tsc As New TableSearchCriteria(table)
                Return tsc.GetAllSearchCriteria()
            End Function

            Public Shared Function GetPrimaryKeySearchCriteria(ByVal table As TableSchema, ByVal extendedProperty As String) As List(Of SearchCriteria)
                Dim tsc As New TableSearchCriteria(table, extendedProperty)
                Return tsc.GetPrimaryKeySearchCriteria()
            End Function
            Public Shared Function GetPrimaryKeySearchCriteria(ByVal table As TableSchema) As List(Of SearchCriteria)
                Dim tsc As New TableSearchCriteria(table)
                Return tsc.GetPrimaryKeySearchCriteria()
            End Function

            Public Shared Function GetForeignKeySearchCriteria(ByVal table As TableSchema, ByVal extendedProperty As String) As List(Of SearchCriteria)
                Dim tsc As New TableSearchCriteria(table, extendedProperty)
                Return tsc.GetForeignKeySearchCriteria()
            End Function
            Public Shared Function GetForeignKeySearchCriteria(ByVal table As TableSchema) As List(Of SearchCriteria)
                Dim tsc As New TableSearchCriteria(table)
                Return tsc.GetForeignKeySearchCriteria()
            End Function

            Public Shared Function GetIndexSearchCriteria(ByVal table As TableSchema, ByVal extendedProperty As String) As List(Of SearchCriteria)
                Dim tsc As New TableSearchCriteria(table, extendedProperty)
                Return tsc.GetIndexSearchCriteria()
            End Function
            Public Shared Function GetIndexSearchCriteria(ByVal table As TableSchema) As List(Of SearchCriteria)
                Dim tsc As New TableSearchCriteria(table)
                Return tsc.GetIndexSearchCriteria()
            End Function
#End Region

#Region "Declarations"

            Protected mcsList As List(Of MemberColumnSchema)
            Protected _methodNameGenerationMode As MethodNameGenerationMode = MethodNameGenerationMode.[Default]
            Protected _methodName As String = [String].Empty
#End Region

#Region "Constructors"

            Protected Sub New()
                mcsList = New List(Of MemberColumnSchema)()
            End Sub
            Protected Sub New(ByVal mcsList As List(Of MemberColumnSchema))
                Me.mcsList = mcsList
            End Sub
#End Region

#Region "Methods"

            ''' <summary>
            ''' Sets MethodName to default generation: "GetBy{0}{1}{n}"
            ''' </summary>
            Public Sub SetMethodNameGeneration()
                _methodNameGenerationMode = MethodNameGenerationMode.[Default]

                GenerateMethodName("GetBy", [String].Empty, [String].Empty)
            End Sub
            ''' <summary>
            ''' Sets MethodName to be value of the specified Extended Property from the database.
            ''' </summary>
            ''' <param name="extendedProperty">Value of the Extended Property.</param>
            Public Sub SetMethodNameGeneration(ByVal extendedProperty As String)
                _methodNameGenerationMode = MethodNameGenerationMode.ExtendedProperty

                _methodName = extendedProperty
            End Sub
            ''' <summary>
            ''' Sets MethodName to custom generation: "{prefix}{0}{delimeter}{1}{suffix}"
            ''' </summary>
            ''' <param name="prefix">Method Prefix</param>
            ''' <param name="delimeter">Column Delimeter</param>
            ''' <param name="suffix">Method Suffix</param>
            Public Sub SetMethodNameGeneration(ByVal prefix As String, ByVal delimeter As String, ByVal suffix As String)
                _methodNameGenerationMode = MethodNameGenerationMode.[Custom]

                GenerateMethodName(prefix, delimeter, suffix)
            End Sub

            Public Overloads Overrides Function ToString() As String
                If [String].IsNullOrEmpty(_methodName) Then
                    SetMethodNameGeneration()
                End If

                Return _methodName
            End Function

            Protected Sub Add(ByVal item As MemberColumnSchema)
                mcsList.Add(item)
            End Sub
            Protected Sub GenerateMethodName(ByVal prefix As String, ByVal delimeter As String, ByVal suffix As String)
                Dim sb As New StringBuilder()
                Dim isFirst As Boolean = True

                sb.Append(prefix)
                For Each mcs As MemberColumnSchema In mcsList
                    If isFirst Then
                        isFirst = False
                    Else
                        sb.Append(delimeter)
                    End If
                    sb.Append(mcs.Name)
                Next
                sb.Append(suffix)

                _methodName = sb.ToString()
            End Sub
#End Region

#Region "Properties"

            Public ReadOnly Property Items() As List(Of MemberColumnSchema)
                Get
                    Return mcsList
                End Get
            End Property
            Public ReadOnly Property IsAllPrimaryKeys() As Boolean
                Get
                    Dim result As Boolean = True
                    For Each msc As MemberColumnSchema In mcsList
                        If Not msc.IsPrimaryKeyMember Then
                            result = False
                            Exit For
                        End If
                    Next
                    Return result
                End Get
            End Property
            Public ReadOnly Property MethodName() As String
                Get
                    Return Me.ToString()
                End Get
            End Property
            Public ReadOnly Property MethodNameGeneration() As MethodNameGenerationMode
                Get
                    Return _methodNameGenerationMode
                End Get
            End Property

            Protected ReadOnly Property Key() As String
                Get
                    Dim sb As New StringBuilder()

                    For Each mcs As MemberColumnSchema In mcsList
                        sb.Append(mcs.Name)
                    Next

                    Return sb.ToString()
                End Get
            End Property
#End Region

#Region "Enums & Classes"

            Public Enum MethodNameGenerationMode
                [Default]
                ExtendedProperty
                [Custom]
            End Enum

            Friend Class TableSearchCriteria
#Region "Declarations"

                Protected _table As TableSchema
                Protected extendedProperty As String = "cs_CriteriaName"
#End Region

#Region "Constructor"

                Public Sub New(ByVal sourceTable As TableSchema)
                    Me._table = sourceTable
                End Sub
                Public Sub New(ByVal sourceTable As TableSchema, ByVal extendedProperty As String)
                    Me.New(sourceTable)
                    Me.extendedProperty = extendedProperty
                End Sub
#End Region

#Region "Methods"

                Public Function GetAllSearchCriteria() As List(Of SearchCriteria)
                    Dim map As New Dictionary(Of String, SearchCriteria)()

                    GetPrimaryKeySearchCriteria(map)
                    GetForeignKeySearchCriteria(map)
                    GetIndexSearchCriteria(map)

                    Return GetResultsFromMap(map)
                End Function
                Public Function GetPrimaryKeySearchCriteria() As List(Of SearchCriteria)
                    Dim map As New Dictionary(Of String, SearchCriteria)()

                    GetPrimaryKeySearchCriteria(map)

                    Return GetResultsFromMap(map)
                End Function
                Public Function GetForeignKeySearchCriteria() As List(Of SearchCriteria)
                    Dim map As New Dictionary(Of String, SearchCriteria)()

                    GetForeignKeySearchCriteria(map)

                    Return GetResultsFromMap(map)
                End Function
                Public Function GetIndexSearchCriteria() As List(Of SearchCriteria)
                    Dim map As New Dictionary(Of String, SearchCriteria)()

                    GetIndexSearchCriteria(map)

                    Return GetResultsFromMap(map)
                End Function

                Protected Sub GetPrimaryKeySearchCriteria(ByVal map As Dictionary(Of String, SearchCriteria))
                    Dim mcsList As New List(Of MemberColumnSchema)(table.PrimaryKey.MemberColumns.ToArray())
                    Dim searchCriteria As New SearchCriteria(mcsList)

                    If table.PrimaryKey.ExtendedProperties.Contains(extendedProperty) Then
                        If Not [String].IsNullOrEmpty(extendedProperty) AndAlso table.PrimaryKey.ExtendedProperties.Contains(extendedProperty) AndAlso table.PrimaryKey.ExtendedProperties(extendedProperty).Value IsNot Nothing Then
                            searchCriteria.SetMethodNameGeneration(table.PrimaryKey.ExtendedProperties(extendedProperty).Value.ToString())
                        End If
                    End If

                    AddToMap(map, searchCriteria)
                End Sub
                Protected Sub GetForeignKeySearchCriteria(ByVal map As Dictionary(Of String, SearchCriteria))
                    For Each tks As TableKeySchema In table.ForeignKeys
                        Dim searchCriteria As New SearchCriteria()
                        For Each mcs As MemberColumnSchema In tks.ForeignKeyMemberColumns
                            If mcs.Table.Equals(table) Then
                                searchCriteria.Add(mcs)
                            End If
                        Next

                        If Not [String].IsNullOrEmpty(extendedProperty) AndAlso tks.ExtendedProperties.Contains(extendedProperty) AndAlso tks.ExtendedProperties(extendedProperty).Value IsNot Nothing Then
                            searchCriteria.SetMethodNameGeneration(tks.ExtendedProperties(extendedProperty).Value.ToString())
                        End If

                        AddToMap(map, searchCriteria)
                    Next
                End Sub
                Protected Sub GetIndexSearchCriteria(ByVal map As Dictionary(Of String, SearchCriteria))
                    For Each indexSchema As IndexSchema In table.Indexes
                        Dim searchCriteria As New SearchCriteria()
                        For Each mcs As MemberColumnSchema In indexSchema.MemberColumns
                            If mcs.Table.Equals(table) Then
                                searchCriteria.Add(mcs)
                            End If
                        Next

                        If Not [String].IsNullOrEmpty(extendedProperty) AndAlso indexSchema.ExtendedProperties.Contains(extendedProperty) AndAlso indexSchema.ExtendedProperties(extendedProperty).Value IsNot Nothing Then
                            searchCriteria.SetMethodNameGeneration(indexSchema.ExtendedProperties(extendedProperty).Value.ToString())
                        End If

                        AddToMap(map, searchCriteria)
                    Next
                End Sub

                Protected Function AddToMap(ByVal map As Dictionary(Of String, SearchCriteria), ByVal searchCriteria As SearchCriteria) As Boolean
                    Dim key As String = searchCriteria.Key
                    Dim result As Boolean = (searchCriteria.Items.Count > 0 AndAlso Not map.ContainsKey(key))

                    If result Then
                        map.Add(key, searchCriteria)
                    End If

                    Return result
                End Function
                Protected Function GetResultsFromMap(ByVal map As Dictionary(Of String, SearchCriteria)) As List(Of SearchCriteria)
                    Dim result As New List(Of SearchCriteria)()
                    For Each kvp As KeyValuePair(Of String, SearchCriteria) In map
                        result.Add(kvp.Value)
                    Next
                    Return result
                End Function
#End Region

#Region "Properties"

                Public ReadOnly Property Table() As TableSchema
                    Get
                        Return _table
                    End Get
                End Property
#End Region
            End Class
#End Region
        End Class
#End Region

    End Class
End Namespace