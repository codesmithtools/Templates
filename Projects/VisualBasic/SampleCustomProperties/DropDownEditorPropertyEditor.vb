'------------------------------------------------------------------------------
'
' Copyright (c) 2002-2008 CodeSmith Tools, LLC.  All rights reserved.
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
Imports System.Windows.Forms
Imports System.Windows.Forms.Design
Imports System.Drawing.Design

Namespace CodeSmith.Samples
    Public Class DropDownEditorPropertyEditor
        Inherits UITypeEditor
        Dim editorService As IWindowsFormsEditorService
        Dim _dropDownEditorPropertyEditorControl As DropDownEditorPropertyEditorControl

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Function EditValue(ByVal context As ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
            If Not IsNothing(provider) Then
                editorService = provider.GetService(GetType(IWindowsFormsEditorService))
                If Not IsNothing(editorService) Then
                    If IsNothing(_dropDownEditorPropertyEditorControl) Then
                        _dropDownEditorPropertyEditorControl = New DropDownEditorPropertyEditorControl()
                    End If

                    _dropDownEditorPropertyEditorControl.Start(editorService, value)
                    editorService.DropDownControl(_dropDownEditorPropertyEditorControl)

                    Return New DropDownEditorProperty(_dropDownEditorPropertyEditorControl.SampleStringTextBox.Text, _dropDownEditorPropertyEditorControl.SampleBooleanCheckBox.Checked)
                End If
            End If

            Return value
        End Function

        Public Overrides Function GetEditStyle(ByVal context As ITypeDescriptorContext) As UITypeEditorEditStyle
            Return UITypeEditorEditStyle.DropDown
        End Function
    End Class
		
End Namespace