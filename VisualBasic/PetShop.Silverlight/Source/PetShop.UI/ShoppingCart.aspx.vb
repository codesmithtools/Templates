Imports PetShop.Business

Partial Public Class ShoppingCart
    Inherits System.Web.UI.Page

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsPostBack Then
            Dim itemId As String = Request.QueryString("addItem")
            If Not String.IsNullOrEmpty(itemId) Then
                Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name)

                profile.ShoppingCart.Add(itemId, profile.UniqueID, True)
                profile = profile.Save()

                ' Redirect to prevent duplictations in the cart if user hits "Refresh"
                Response.Redirect("~/ShoppingCart.aspx", True)
            End If
        End If
    End Sub

End Class