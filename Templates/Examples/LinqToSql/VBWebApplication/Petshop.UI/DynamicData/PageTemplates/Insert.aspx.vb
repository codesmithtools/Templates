Imports System.Web.DynamicData

Partial Class Insert
    Inherits System.Web.UI.Page

        
    Protected table As MetaTable
    
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        DynamicDataManager1.RegisterControl(DetailsView1)
    End Sub
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        table = DetailsDataSource.GetTable
        Title = table.DisplayName
    End Sub
    
    Protected Sub DetailsView1_ItemCommand(ByVal sender As Object, ByVal e As DetailsViewCommandEventArgs)
        If (e.CommandName = DataControlCommands.CancelCommandName) Then
            Response.Redirect(table.ListActionPath)
        End If
    End Sub
    
    Protected Sub DetailsView1_ItemInserted(ByVal sender As Object, ByVal e As DetailsViewInsertedEventArgs)
        If ((e.Exception Is Nothing)  _
                    OrElse e.ExceptionHandled) Then
            Response.Redirect(table.ListActionPath)
        End If
    End Sub


End Class
