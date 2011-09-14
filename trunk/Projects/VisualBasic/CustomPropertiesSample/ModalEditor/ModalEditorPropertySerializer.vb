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
Imports CodeSmith.Engine
Imports System.Xml.XPath
Imports System.Xml
Imports CodeSmith.Engine.Schema

Namespace CodeSmith.Samples

    ''' <summary>
    ''' This class provides CodeSmith serialization support for ModalEditorProperty.
    ''' </summary>
    Public Class ModalEditorPropertySerializer
        Implements IPropertySerializer

        Public Sub New()

        End Sub

        ''' <summary>
        ''' This method will be used to restore the property value after a template has been compiled.
        ''' </summary>
        ''' <param name="propertyInfo">Information about the target property.</param>
        ''' <param name="propertyValue">The property to be loaded.</param>
        ''' <returns>The value to be assigned to the template property after it has been compiled.</returns>
        Public Function LoadProperty(ByVal context As PropertySerializerContext, ByVal propertyValue As Object) As Object Implements IPropertySerializer.LoadProperty
            ' Nothing special needs to be done to load this property so we just return the unmodified property value.
            Return propertyValue
        End Function

        ''' <summary>
        ''' This method will be used to parse a default value for a property when a template is being instantiated.
        ''' </summary>
        ''' <param name="propertyInfo">Information about the target property.</param>
        ''' <param name="defaultValue">The default value.</param>
        ''' <param name="basePath">The path to use for resolving file references.</param>
        ''' <returns>An object that will be assigned to the template property.</returns>
        Public Function ParseDefaultValue(ByVal context As PropertySerializerContext, ByVal defaultValue As String) As Object Implements IPropertySerializer.ParseDefaultValue
            If context.PropertyInfo.PropertyType Is GetType(ModalEditorProperty) Then
                Dim modalEditorPropertyValue As ModalEditorProperty
                modalEditorPropertyValue = Nothing
                Dim values As Array
                values = defaultValue.Split(",")
                If values.Length = 2 Then
                    modalEditorPropertyValue.SampleString = values(0)
                    modalEditorPropertyValue.SampleBoolean = Boolean.Parse(values(1))
                    Return modalEditorPropertyValue
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' This method will be used when deserializing the property from an XML property set.
        ''' </summary>
        ''' <param name="propertyInfo">Information about the target property.</param>
        ''' <param name="propertyValue">The XML node to read the property value from.</param>
        ''' <param name="basePath">The path to use for resolving file references.</param>
        ''' <returns>The value to be assigned to the template property.</returns>
        Public Function ReadPropertyXml(ByVal context As PropertySerializerContext, ByVal propertyValue As System.Xml.XmlNode) As Object Implements IPropertySerializer.ReadPropertyXml
            If Not context.PropertyInfo.PropertyType Is GetType(ModalEditorProperty) Then
                Return Nothing
            End If

            Dim navigator As XPathNavigator
            Dim manager As XmlNamespaceManager
            Dim sampleBooleanExpression As XPathExpression
            Dim sampleTextExpression As XPathExpression
            Dim sampleString As String
            Dim ModalEditorPropertyValue As ModalEditorProperty

            ModalEditorPropertyValue = Nothing

            ' use XPath to select out values
            navigator = propertyValue.CreateNavigator()
            ' we need to import the CodeSmith Namespace
            manager = New XmlNamespaceManager(navigator.NameTable)
            manager.AddNamespace("cs", CodeSmithProject.DefaultNamespace)

            ' expresion to select SampleBoolean value
            sampleBooleanExpression = XPathExpression.Compile("string(cs:SampleBoolean/text())", manager)
            Dim boolString As String
            boolString = navigator.Evaluate(sampleBooleanExpression)
            Dim sampleBoolean As Boolean
            Boolean.TryParse(boolString, sampleBoolean)

            ' expresion to select SampleString value
            sampleTextExpression = XPathExpression.Compile("string(cs:SampleString/text())", manager)
            sampleString = navigator.Evaluate(sampleTextExpression)

            ModalEditorPropertyValue.SampleBoolean = sampleBoolean
            ModalEditorPropertyValue.SampleString = sampleString
            Return ModalEditorPropertyValue
        End Function

        ''' <summary>
        ''' This method will be used to save the property value when a template is being compiled.
        ''' </summary>
        ''' <param name="propertyInfo">Information about the target property.</param>
        ''' <param name="propertyValue">The property to be saved.</param>
        ''' <returns>An object that will be stored in a Hashtable during template compilation.</returns>
        Public Function SaveProperty(ByVal context As PropertySerializerContext, ByVal propertyValue As Object) As Object Implements IPropertySerializer.SaveProperty
            ' Nothing special needs to be done to save this property so we just return the unmodified property value.
            Return propertyValue
        End Function

        ''' <summary>
        ''' This method will be used when serializing the property value to an XML property set.
        ''' </summary>
        ''' <param name="propertyInfo">Information about the target property.</param>
        ''' <param name="writer">The XML writer that the property value will be written to.</param>
        ''' <param name="propertyValue">The property to be serialized.</param>
        Public Sub WritePropertyXml(ByVal context As PropertySerializerContext, ByVal writer As System.Xml.XmlWriter, ByVal propertyValue As Object) Implements IPropertySerializer.WritePropertyXml
            If IsNothing(propertyValue) Then
                Return
            End If
            Dim modalEditorPropertyValue As ModalEditorProperty
            modalEditorPropertyValue = propertyValue
            If Not IsNothing(modalEditorPropertyValue) Then
                writer.WriteElementString("SampleBoolean", modalEditorPropertyValue.SampleBoolean.ToString())
                writer.WriteElementString("SampleString", modalEditorPropertyValue.SampleString)
            End If
        End Sub
    End Class

End Namespace