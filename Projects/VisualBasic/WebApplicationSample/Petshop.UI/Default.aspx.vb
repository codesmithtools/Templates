Imports System.Web.DynamicData

Partial Class _Default
    Inherits System.Web.UI.Page

        
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Dim visibleTables As System.Collections.IList = MetaModel.Default.VisibleTables
        If (visibleTables.Count = 0) Then
            Throw New InvalidOperationException("There are no accessible tables. Make sure that at least one data model is registered in Global.asax a"& _ 
                "nd scaffolding is enabled or implement custom pages.")
        End If
        Menu1.DataSource = visibleTables
        Menu1.DataBind
    End Sub


End Class
