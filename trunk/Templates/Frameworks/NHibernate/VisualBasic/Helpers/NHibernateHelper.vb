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
Public Enum VisualStudioVersion
	VS_2005
	VS_2008
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

	#region "Variable & Class Name Methods"

	Public Function GetPropertyName(ByVal table As TableSchema, ByVal column As ColumnSchema) As String
		If ColumnHasAlias(column) Then
			Return GetPropertyName(GetNameFromColumn(column))
		Else
			Return GetPropertyName(GetClassName(table))
		End If
	End Function
	Public Function GetPropertyName(ByVal column As ColumnSchema) As String
		Return GetPropertyName(GetNameFromColumn(column))
	End Function
	Private Function GetPropertyName(ByVal name As String) As String
		Return StringUtil.ToSingular(StringUtil.ToPascalCase(name))
	End Function

	Public Function GetPropertyNamePlural(ByVal table As TableSchema, ByVal column As ColumnSchema) As String
		If ColumnHasAlias(column) Then
			Return GetPropertyNamePlural(GetNameFromColumn(column))
		Else
			Return GetPropertyNamePlural(GetClassName(table))
		End If
	End Function
	Public Function GetPropertyNamePlural(ByVal column As ColumnSchema) As String
		Return GetPropertyNamePlural(GetNameFromColumn(column))
	End Function
	Private Function GetPropertyNamePlural(ByVal name As String) As String
		Return StringUtil.ToPlural(StringUtil.ToPascalCase(name))
	End Function

	Public Function GetPrivateVariableName(ByVal table As TableSchema, ByVal column As ColumnSchema) As String
		If ColumnHasAlias(column) Then
			Return GetPrivateVariableName(GetNameFromColumn(column))
		Else
			Return GetPrivateVariableName(GetClassName(table))
		End If
	End Function
	Public Function GetPrivateVariableName(ByVal column As ColumnSchema) As String
		Return GetPrivateVariableName(GetNameFromColumn(column))
	End Function
	Private Function GetPrivateVariableName(ByVal name As String) As String
		Return "p_" + GetVariableName(name)
	End Function

	Public Function GetPrivateVariableNamePlural(ByVal table As TableSchema, ByVal column As ColumnSchema) As String
		If ColumnHasAlias(column) Then
			Return GetPrivateVariableNamePlural(GetNameFromColumn(column))
		Else
			Return GetPrivateVariableNamePlural(GetClassName(table))
		End If
	End Function
	Public Function GetPrivateVariableNamePlural(ByVal column As ColumnSchema) As String
		Return GetPrivateVariableNamePlural(GetNameFromColumn(column))
	End Function
	Private Function GetPrivateVariableNamePlural(ByVal name As String) As String
		Return "p_" + GetVariableNamePlural(name)
	End Function

	Public Function GetVariableName(ByVal table As TableSchema, ByVal column As ColumnSchema) As String
		If ColumnHasAlias(column) Then
			Return GetVariableName(GetNameFromColumn(column))
		Else
			Return GetVariableName(GetClassName(table))
		End If
	End Function
	Public Function GetVariableName(ByVal column As ColumnSchema) As String
		Return GetVariableName(GetNameFromColumn(column))
	End Function
	Private Function GetVariableName(ByVal name As String) As String
		Return "_" + StringUtil.ToSingular(StringUtil.ToCamelCase(name))
	End Function

	Public Function GetVariableNamePlural(ByVal table As TableSchema, ByVal column As ColumnSchema) As String
		If ColumnHasAlias(column) Then
			Return GetVariableNamePlural(GetNameFromColumn(column))
		Else
			Return GetVariableNamePlural(GetClassName(table))
		End If
	End Function
	Public Function GetVariableNamePlural(ByVal column As ColumnSchema) As String
		Return GetVariableNamePlural(GetNameFromColumn(column))
	End Function
	Private Function GetVariableNamePlural(ByVal name As String) As String
		Return "_" + StringUtil.ToPlural(StringUtil.ToCamelCase(name))
	End Function

	Public Function GetClassName(ByVal table As TableSchema) As String
		Dim className As String
		If table.ExtendedProperties.Contains(extendedPropertyName) Then
			className = table.ExtendedProperties(extendedPropertyName).Value.ToString()
		Else
			className = table.Name

			If Not [String].IsNullOrEmpty(tablePrefix) AndAlso className.StartsWith(tablePrefix) Then
				className = className.Remove(0, tablePrefix.Length)
			End If
		End If

		Return StringUtil.ToSingular(StringUtil.ToPascalCase(className))
	End Function
	Protected tablePrefix As String = [String].Empty

	Private Function ColumnHasAlias(ByVal column As ColumnSchema) As Boolean
		Return column.ExtendedProperties.Contains(extendedPropertyName)
	End Function
	Private Function GetNameFromColumn(ByVal column As ColumnSchema) As String
		If ColumnHasAlias(column) Then
			Return column.ExtendedProperties(extendedPropertyName).Value.ToString()
		Else
			Return column.Name
		End IF
	End Function
	Private Const extendedPropertyName As String = "cs_alias"
#End Region

	#region "ManyToMany Table Methods"

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

	#region "BusinessObject Methods"

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

	#region "PrimaryKey Methods"

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
#End Region

	#region "Method Creation Methods"

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
			result.Append(GetVariableName(mcs))
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
			System.Math.Max(System.Threading.Interlocked.Increment(x),x - 1)
		End While
		Return GetMethodParameters(mcsList, isDeclaration)
	End Function
	Public Function GetMethodDeclaration(ByVal sc As SearchCriteria) As String
		Dim result As New StringBuilder()
		result.Append(sc.MethodName)
		result.Append("(")
		result.Append(GetMethodParameters(sc.Items, True))
		result.Append(")")
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
			System.Math.Max(System.Threading.Interlocked.Increment(x),x - 1)
		End While
		Return result.ToString()
	End Function
#End Region

	Public Function GetForeignTable(ByVal mcs As MemberColumnSchema, ByVal table As TableSchema) As TableSchema
		For Each tks As TableKeySchema In table.ForeignKeys
			If tks.ForeignKeyMemberColumns.Contains(mcs) Then
				Return tks.PrimaryKeyTable
			End If
		Next
		Throw New Exception([String].Format("Could not find Column {0} in Table {1}'s ForeignKeys.", mcs.Name, table.Name))
	End Function

	Protected random As New Random()
	Public Function GetUnitTestInitialization(ByVal column As ColumnSchema) As String
		Dim result As String

		If column.SystemType.Equals(GetType(String)) Then
			Dim sb As New StringBuilder()

			Dim size As Integer = 10
			If (column.Size > 0 AndAlso column.Size < 100) Then
				size = random.[Next](1, column.Size)
			End If

			sb.Append("""")
			Dim x As Integer = 0
			While x < size
				Select Case x Mod 5
					Case 0
						sb.Append("T")
						Exit Select
					Case 1
						sb.Append("e")
						Exit Select
					Case 2
						sb.Append("s")
						Exit Select
					Case 3
						sb.Append("t")
						Exit Select
					Case 4
						sb.Append(" ")
						Exit Select
				End Select
				System.Math.Max(System.Threading.Interlocked.Increment(x),x - 1)
			End While
			sb.Append("""")

			result = sb.ToString()
ElseIf column.SystemType.Equals(GetType(DateTime)) Then
			result = "DateTime.Now"
ElseIf column.SystemType.Equals(GetType(Decimal)) Then
			result = Convert.ToDecimal(random.[Next](1, 100)).ToString()
ElseIf column.SystemType.Equals(GetType(Int32)) Then
			result = random.[Next](1, 100).ToString()
ElseIf column.SystemType.Equals(GetType(Boolean)) Then
			result = (random.[Next](1, 2).Equals(1)).ToString()
ElseIf column.SystemType.IsPrimitive Then
			result = [String].Format("default({0})", column.SystemType.Name.ToString())
		Else
			result = "null"
		End If

		Return result
	End Function

	Public Function ContainsForeignKey(ByVal sc As SearchCriteria, ByVal tsc As TableSchemaCollection) As Boolean
		For Each ts As TableSchema In tsc
			For Each tks As TableKeySchema In ts.PrimaryKeys
				For Each mcs As MemberColumnSchema In sc.Items
					If tks.PrimaryKeyMemberColumns.Contains(mcs) Then
						Return True
					End If
				Next
			Next
		Next
		Return False
	End Function
End Class

#region "SearchCriteria Class"

Public Class SearchCriteria
	#region "Static Content"

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

	#region "Declarations"

	Protected mcsList As List(Of MemberColumnSchema)
	Protected _methodNameGenerationMode As MethodNameGenerationMode = MethodNameGenerationMode.[Default]
	Protected _methodName As String = [String].Empty
	Protected _extendedProperty As String
#End Region

	#region "Constructors"

	Protected Sub New(ByVal _extendedProperty As String)
		Me._extendedProperty = _extendedProperty
		mcsList = New List(Of MemberColumnSchema)()
	End Sub
	Protected Sub New(ByVal _extendedProperty As String, ByVal mcsList As List(Of MemberColumnSchema))
		Me._extendedProperty = _extendedProperty
		Me.mcsList = mcsList
	End Sub
#End Region

	#region "Methods"

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
	''' <param name="_extendedProperty">Value of the Extended Property.</param>
	Public Sub SetMethodNameGeneration(ByVal _extendedProperty As String)
		_methodNameGenerationMode = MethodNameGenerationMode.ExtendedProperty

		_methodName = _extendedProperty
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

			If mcs.ExtendedProperties.Contains(_extendedProperty) Then
				sb.Append(mcs.ExtendedProperties(_extendedProperty).Value.ToString())
			Else
				sb.Append(mcs.Name)
			End If
		Next
		sb.Append(suffix)

		_methodName = sb.ToString()
	End Sub
#End Region

	#region "Properties"

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

	#region "Enums & Classes"

	Public Enum MethodNameGenerationMode
		[Default]
		ExtendedProperty
		[Custom]
	End Enum

	Friend Class TableSearchCriteria
		#region "Declarations"

		Protected _table As TableSchema
		Protected _extendedProperty As String = [String].Empty
		Protected Const defaultExtendedProperty As String = "cs_alias"
#End Region

		#region "Constructor"

		Public Sub New(ByVal sourceTable As TableSchema)
			Me._table = sourceTable
		End Sub
		Public Sub New(ByVal sourceTable As TableSchema, ByVal _extendedProperty As String)
			Me.New(sourceTable)
			Me._extendedProperty = _extendedProperty
		End Sub
#End Region

		#region "Methods"

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
			Dim mcsList As New List(Of MemberColumnSchema)(_table.PrimaryKey.MemberColumns.ToArray())
			Dim searchCriteria As New SearchCriteria(ExtendedProperty, mcsList)

			If _table.PrimaryKey.ExtendedProperties.Contains(ExtendedProperty) Then
				If Not [String].IsNullOrEmpty(ExtendedProperty) AndAlso _table.PrimaryKey.ExtendedProperties.Contains(ExtendedProperty) AndAlso _table.PrimaryKey.ExtendedProperties(ExtendedProperty).Value <> Nothing Then
					searchCriteria.SetMethodNameGeneration(_table.PrimaryKey.ExtendedProperties(ExtendedProperty).Value.ToString())
				End If
			End If

			AddToMap(map, searchCriteria)
		End Sub
		Protected Sub GetForeignKeySearchCriteria(ByVal map As Dictionary(Of String, SearchCriteria))
			For Each tks As TableKeySchema In _table.ForeignKeys
				Dim searchCriteria As New SearchCriteria(ExtendedProperty)
				For Each mcs As MemberColumnSchema In tks.ForeignKeyMemberColumns
					If mcs.Table.Equals(_table) Then
						searchCriteria.Add(mcs)
					End If
				Next

				If Not [String].IsNullOrEmpty(ExtendedProperty) AndAlso tks.ExtendedProperties.Contains(ExtendedProperty) AndAlso tks.ExtendedProperties(ExtendedProperty).Value <> Nothing Then
					searchCriteria.SetMethodNameGeneration(tks.ExtendedProperties(ExtendedProperty).Value.ToString())
				End If

				AddToMap(map, searchCriteria)
			Next
		End Sub
		Protected Sub GetIndexSearchCriteria(ByVal map As Dictionary(Of String, SearchCriteria))
			For Each indexSchema As IndexSchema In _table.Indexes
				Dim searchCriteria As New SearchCriteria(ExtendedProperty)
				For Each mcs As MemberColumnSchema In indexSchema.MemberColumns
					If mcs.Table.Equals(_table) Then
						searchCriteria.Add(mcs)
					End If
				Next

				If Not [String].IsNullOrEmpty(ExtendedProperty) AndAlso indexSchema.ExtendedProperties.Contains(ExtendedProperty) AndAlso indexSchema.ExtendedProperties(ExtendedProperty).Value <> Nothing Then
					searchCriteria.SetMethodNameGeneration(indexSchema.ExtendedProperties(ExtendedProperty).Value.ToString())
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

		#region "Properties"

		Public ReadOnly Property ExtendedProperty() As String
			Get
				If [String].IsNullOrEmpty(_extendedProperty) Then
					Return defaultExtendedProperty
				Else
					Return _extendedProperty
				End If
			End Get
		End Property
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
