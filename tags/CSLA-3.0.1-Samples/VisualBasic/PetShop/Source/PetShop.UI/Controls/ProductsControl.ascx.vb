Imports PetShop.Business

Partial Public Class ProductsControl
    Inherits System.Web.UI.UserControl

    ''' <summary>
    ''' Rebind control
    ''' </summary>
    Protected Sub PageChanged(ByVal sender As Object, ByVal e As DataGridPageChangedEventArgs)
        'reset index
        productsList.CurrentPageIndex = e.NewPageIndex

        'get category id
        Dim categoryId As String = Request.QueryString("categoryId")

        'bind data
        productsList.DataSource = Category.GetByCategoryId(categoryId).Products
        productsList.DataBind()
    End Sub

End Class