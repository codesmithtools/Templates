Imports System.Web.DynamicData

Partial Class List
    Inherits System.Web.UI.Page

        
    Protected table As MetaTable
    
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        DynamicDataManager1.RegisterControl(GridView1, true)
    End Sub
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        table = GridDataSource.GetTable
        Title = table.DisplayName
        InsertHyperLink.NavigateUrl = table.GetActionPath(PageAction.Insert)
        ' Disable various options if the table is readonly
        If table.IsReadOnly Then
            GridView1.Columns(0).Visible = false
            InsertHyperLink.Visible = false
        End If
    End Sub
    
    Protected Sub OnFilterSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        GridView1.PageIndex = 0
    End Sub


End Class
