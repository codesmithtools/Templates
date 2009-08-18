Imports PetShop.Business

Partial Public Class ItemsControl
    Inherits System.Web.UI.UserControl

    ''' <summary>
    ''' Rebind control
    ''' </summary>
    Protected Sub PageChanged(ByVal sender As Object, ByVal e As DataGridPageChangedEventArgs)
        'reset index
        itemsGrid.CurrentPageIndex = e.NewPageIndex

        'get category id
        Dim productId As String = Request.QueryString("productId")

        itemsGrid.DataSource = Product.GetProduct(productId).Items
        itemsGrid.DataBind()
    End Sub

End Class