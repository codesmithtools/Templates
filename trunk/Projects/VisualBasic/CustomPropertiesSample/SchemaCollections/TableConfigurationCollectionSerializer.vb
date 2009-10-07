'------------------------------------------------------------------------------
'
' Copyright (c) 2002-2009 CodeSmith Tools, LLC.  All rights reserved.
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

Namespace SchemaCollections

    Public Class TableConfigurationCollectionSerializer
        Implements IPropertySerializer

        Public Function SaveProperty(ByVal context As PropertySerializerContext, ByVal propertyValue As Object) As Object Implements IPropertySerializer.SaveProperty
            Return propertyValue
        End Function

        Public Function LoadProperty(ByVal context As PropertySerializerContext, ByVal propertyValue As Object) As Object Implements IPropertySerializer.LoadProperty
            Return propertyValue
        End Function

        Public Sub WritePropertyXml(ByVal context As PropertySerializerContext, ByVal writer As XmlWriter, ByVal propertyValue As Object) Implements IPropertySerializer.WritePropertyXml
            Dim collection As TableConfigurationCollection = TryCast(propertyValue, TableConfigurationCollection)

            If Not collection Is Nothing Then
                Dim itemSerializer As New TableConfigurationSerializer()

                For Each item As TableConfiguration In collection
                    writer.WriteStartElement("TableConfiguration")
                    itemSerializer.WritePropertyXml(context, writer, item)
                    writer.WriteEndElement()
                Next
            End If
        End Sub

        Public Function ReadPropertyXml(ByVal context As PropertySerializerContext, ByVal propertyValue As XmlNode) As Object Implements IPropertySerializer.ReadPropertyXml
            If Not context.PropertyInfo.PropertyType.ToString().Equals(GetType(TableConfigurationCollection).ToString(), StringComparison.InvariantCultureIgnoreCase) Then
                Return Nothing
            End If

            Dim navigator As XPathNavigator = propertyValue.CreateNavigator()
            Dim oManager As New XmlNamespaceManager(navigator.NameTable)
            Dim collection As New TableConfigurationCollection()
            Dim itemSerializer As New TableConfigurationSerializer()

            ' Add the CodeSmith namespace
            oManager.AddNamespace("cs", CodeSmithProject.DefaultNamespace)

            ' Loop through items
            Dim oIterator As XPathNodeIterator = navigator.Select("cs:TableConfiguration", oManager)
            While oIterator.MoveNext()
                collection.Add(itemSerializer.ReadPropertyXmlInner(context, (DirectCast(oIterator.Current, IHasXmlNode)).GetNode()))
            End While

            Return collection
        End Function

        Public Function ParseDefaultValue(ByVal context As PropertySerializerContext, ByVal defaultValue As String) As Object Implements IPropertySerializer.ParseDefaultValue
            Return New TableConfigurationCollection()
        End Function

    End Class

End Namespace