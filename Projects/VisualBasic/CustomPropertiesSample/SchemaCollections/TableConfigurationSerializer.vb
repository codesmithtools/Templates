'------------------------------------------------------------------------------
'
' Copyright (c) 2002-2011 CodeSmith Tools, LLC.  All rights reserved.
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
Imports System.Xml
Imports System.Xml.XPath
Imports CodeSmith.Engine
Imports CodeSmith.Engine.Schema
Imports SchemaExplorer
Imports SchemaExplorer.Serialization

Namespace CodeSmith.Samples

    Public Class TableConfigurationSerializer
        Implements IPropertySerializer

        Public Function SaveProperty(ByVal context As PropertySerializerContext, ByVal propertyValue As Object) As Object Implements IPropertySerializer.SaveProperty
            Return propertyValue
        End Function

        Public Function LoadProperty(ByVal context As PropertySerializerContext, ByVal propertyValue As Object) As Object Implements IPropertySerializer.LoadProperty
            Return propertyValue
        End Function

        Public Sub WritePropertyXml(ByVal context As PropertySerializerContext, ByVal writer As XmlWriter, ByVal propertyValue As Object) Implements IPropertySerializer.WritePropertyXml
            Dim configuration As TableConfiguration = TryCast(propertyValue, TableConfiguration)

            If Not configuration Is Nothing Then
                Dim database As DatabaseSchema = Nothing
                If Not configuration.SourceTable Is Nothing Then
                    database = configuration.SourceTable.Database
                ElseIf Not configuration.SourceView Is Nothing Then
                    database = configuration.SourceView.Database
                End If

                If Not database Is Nothing Then
                    writer.WriteStartElement("DatabaseSchema")
                    Dim dss As New DatabaseSchemaSerializer()
                    dss.WritePropertyXml(context, writer, database)
                    writer.WriteEndElement()
                End If

                If Not configuration.SourceTable Is Nothing Then
                    writer.WriteStartElement("SourceTable")
                    writer.WriteElementString("Owner", configuration.SourceTable.Owner)
                    writer.WriteElementString("Name", configuration.SourceTable.Name)
                    writer.WriteEndElement()
                End If

                If Not configuration.SourceView Is Nothing Then
                    writer.WriteStartElement("SourceView")
                    writer.WriteElementString("Owner", configuration.SourceView.Owner)
                    writer.WriteElementString("Name", configuration.SourceView.Name)
                    writer.WriteEndElement()
                End If
            End If
        End Sub

        Public Function ReadPropertyXml(ByVal context As PropertySerializerContext, ByVal propertyValue As XmlNode) As Object Implements IPropertySerializer.ReadPropertyXml
            If Not context.PropertyInfo.PropertyType.ToString().Equals(GetType(TableConfiguration).ToString(), StringComparison.InvariantCultureIgnoreCase) Then
                Return Nothing
            End If
            Return ReadPropertyXmlInner(context, propertyValue)
        End Function

        Friend Function ReadPropertyXmlInner(ByVal context As PropertySerializerContext, ByVal propertyValue As XmlNode) As TableConfiguration
            Dim navigator As XPathNavigator = propertyValue.CreateNavigator()
            Dim manager As New XmlNamespaceManager(navigator.NameTable)
            Dim iterator As XPathNodeIterator

            ' Add the CodeSmith namespace
            manager.AddNamespace("cs", CodeSmithProject.DefaultNamespace)

            ' Get the DatabaseSchema
            Dim databaseSchema As DatabaseSchema
            iterator = DirectCast(navigator.Evaluate("cs:DatabaseSchema", manager), XPathNodeIterator)
            If iterator.MoveNext() Then
                Dim oNode As XmlNode = (DirectCast(iterator.Current, IHasXmlNode)).GetNode()
                databaseSchema = DirectCast(New DatabaseSchemaSerializer().ReadPropertyXml(context, oNode), DatabaseSchema)
            End If

            ' Get the SourceTable
            Dim sourceTable As TableSchema = Nothing
            If Not databaseSchema Is Nothing Then
                Dim strOwner As String = TryCast(navigator.Evaluate("string(cs:SourceTable/cs:Owner/text())", manager), String)
                Dim strName As String = TryCast(navigator.Evaluate("string(cs:SourceTable/cs:Name/text())", manager), String)

                If Not String.IsNullOrEmpty(strName) Then
                    sourceTable = If(Not String.IsNullOrEmpty(strOwner), databaseSchema.Tables(strOwner, strName), databaseSchema.Tables(strName))
                End If
            End If

            ' Get the SourceView
            Dim sourceView As ViewSchema = Nothing
            If Not databaseSchema Is Nothing Then
                Dim strOwner As String = TryCast(navigator.Evaluate("string(cs:SourceView/cs:Owner/text())", manager), String)
                Dim strName As String = TryCast(navigator.Evaluate("string(cs:SourceView/cs:Name/text())", manager), String)

                If Not String.IsNullOrEmpty(strName) Then
                    sourceView = If(Not String.IsNullOrEmpty(strOwner), databaseSchema.Views(strOwner, strName), databaseSchema.Views(strName))
                End If
            End If

            ' Create and return
            Return New TableConfiguration()
        End Function

        Public Function ParseDefaultValue(ByVal context As PropertySerializerContext, ByVal defaultValue As String) As Object Implements IPropertySerializer.ParseDefaultValue
            Return Nothing
        End Function
    End Class

End Namespace