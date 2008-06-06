<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DropDownEditorPropertyEditorControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.SampleStringTextBox = New System.Windows.Forms.TextBox
        Me.SampleBooleanCheckBox = New System.Windows.Forms.CheckBox
        Me.SuspendLayout()
        '
        'SampleStringTextBox
        '
        Me.SampleStringTextBox.Location = New System.Drawing.Point(20, 12)
        Me.SampleStringTextBox.Name = "SampleStringTextBox"
        Me.SampleStringTextBox.Size = New System.Drawing.Size(100, 20)
        Me.SampleStringTextBox.TabIndex = 1
        Me.SampleStringTextBox.Text = "Hello World!"
        '
        'SampleBooleanCheckBox
        '
        Me.SampleBooleanCheckBox.Location = New System.Drawing.Point(8, 38)
        Me.SampleBooleanCheckBox.Name = "SampleBooleanCheckBox"
        Me.SampleBooleanCheckBox.Size = New System.Drawing.Size(112, 24)
        Me.SampleBooleanCheckBox.TabIndex = 2
        Me.SampleBooleanCheckBox.Text = "Sample Boolean"
        '
        'DropDownEditorPropertyEditorControl
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.Controls.Add(Me.SampleBooleanCheckBox)
        Me.Controls.Add(Me.SampleStringTextBox)
        Me.Name = "DropDownEditorPropertyEditorControl"
        Me.Size = New System.Drawing.Size(112, 56)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents SampleStringTextBox As System.Windows.Forms.TextBox
    Public WithEvents SampleBooleanCheckBox As System.Windows.Forms.CheckBox

End Class
