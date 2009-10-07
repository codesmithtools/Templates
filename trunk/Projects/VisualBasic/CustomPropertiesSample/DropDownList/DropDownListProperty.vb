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
Imports System.Collections.Generic
Imports System.ComponentModel

Namespace DropDownList

    <Editor(GetType(DropDownListPropertyEditor), GetType(System.Drawing.Design.UITypeEditor))> _
    Public Class DropDownListProperty
        Private _values As New List(Of String)()
        Private _selectedItem As String

        Public Sub New()
            SelectedItem = "None"
        End Sub

        Public Sub New(ByVal values As List(Of [String]))
            If values.Count > 0 Then
                SelectedItem = values(0)
            Else
                SelectedItem = "None"
            End If

            values = values
        End Sub

        Public Property Values() As List(Of String)
            Get
                If _values Is Nothing Then
                    _values = New List(Of [String])()
                End If

                Return _values
            End Get
            Set(ByVal value As List(Of String))
                If Not value Is Nothing Then
                    _values = value
                End If
            End Set
        End Property

        <Browsable(False)> _
        Public Property SelectedItem() As String
            Get
                Return _selectedItem
            End Get
            Set(ByVal value As String)
                _selectedItem = value
            End Set
        End Property

        ''' <summary>
        ''' The value that we return here will be shown in the property grid.
        ''' </summary>
        ''' <returns></returns>
        Public Overloads Overrides Function ToString() As String
            Return SelectedItem
        End Function
    End Class

End Namespace