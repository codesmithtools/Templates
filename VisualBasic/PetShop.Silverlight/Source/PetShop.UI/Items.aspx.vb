Imports PetShop.Controls

Partial Public Class Items
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'get page header and title
        Page.Title = WebUtility.GetProductName(Request.QueryString("productId"))
    End Sub

End Class