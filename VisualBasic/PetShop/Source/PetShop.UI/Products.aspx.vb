Imports PetShop.Controls

Partial Public Class Products
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'get page header and title
        Page.Title = WebUtility.GetCategoryName(Request.QueryString("categoryId"))
    End Sub

End Class