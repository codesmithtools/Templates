Imports System.Web.DynamicData

Partial Class ForeignKey_EditField
    Inherits System.Web.DynamicData.FieldTemplateUserControl

        
    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return DropDownList1
        End Get
    End Property
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If (DropDownList1.Items.Count = 0) Then
            If Not Column.IsRequired Then
                DropDownList1.Items.Add(New ListItem("[Not Set]", ""))
            End If
            PopulateListControl(DropDownList1)
        End If
    End Sub
    
    Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)
        MyBase.OnDataBinding(e)
        If (Mode = DataBoundControlMode.Edit) Then
            Dim foreignkey As String = ForeignKeyColumn.GetForeignKeyString(Row)
            Dim item As ListItem = DropDownList1.Items.FindByValue(foreignkey)
            If (Not (item) Is Nothing) Then
                DropDownList1.SelectedValue = foreignkey
            End If
        End If
    End Sub
    
    Protected Overrides Sub ExtractValues(ByVal dictionary As IOrderedDictionary)
        ' If it's an empty string, change it to null
        Dim val As String = DropDownList1.SelectedValue
        If (val = String.Empty) Then
            val = Nothing
        End If
        ExtractForeignKey(dictionary, val)
    End Sub


End Class
