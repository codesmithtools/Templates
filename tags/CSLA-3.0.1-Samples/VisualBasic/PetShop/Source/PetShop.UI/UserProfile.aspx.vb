Imports PetShop.Business

Partial Public Class UserProfile
    Inherits System.Web.UI.Page

    ''' <summary>
    ''' Update profile
    ''' </summary>
    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name)
        If AddressForm.IsValid Then
            If profile.Accounts.Count > 0 Then
                Dim account As Account = profile.Accounts(0)
                UpdateAccount(account, AddressForm.Address)
            Else
                Dim account As Account = profile.Accounts.AddNew()
                account.UniqueID = profile.UniqueID

                UpdateAccount(account, AddressForm.Address)
            End If

            profile = profile.Save(True)
        End If

        lblMessage.Text = "Your profile information has been successfully updated.<br>&nbsp;"
    End Sub

    ''' <summary>
    ''' Updates an account from an address.
    ''' </summary>
    ''' <param name="account">The account.</param>
    ''' <param name="address">The address.</param>
    Private Shared Sub UpdateAccount(ByRef account As Account, ByVal address As Address)
        account.FirstName = address.FirstName
        account.LastName = address.LastName
        account.Address1 = address.Address1
        account.Address2 = address.Address2
        account.City = address.City
        account.State = address.State
        account.Zip = address.Zip
        account.Country = address.Country
        account.Phone = address.Phone
        account.Email = address.Email
    End Sub

    ''' <summary>
    ''' Handle Page load event
    ''' </summary>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If String.IsNullOrEmpty(User.Identity.Name) Then
            Response.Redirect("~/SignIn.aspx")
        End If

        If Not IsPostBack Then
            BindUser()
        End If
    End Sub

    ''' <summary>
    ''' Bind controls to profile
    ''' </summary>
    Private Sub BindUser()
        Dim profile As Profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name)
        AddressForm.Address = New Address(profile)
    End Sub

End Class