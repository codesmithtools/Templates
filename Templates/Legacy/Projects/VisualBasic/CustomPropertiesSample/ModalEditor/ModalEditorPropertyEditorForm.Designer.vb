<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ModalEditorPropertyEditorForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.button1 = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'SampleStringTextBox
        '
        Me.SampleStringTextBox.Location = New System.Drawing.Point(12, 12)
        Me.SampleStringTextBox.Name = "SampleStringTextBox"
        Me.SampleStringTextBox.Size = New System.Drawing.Size(100, 20)
        Me.SampleStringTextBox.TabIndex = 4
        Me.SampleStringTextBox.Text = "Hello World!"
        '
        'SampleBooleanCheckBox
        '
        Me.SampleBooleanCheckBox.Location = New System.Drawing.Point(12, 38)
        Me.SampleBooleanCheckBox.Name = "SampleBooleanCheckBox"
        Me.SampleBooleanCheckBox.Size = New System.Drawing.Size(112, 24)
        Me.SampleBooleanCheckBox.TabIndex = 5
        Me.SampleBooleanCheckBox.Text = "Sample Boolean"
        '
        'button1
        '
        Me.button1.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.button1.Location = New System.Drawing.Point(37, 68)
        Me.button1.Name = "button1"
        Me.button1.Size = New System.Drawing.Size(75, 23)
        Me.button1.TabIndex = 6
        Me.button1.Text = "OK"
        '
        'ModalEditorPropertyEditorForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.ClientSize = New System.Drawing.Size(115, 94)
        Me.ControlBox = False
        Me.Controls.Add(Me.button1)
        Me.Controls.Add(Me.SampleBooleanCheckBox)
        Me.Controls.Add(Me.SampleStringTextBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "ModalEditorPropertyEditorForm"
        Me.Text = "Modal Editor"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents SampleStringTextBox As System.Windows.Forms.TextBox
    Public WithEvents SampleBooleanCheckBox As System.Windows.Forms.CheckBox
    Private WithEvents button1 As System.Windows.Forms.Button
End Class
