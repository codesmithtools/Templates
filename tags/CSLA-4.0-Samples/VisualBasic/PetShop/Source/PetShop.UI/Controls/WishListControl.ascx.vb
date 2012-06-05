Imports PetShop.Business

Partial Public Class WishListControl
    Inherits System.Web.UI.UserControl

    ''' <summary>
    ''' Handle Page load event
    ''' </summary>
    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsPostBack Then
            BindCart()
        End If
    End Sub

    ''' <summary>
    ''' Bind repeater to Cart object in Profile
    ''' </summary>
    Private Sub BindCart()
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name)

        Dim wishList As Business.CartList = profile.WishList
        If wishList.Count > 0 Then
            repWishList.DataSource = wishList
            repWishList.DataBind()
        Else
            repWishList.Visible = False
            lblMsg.Text = "Your wish list is empty."
        End If
    End Sub

    ''' <summary>
    ''' Handler for Delete/Move buttons
    ''' </summary>
    Protected Sub CartItem_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name)

        Select Case e.CommandName
            Case "Del"
                profile.WishList.Remove(e.CommandArgument.ToString())
                Exit Select
            Case "Move"
                profile.WishList.Remove(e.CommandArgument.ToString())
                profile.ShoppingCart.Add(e.CommandArgument.ToString(), profile.UniqueID, True)
                Exit Select
        End Select

        profile = profile.Save()

        BindCart()
    End Sub

End Class