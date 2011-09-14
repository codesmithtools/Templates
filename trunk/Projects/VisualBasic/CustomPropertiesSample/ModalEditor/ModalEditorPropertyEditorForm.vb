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
    ''' Summary description for ModalEditorPropertyEditorForm.
    ''' </summary>
    Public Class ModalEditorPropertyEditorForm
        Inherits System.Windows.Forms.Form
        Private button1 As System.Windows.Forms.Button
        Public SampleBooleanCheckBox As System.Windows.Forms.CheckBox
        Public SampleStringTextBox As System.Windows.Forms.TextBox
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.Container

        Public Sub New()
            '
            ' Required for Windows Form Designer support
            '
            InitializeComponent()
            '
            ' TODO: Add any constructor code after InitializeComponent call
            ''
        End Sub

        Public Sub Start(ByVal editorService As IWindowsFormsEditorService, ByVal value As Object)
            If value Is GetType(ModalEditorProperty) Then
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

#Region "Windows Form Designer generated code"
        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.button1 = New System.Windows.Forms.Button()
            Me.SampleBooleanCheckBox = New System.Windows.Forms.CheckBox()
            Me.SampleStringTextBox = New System.Windows.Forms.TextBox()
            Me.SuspendLayout()
            ' 
            ' button1
            ' 
            Me.button1.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.button1.Location = New System.Drawing.Point(32, 64)
            Me.button1.Name = "button1"
            Me.button1.TabIndex = 5
            Me.button1.Text = "OK"
            ' 
            ' SampleBooleanCheckBox
            ' 
            Me.SampleBooleanCheckBox.Location = New System.Drawing.Point(8, 32)
            Me.SampleBooleanCheckBox.Name = "SampleBooleanCheckBox"
            Me.SampleBooleanCheckBox.Size = New System.Drawing.Size(112, 24)
            Me.SampleBooleanCheckBox.TabIndex = 4
            Me.SampleBooleanCheckBox.Text = "Sample Boolean"
            ' 
            ' SampleStringTextBox
            ' 
            Me.SampleStringTextBox.Location = New System.Drawing.Point(8, 8)
            Me.SampleStringTextBox.Name = "SampleStringTextBox"
            Me.SampleStringTextBox.TabIndex = 3
            Me.SampleStringTextBox.Text = "Hello World!"
            ' 
            ' ModalEditorPropertyEditorForm
            ' 
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(115, 94)
            Me.ControlBox = False
            Me.Controls.Add(Me.button1)
            Me.Controls.Add(Me.SampleBooleanCheckBox)
            Me.Controls.Add(Me.SampleStringTextBox)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ModalEditorPropertyEditorForm"
            Me.ShowInTaskbar = False
            Me.Text = "Modal Editor"
            Me.ResumeLayout(False)

        End Sub
#End Region
    End Class

End Namespace