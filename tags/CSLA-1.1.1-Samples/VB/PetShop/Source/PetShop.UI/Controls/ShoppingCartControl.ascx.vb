Imports PetShop.Business
Imports PetShop.Controls


Partial Public Class ShoppingCartControl
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

        Dim items As Business.CartList = profile.ShoppingCart
        If items.Count > 0 Then
            repShoppingCart.DataSource = items
            repShoppingCart.DataBind()
            PrintTotal()
            plhTotal.Visible = True
        Else
            repShoppingCart.Visible = False
            plhTotal.Visible = False
            lblMsg.Text = "Your cart is empty."
        End If
    End Sub

    ''' <summary>
    ''' Recalculate the total
    ''' </summary>
    Private Sub PrintTotal()
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name)
        If profile.ShoppingCart.Count > 0 Then
            ltlTotal.Text = profile.ShoppingCart.Total.ToString("c")
        End If
    End Sub

    ''' <summary>
    ''' Calculate total
    ''' </summary>
    Protected Sub BtnTotal_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name)

        For Each row As RepeaterItem In repShoppingCart.Items
            Dim txtQuantity As TextBox = DirectCast(row.FindControl("txtQuantity"), TextBox)
            Dim btnDelete As ImageButton = DirectCast(row.FindControl("btnDelete"), ImageButton)

            Dim quantity As Integer
            If Integer.TryParse(WebUtility.InputText(txtQuantity.Text, 10), quantity) Then

                If quantity > 0 Then
                    profile.ShoppingCart.SetQuantity(btnDelete.CommandArgument, quantity)
                ElseIf quantity = 0 Then
                    profile.ShoppingCart.Remove(btnDelete.CommandArgument)
                End If
            End If
        Next

        profile = profile.Save(True)

        BindCart()
    End Sub

    ''' <summary>
    ''' Handler for Delete/Move buttons
    ''' </summary>
    Protected Sub CartItem_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name)
        Select Case e.CommandName
            Case "Del"
                profile.ShoppingCart.Remove(e.CommandArgument.ToString())
                Exit Select
            Case "Move"
                profile.ShoppingCart.Remove(e.CommandArgument.ToString())
                profile.WishList.Add(e.CommandArgument.ToString(), profile.UniqueID, False)
                Exit Select
        End Select

        profile = profile.Save(True)

        BindCart()
    End Sub

End Class