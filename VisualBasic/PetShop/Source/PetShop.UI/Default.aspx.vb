Imports PetShop.Controls

Partial Public Class _Default
    Inherits System.Web.UI.Page

    ''' <summary>
    ''' Redirect to Search page
    ''' </summary>
    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
        WebUtility.SearchRedirect(txtSearch.Text)
    End Sub

End Class