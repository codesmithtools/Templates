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
Imports System.Windows.Forms.Design

Namespace CodeSmith.Samples

    ''' <summary>
    ''' Summary description for DropDownEditorPropertyEditorControl.
    ''' </summary>
    Public Class DropDownEditorPropertyEditorControl
        Inherits System.Windows.Forms.UserControl
        Public SampleStringTextBox As System.Windows.Forms.TextBox
        Public SampleBooleanCheckBox As System.Windows.Forms.CheckBox
        ''' <summary> 
        ''' Required designer variable.
        ''' </summary>
        Dim components As System.ComponentModel.Container

        Public Sub New()
            ' This call is required by the Windows.Forms Form Designer.
            Me.InitializeComponent()

            ' TODO: Add any initialization after the InitializeComponent call

        End Sub


        Public Sub Start(ByRef editorService As IWindowsFormsEditorService, ByVal value As Object)
            If value Is GetType(DropDownEditorProperty) Then
                Me.SampleStringTextBox.Text = value.SampleString
                Me.SampleBooleanCheckBox.Checked = value.SampleBoolean
            End If
        End Sub

        ''' <summary> 
        ''' Clean up any resources being used.
        ''' </summary>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If Not IsNothing(disposing) Then
                If Not IsNothing(components) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Component Designer generated code"
        ''' <summary> 
        ''' Required method for Designer support - do not modify 
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.SampleStringTextBox = New System.Windows.Forms.TextBox()
            Me.SampleBooleanCheckBox = New System.Windows.Forms.CheckBox()
            Me.SuspendLayout()
            ' 
            ' SampleStringTextBox
            ' 
            Me.SampleStringTextBox.Location = New System.Drawing.Point(8, 8)
            Me.SampleStringTextBox.Name = "SampleStringTextBox"
            Me.SampleStringTextBox.TabIndex = 0
            Me.SampleStringTextBox.Text = "Hello World!"
            ' 
            ' SampleBooleanCheckBox
            ' 
            Me.SampleBooleanCheckBox.Location = New System.Drawing.Point(8, 32)
            Me.SampleBooleanCheckBox.Name = "SampleBooleanCheckBox"
            Me.SampleBooleanCheckBox.Size = New System.Drawing.Size(112, 24)
            Me.SampleBooleanCheckBox.TabIndex = 1
            Me.SampleBooleanCheckBox.Text = "Sample Boolean"
            ' 
            ' DropDownEditorPropertyEditorControl
            ' 
            Me.Controls.Add(Me.SampleBooleanCheckBox)
            Me.Controls.Add(Me.SampleStringTextBox)
            Me.Name = "DropDownEditorPropertyEditorControl"
            Me.Size = New System.Drawing.Size(112, 56)
            Me.ResumeLayout(False)
        End Sub
#End Region
    End Class

End Namespace

