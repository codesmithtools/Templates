
Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Xml.Serialization
Imports LinqToSqlShared.DbmlObjectModel
Imports LinqToSqlShared.Generator
Imports CodeSmith.Engine
Imports SchemaExplorer
Imports System.Text
Imports System.Collections

Namespace Manager
    Public Class ManagerGenerator
        Public Shared Function Create(ByVal databaseSchema As DatabaseSchema, ByVal database As Database) As DataManager
            Dim managerMapping As New DataManager(database)
            GetMethods(databaseSchema, database, managerMapping)
            Return managerMapping
        End Function
        
        Private Shared Sub GetMethods(ByVal databaseSchema As DatabaseSchema, ByVal database As Database, ByVal managerMapping As DataManager)
            For Each manager As EntityManager In managerMapping.Managers
                Dim tableMapping As Table = database.Tables(manager.TableName)
                Dim table As TableSchema
                Dim parts As String() = manager.TableName.Split("."C)
                
                If parts.Length = 2 Then
                    table = databaseSchema.Tables(parts(0), parts(1))
                Else
                    table = databaseSchema.Tables(manager.TableName)
                End If
                
                If table Is Nothing Then
                    Continue For
                End If
                
                If table.HasPrimaryKey Then
                    Dim method As ManagerMethod = GetMethodFromColumns(tableMapping, table.PrimaryKey.MemberColumns)
                    method.IsKey = True
                    If Not manager.Methods.Contains(method.NameSuffix) Then
                        manager.Methods.Add(method)
                    End If
                End If
                
                GetIndexes(manager, tableMapping, table)
                GetForienKeys(manager, tableMapping, table)
            Next
        End Sub
        
        Private Shared Sub GetForienKeys(ByVal manager As EntityManager, ByVal tableMapping As Table, ByVal table As TableSchema)
            Dim columns As New List(Of ColumnSchema)()
            
            For Each column As ColumnSchema In table.ForeignKeyColumns
                columns.Add(column)
                
                Dim method As ManagerMethod = GetMethodFromColumns(tableMapping, columns)
                If Not manager.Methods.Contains(method.NameSuffix) Then
                    manager.Methods.Add(method)
                End If
                
                columns.Clear()
            Next
        End Sub
        
        Private Shared Sub GetIndexes(ByVal manager As EntityManager, ByVal tableMapping As Table, ByVal table As TableSchema)
            For Each index As IndexSchema In table.Indexes
                Dim method As ManagerMethod = GetMethodFromColumns(tableMapping, index.MemberColumns)
                method.IsUnique = index.IsUnique
                
                If Not manager.Methods.Contains(method.NameSuffix) Then
                    manager.Methods.Add(method)
                End If
            Next
        End Sub
        
        Private Shared Function GetMethodFromColumns(ByVal tableMapping As Table, ByVal columns As IList) As ManagerMethod
            Dim method As New ManagerMethod()
            method.EntityName = tableMapping.Type.Name
            Dim methodName As String = String.Empty
            For Each column As ColumnSchema In columns
                Dim columnMapping As Column = tableMapping.Type.Columns(column.Name)
                method.Columns.Add(columnMapping)
                methodName += columnMapping.Member
            Next
            method.NameSuffix = methodName
            Return method
        End Function
        
    End Class
    
    Public Class DataManager
        Public Const ManagerSuffix As String = "Manager"
        Public Const DataManagerSuffix As String = "DataManager"
        
        Public Sub New(ByVal database As Database)
            Initialize(database)
        End Sub
        
        Public Sub New()
        End Sub
        
        Public Sub Initialize(ByVal database As Database)
            DataManagerName = StringUtil.ToPascalCase(database.Name) + DataManagerSuffix
            DataContextName = CommonUtility.GetClassName(database.[Class])
            For Each table As Table In database.Tables
                Managers.Add(New EntityManager(table))
            Next
        End Sub
        
        Public DataManagerName As String
        Public DataContextName As String
        
        Public Managers As New List(Of EntityManager)()
        
        Public Overloads Overrides Function ToString() As String
            Return DataManagerName
        End Function
    End Class
    
    
    Public Class EntityManager
        Public Sub New(ByVal table As Table)
            TableName = table.Name
            EntityName = table.Type.Name
			If String.IsNullOrEmpty(table.Member) THEN
            	PropertyName = table.Type.Name.ToString()
			Else
				PropertyName = table.Member.ToString()
			End If
            ManagerName = EntityName + DataManager.ManagerSuffix
            FieldName = CommonUtility.GetFieldName(ManagerName)
        End Sub
        
        Public Sub New()
        End Sub
        
        Public TableName As String
        Public EntityName As String
        Public ManagerName As String
        
        Public FieldName As String
        Public PropertyName As String
        
        Public Methods As New ManagerMethodCollection()
        
        Public Overloads Overrides Function ToString() As String
            Return ManagerName
        End Function
    End Class
    
    Public Class ManagerMethod
        Public NameSuffix As String
        Public EntityName As String
        Public IsKey As Boolean
        Public IsUnique As Boolean
        
        Public Columns As New List(Of Column)()
        
        Public Overloads Overrides Function ToString() As String
            Return NameSuffix
        End Function
    End Class
    
    Public Class ManagerMethodCollection
        Inherits KeyedCollection(Of String, ManagerMethod)
        Protected Overloads Overrides Function GetKeyForItem(ByVal item As ManagerMethod) As String
            Return item.NameSuffix
        End Function
    End Class
End Namespace
