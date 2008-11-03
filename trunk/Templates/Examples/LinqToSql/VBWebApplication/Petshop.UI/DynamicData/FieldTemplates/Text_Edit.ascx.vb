Imports System.Web.DynamicData

Partial Class Text_EditField
    Inherits System.Web.DynamicData.FieldTemplateUserControl

        
    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return TextBox1
        End Get
    End Property
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        TextBox1.MaxLength = Column.MaxLength
        If (Column.MaxLength < 20) Then
            TextBox1.Columns = Column.MaxLength
        End If
        TextBox1.ToolTip = Column.Description
        SetUpValidator(RequiredFieldValidator1)
        SetUpValidator(RegularExpressionValidator1)
        SetUpValidator(DynamicValidator1)
    End Sub
    
    Protected Overrides Sub ExtractValues(ByVal dictionary As IOrderedDictionary)
        dictionary(Column.Name) = ConvertEditedValue(TextBox1.Text)
    End Sub


End Class
