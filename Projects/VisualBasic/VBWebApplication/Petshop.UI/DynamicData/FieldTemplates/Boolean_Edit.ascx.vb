Imports System.Web.DynamicData

Partial Class Boolean_EditField
    Inherits System.Web.DynamicData.FieldTemplateUserControl

        
    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return CheckBox1
        End Get
    End Property
    
    Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)
        MyBase.OnDataBinding(e)
        Dim val As Object = FieldValue
        If (Not (val) Is Nothing) Then
            CheckBox1.Checked = CType(val,Boolean)
        End If
    End Sub
    
    Protected Overrides Sub ExtractValues(ByVal dictionary As IOrderedDictionary)
        dictionary(Column.Name) = CheckBox1.Checked
    End Sub


End Class
