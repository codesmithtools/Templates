Imports System
Imports System.ComponentModel
Imports System.Data
Imports System.Collections.Generic
Imports System.Text
Imports CodeSmith.Engine
Imports SchemaExplorer
Imports NHibernateHelper

Public Class VbNHibernateHelper
	Inherits NHibernateHelper.NHibernateHelper
	Private _keyWords As MapCollection
	Public ReadOnly Property KeyWords() As MapCollection
		Get
			If _keyWords Is Nothing Then
				Dim path As String
				Map.TryResolvePath("VBKeyWordEscape", Me.CodeTemplateInfo.DirectoryName, path)
				_keyWords = Map.Load(path)
			End If
			Return _keyWords
		End Get
	End Property
	
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
			result.Append(KeyWords(GetVariableName(mcs)))
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
End Class
