Imports System
Imports System.ComponentModel
Imports System.Data
Imports System.Collections.Generic
Imports System.Text
Imports CodeSmith.Engine
Imports SchemaExplorer

Public Enum NHibernateVersion
    v1_2
    v2_0
End Enum

Public Class NHibernateHelper
    Inherits CodeTemplate
    Public Function GetCriterionNamespace(ByVal version As NHibernateVersion) As String
        Select Case version
            Case NHibernateVersion.v1_2
                Return "NHibernate.Expression"
				
            Case NHibernateVersion.v2_0
				Return "NHibernate.Criterion"
				
            Case Else
                Throw New Exception("Invalid NHibernateVersion")

        End Select
    End Function

#Region "Variable Name Methods"

    Public Function GetPropertyName(ByVal name As String) As String
        Return StringUtil.ToPascalCase(name)
    End Function
    Public Function GetPropertyNamePlural(ByVal name As String) As String
        Return GetPropertyName(GetNamePlural(name))
    End Function

    Public Function GetPrivateVariableName(ByVal name As String) As String
        Return "p_" + GetVariableName(name)
    End Function
    Public Function GetPrivateVariableNamePlural(ByVal name As String) As String
        Return GetPrivateVariableName(GetNamePlural(name))
    End Function

    Public Function GetVariableName(ByVal name As String) As String
        Return "_" + StringUtil.ToCamelCase(name)
    End Function
    Public Function GetVariableNamePlural(ByVal name As String) As String
        Return GetVariableName(GetNamePlural(name))
    End Function

    Private Function GetNamePlural(ByVal name As String) As String
        Dim result As New System.Text.StringBuilder()
        result.Append(name)

        If Not name.EndsWith("es") AndAlso name.EndsWith("s") Then
            result.Append("es")
        Else
            result.Append("s")
        End If

        Return result.ToString()
    End Function
#End Region

#Region "ManyToMany Table Methods"

    Public Function GetToManyTable(ByVal manyToTable As TableSchema, ByVal sourceTable As TableSchema) As TableSchema
        Dim result As TableSchema = Nothing
        For Each key As TableKeySchema In manyToTable.ForeignKeys
            If Not key.PrimaryKeyTable.Equals(sourceTable) Then
                result = key.PrimaryKeyTable
                Exit For
            End If
        Next
        Return result
    End Function
    Public Shared Function GetToManyTableKey(ByVal manyToTable As TableSchema, ByVal foreignTable As TableSchema) As MemberColumnSchema
        Dim result As MemberColumnSchema = Nothing
        For Each key As TableKeySchema In manyToTable.ForeignKeys
            If key.PrimaryKeyTable.Equals(foreignTable) Then
                result = key.ForeignKeyMemberColumns(0)
                Exit For
            End If
        Next
        Return result
    End Function
    Public Function IsManyToMany(ByVal table As TableSchema) As Boolean
        ' If there are 2 ForeignKeyColumns AND...
        ' ...there are only two columns OR
        '    there are 3 columns and 1 is a primary key.
        Return (table.ForeignKeyColumns.Count = 2 AndAlso ((table.Columns.Count = 2) OrElse (table.Columns.Count = 3 AndAlso table.PrimaryKey IsNot Nothing)))
    End Function
#End Region

    Public Function GetCascade(ByVal column As MemberColumnSchema) As String
		If column.AllowDBNull Then
			Return "all"
		Else
			Return "all-delete-orphan"
		End If
    End Function

#Region "BusinessObject Methods"

    Public Function GetInitialization(ByVal type As Type) As String
        Dim result As String

        If type.Equals(GetType(String)) Then
            result = "String.Empty"
        ElseIf type.Equals(GetType(DateTime)) Then
            result = "new DateTime()"
        Else
            result = "Nothing"
        End If
        Return result
    End Function
    Public Function GetBusinessBaseIdType(ByVal table As TableSchema) As Type
        If IsMutliColumnPrimaryKey(table.PrimaryKey) Then
            Return GetType(String)
        Else
            Return GetPrimaryKeyColumn(table.PrimaryKey).SystemType
        End If
    End Function
#End Region

    Public Function GetClassName(ByVal tableName As String) As String
        If Not [String].IsNullOrEmpty(_tablePrefix) AndAlso tableName.StartsWith(_tablePrefix) Then
            tableName = tableName.Remove(0, _tablePrefix.Length)
        End If

        If tableName.EndsWith("s") AndAlso Not tableName.EndsWith("ss") Then
            tableName = tableName.Substring(0, tableName.Length - 1)
        End If

        Return StringUtil.ToPascalCase(tableName)
    End Function
    Protected _tablePrefix As String = [String].Empty

#Region "PrimaryKey Methods"

    Public Function GetPrimaryKeyColumn(ByVal primaryKey As PrimaryKeySchema) As MemberColumnSchema
        If primaryKey.MemberColumns.Count <> 1 Then
            Throw New System.ApplicationException("This method will only work on primary keys with exactly one member column.")
        End If
        Return primaryKey.MemberColumns(0)
    End Function
    Public Function IsMutliColumnPrimaryKey(ByVal primaryKey As PrimaryKeySchema) As Boolean
        If primaryKey.MemberColumns.Count = 0 Then
            Throw New System.ApplicationException("This template will only work on primary keys with exactly one member column.")
        End If

        Return (primaryKey.MemberColumns.Count > 1)
    End Function
    Public Function GetForeignKeyColumnClassName(ByVal mcs As MemberColumnSchema, ByVal table As TableSchema) As String
        Dim result As String = [String].Empty
        For Each tks As TableKeySchema In table.ForeignKeys
            If tks.ForeignKeyMemberColumns.Contains(mcs) Then
                result = GetPropertyName(tks.PrimaryKeyTable.Name)
                Exit For
            End If
        Next
        Return result
    End Function
    Public Function IsPrimaryKeyColumn(ByVal mcs As MemberColumnSchema, ByVal table As TableSchema) As Boolean
        Dim result As Boolean = False
        For Each primaryKeyColumn As MemberColumnSchema In table.PrimaryKey.MemberColumns
            If primaryKeyColumn.Equals(mcs) Then
                result = True
                Exit For
            End If
        Next
        Return result
    End Function
#End Region

#Region "Method Creation Methods"

    Public Function GetMethodParameters(ByVal mcsList As List(Of MemberColumnSchema), ByVal isDeclaration As Boolean) As String
        Dim result As New StringBuilder()
        Dim isFirst As Boolean = True
        For Each mcs As MemberColumnSchema In mcsList
            If isFirst Then
                isFirst = False
            Else
                result.Append(", ")
            End If
            If isDeclaration Then
                result.Append("ByVal ")
            End If
            result.Append(GetVariableName(mcs.Name))
			If isDeclaration Then
				result.Append(" As ")
                result.Append(mcs.SystemType.ToString())
            End If
        Next
        Return result.ToString()
    End Function
    Public Function GetMethodParameters(ByVal mcsc As MemberColumnSchemaCollection, ByVal isDeclaration As Boolean) As String
        Dim mcsList As New List(Of MemberColumnSchema)()
        Dim x As Integer = 0
        While x < mcsc.Count
            mcsList.Add(mcsc(x))
            System.Math.Max(System.Threading.Interlocked.Increment(x), x - 1)
        End While
        Return GetMethodParameters(mcsList, isDeclaration)
    End Function
    Public Function GetMethodDeclaration(ByVal sc As SearchCriteria) As String
        Dim result As New StringBuilder()
        result.Append(GetMethodName(sc))
        result.Append("(")
        result.Append(GetMethodParameters(sc.Items, True))
        result.Append(")")
        Return result.ToString()
    End Function
	Public Function GetMethodName(ByVal sc As SearchCriteria) As String
		Dim result As New StringBuilder()
        result.Append("GetBy")
        For Each mcs As MemberColumnSchema In sc.Items
            result.Append(GetPropertyName(mcs.Name))
        Next
		Return result.ToString()
	End Function
    Public Function GetPrimaryKeyCallParameters(ByVal mcsList As List(Of MemberColumnSchema)) As String
        Dim result As New System.Text.StringBuilder()
        Dim x As Integer = 0
        While x < mcsList.Count
            If x > 0 Then
                result.Append(", ")
            End If
            result.Append([String].Format("{0}.Parse(keys({1}))", mcsList(x).SystemType, x))
            System.Math.Max(System.Threading.Interlocked.Increment(x), x - 1)
        End While
        Return result.ToString()
    End Function
#End Region

    Public Function GetForeignTableName(ByVal mcs As MemberColumnSchema, ByVal table As TableSchema) As String
        For Each tks As TableKeySchema In table.ForeignKeys
            If tks.ForeignKeyMemberColumns.Contains(mcs) Then
                Return tks.PrimaryKeyTable.Name
            End If
        Next
        Throw New Exception([String].Format("Could not find Column {0} in Table {1}'s ForeignKeys.", mcs.Name, table.Name))
    End Function

    Public Function GetUnitTestInitialization(ByVal type As Type) As String
        Dim result As String

        If type.Equals(GetType(String)) Then
            result = """ABC"""
        ElseIf type.Equals(GetType(DateTime)) Then
            result = "DateTime.Now"
        Else
            result = "Nothing"
        End If
        Return result
    End Function
End Class

#Region "SearchCriteria Class"

Public Class SearchCriteria
#Region "Static Content"

    Public Shared Function GetAllSearchCriteria(ByVal table As TableSchema) As List(Of SearchCriteria)
        Dim tsc As New TableSearchCriteria(table)
        Return tsc.GetAllSearchCriteria()
    End Function
    Public Shared Function GetPrimaryKeySearchCriteria(ByVal table As TableSchema) As List(Of SearchCriteria)
        Dim tsc As New TableSearchCriteria(table)
        Return tsc.GetPrimaryKeySearchCriteria()
    End Function
    Public Shared Function GetForeignKeySearchCriteria(ByVal table As TableSchema) As List(Of SearchCriteria)
        Dim tsc As New TableSearchCriteria(table)
        Return tsc.GetForeignKeySearchCriteria()
    End Function
    Public Shared Function GetIndexSearchCriteria(ByVal table As TableSchema) As List(Of SearchCriteria)
        Dim tsc As New TableSearchCriteria(table)
        Return tsc.GetIndexSearchCriteria()
    End Function
#End Region

#Region "Declarations"

    Protected mcsList As List(Of MemberColumnSchema)
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

    Protected Sub Add(ByVal item As MemberColumnSchema)
        mcsList.Add(item)
    End Sub
    Public Overloads Overrides Function ToString() As String
        Dim sb As New StringBuilder()
        Dim isFirst As Boolean = True

        For Each mcs As MemberColumnSchema In mcsList
            If isFirst Then
                isFirst = False
            Else
                sb.Append(" & ")
            End If
            sb.Append(mcs.Name)
        Next

        Return sb.ToString()
    End Function
    Public Function ContainsForeignKey(ByVal tsc As TableSchemaCollection) As Boolean
        For Each ts As TableSchema In tsc
            For Each tks As TableKeySchema In ts.PrimaryKeys
                For Each mcs As MemberColumnSchema In mcsList
                    If tks.PrimaryKeyMemberColumns.Contains(mcs) Then
                        Return True
                    End If
                Next
            Next
        Next
        Return False
    End Function
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
    Protected ReadOnly Property Key() As String
        Get
            Return Me.ToString()
        End Get
    End Property
#End Region

#Region "Internal Classes"

    Friend Class TableSearchCriteria
#Region "Declarations"

        Protected _table As TableSchema
#End Region

#Region "Constructor"

        Public Sub New(ByVal sourceTable As TableSchema)
            Me._table = sourceTable
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

