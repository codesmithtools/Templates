Imports PetShop.Business

Partial Public Class NewUser
    Inherits System.Web.UI.Page

    Protected Sub CreateUserWizard1_CreatedUser(ByVal sender As Object, ByVal e As EventArgs)
        Dim username As String = (DirectCast(sender, System.Web.UI.WebControls.CreateUserWizard)).UserName

        ProfileManager.Instance.CreateUser(username, False)
    End Sub

End Class