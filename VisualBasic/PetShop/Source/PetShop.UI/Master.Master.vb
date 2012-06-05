Imports PetShop.Controls

Partial Public Class Master
    Inherits System.Web.UI.MasterPage

    Private Const HEADER_PREFIX As String = ".NET Pet Shop :: {0}"

    ''' <summary>
    ''' Create page header on Page PreRender event
    ''' </summary>
    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs)
        ltlHeader.Text = Page.Header.Title
        Page.Header.Title = String.Format(HEADER_PREFIX, Page.Header.Title)
    End Sub

    ''' <summary>
    ''' Redirect to Search page
    ''' </summary>
    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
        WebUtility.SearchRedirect(txtSearch.Text)
    End Sub

End Class