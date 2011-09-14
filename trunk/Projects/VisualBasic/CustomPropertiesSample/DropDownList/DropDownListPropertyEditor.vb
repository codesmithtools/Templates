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
Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.Windows.Forms
Imports System.Windows.Forms.Design

Namespace CodeSmith.Samples

    ''' <summary>
    ''' Provides a user interface for selecting a state property.
    ''' </summary>
    Public Class DropDownListPropertyEditor
        Inherits UITypeEditor

#Region "Members"

        Private _service As IWindowsFormsEditorService = Nothing

#End Region

        ''' <summary>
        ''' Displays a list of available values for the specified component than sets the value.
        ''' </summary>
        ''' <param name="context">An ITypeDescriptorContext that can be used to gain additional context information.</param>
        ''' <param name="provider">A service provider object through which editing services may be obtained.</param>
        ''' <param name="value">An instance of the value being edited.</param>
        ''' <returns>The new value of the object. If the value of the object hasn't changed, this method should return the same object it was passed.</returns>
        Public Overrides Function EditValue(ByVal context As ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
            If Not provider Is Nothing Then
                ' This service is in charge of popping our ListBox.
                _service = (DirectCast(provider.GetService(GetType(IWindowsFormsEditorService)), IWindowsFormsEditorService))

                If Not _service Is Nothing AndAlso TypeOf value Is DropDownListProperty Then
                    Dim [property] As DropDownListProperty = DirectCast(value, DropDownListProperty)

                    Dim list As New ListBox()

                    AddHandler list.Click, AddressOf ListBox_Click

                    For Each item As String In [property].Values
                        list.Items.Add(item)
                    Next

                    ' Drop the list control.
                    _service.DropDownControl(list)

                    If list.SelectedItem <> Nothing AndAlso list.SelectedIndices.Count = 1 Then
                        [property].SelectedItem = list.SelectedItem.ToString()
                        value = [property]
                    End If
                End If
            End If

            Return value
        End Function

        Private Sub ListBox_Click(ByVal sender As Object, ByVal e As EventArgs)
            If Not _service Is Nothing Then
                _service.CloseDropDown()
            End If
        End Sub

        ''' <summary>
        ''' Gets the editing style of the <see cref="EditValue"/> method.
        ''' </summary>
        ''' <param name="context">An ITypeDescriptorContext that can be used to gain additional context information.</param>
        ''' <returns>Returns the DropDown style, since this editor uses a drop down list.</returns>
        Public Overrides Function GetEditStyle(ByVal context As ITypeDescriptorContext) As UITypeEditorEditStyle
            ' We're using a drop down style UITypeEditor.
            Return UITypeEditorEditStyle.DropDown
        End Function
    End Class

End Namespace