Imports PetShop.Controls
Imports PetShop.Business

Partial Public Class CheckOut
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name)
        If profile.IsAnonymous.Value Then
            Response.Redirect("~/SignIn.aspx")
        End If

        If billingForm.Address Is Nothing Then
            billingForm.Address = New Address(profile)
        End If
    End Sub

    ''' <summary>
    ''' Process the order
    ''' </summary>
    Protected Sub wzdCheckOut_FinishButtonClick(ByVal sender As Object, ByVal e As WizardNavigationEventArgs)
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name)
        If profile.ShoppingCart.Count > 0 Then
            ' display ordered items
            CartListOrdered.Bind(profile.ShoppingCart)

            ' display total and credit card information
            ltlTotalComplete.Text = ltlTotal.Text
            ltlCreditCardComplete.Text = ltlCreditCard.Text

            'Create Order
            Dim order As Order = order.NewOrder()

            order.UserId = profile.UniqueID.ToString()
            order.OrderDate = DateTime.Now
            order.CreditCard = GetCreditCard()
            order.Courier = order.CreditCard.CardType
            order.TotalPrice = profile.ShoppingCart.Total
            order.AuthorizationNumber = 0
            order.Locale = "en-us"

            'Shipping Information
            order.ShipAddr1 = billingForm.Address.Address1
            order.ShipAddr2 = billingForm.Address.Address2
            order.ShipCity = billingForm.Address.City
            order.ShipState = billingForm.Address.State
            order.ShipZip = billingForm.Address.Zip
            order.ShipCountry = billingForm.Address.Country
            order.ShipToFirstName = billingForm.Address.FirstName
            order.ShipToLastName = billingForm.Address.LastName

            'Billing Information
            order.BillAddr1 = shippingForm.Address.Address1
            order.BillAddr2 = shippingForm.Address.Address2
            order.BillCity = shippingForm.Address.City
            order.BillState = shippingForm.Address.State
            order.BillZip = shippingForm.Address.Zip
            order.BillCountry = shippingForm.Address.Country
            order.BillToFirstName = shippingForm.Address.FirstName
            order.BillToLastName = shippingForm.Address.LastName

            order = order.Save()

            Dim _itemsOnBackOrder As Integer = 0
            'Decrement and check the Inventory.
            For Each cart As Cart In profile.ShoppingCart
                Dim inventory As Inventory = inventory.GetByItemId(cart.ItemId)

                If cart.Quantity > inventory.Qty Then
                    _itemsOnBackOrder += cart.Quantity - inventory.Qty
                End If

                inventory.Qty -= cart.Quantity

                'Reset the Inventory back to 10,000
                If inventory.Qty < 0 Then
                    inventory.Qty = 10000
                End If

                inventory.Save()
            Next

            If _itemsOnBackOrder > 0 Then
                ItemsOnBackOrder.Text = String.Format("<br /><p style=""color:red;""><b>Backorder ALERT:</b> {0} items are on backorder.</p>", _itemsOnBackOrder)
            End If

            profile.ShoppingCart.SaveOrderLineItems(order.OrderId)

            ' destroy cart
            profile.ShoppingCart.Clear()
            profile.Save()
        Else
            lblMsg.Text = "<p><br>Can not process the order. Your cart is empty.</p><p class=SignUpLabel><a class=linkNewUser href=Default.aspx>Continue shopping</a></p>"
            wzdCheckOut.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Create CreditCardInfo object from user input
    ''' </summary>
    Private Function GetCreditCard() As CreditCard
        Dim type As String = WebUtility.InputText(listCCType.SelectedValue, 40)
        Dim exp As String = WebUtility.InputText(txtExpDate.Text, 7)
        Dim number As String = WebUtility.InputText(txtCCNumber.Text, 20)
        Return New CreditCard(type, number, exp)
    End Function

    ''' <summary>
    ''' Changing Wiszard steps
    ''' </summary>
    Protected Sub wzdCheckOut_ActiveStePChanged(ByVal sender As Object, ByVal e As EventArgs)
        If wzdCheckOut.ActiveStepIndex = 3 Then
            billingConfirm.Address = billingForm.Address
            shippingConfirm.Address = shippingForm.Address

            Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name)
            ltlTotal.Text = profile.ShoppingCart.Total.ToString("c")

            If txtCCNumber.Text.Length > 4 Then
                ltlCreditCard.Text = txtCCNumber.Text.Substring(txtCCNumber.Text.Length - 4, 4)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Handler for "Ship to Billing Addredd" checkbox.
    ''' Prefill/Clear shipping address form.
    ''' </summary>
    Protected Sub chkShipToBilling_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        If chkShipToBilling.Checked Then
            shippingForm.Address = billingForm.Address
        End If
    End Sub

    ''' <summary>
    ''' Custom validator to check CC expiration date
    ''' </summary>
    Protected Sub ServerValidate(ByVal source As Object, ByVal value As ServerValidateEventArgs)
        Dim dt As DateTime
        If DateTime.TryParse(value.Value, dt) Then
            value.IsValid = (dt > DateTime.Now)
        Else
            value.IsValid = False
        End If
    End Sub

End Class