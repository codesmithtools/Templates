Public Partial Class BreadCrumbControl
    Inherits System.Web.UI.UserControl

    Private Const PRODUCTS_URL As String = "~/Products.aspx?page=0&categoryId={0}"
    Private Const ITEMS_URL As String = "~/Items.aspx?categoryId={0}&productId={1}"
    Private Const DIVIDER As String = "&nbsp;&#062;&nbsp;"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Dim categoryId As String = Request.QueryString("categoryId")

        If Not String.IsNullOrEmpty(categoryId) Then

            ' process Home Page link
            Dim lnkHome As New HtmlAnchor()
            plhControl.Controls.Add(lnkHome)
            plhControl.Controls.Add(GetDivider())

            ' Process Product page link
            Dim lnkProducts As New HtmlAnchor()
            plhControl.Controls.Add(lnkProducts)

            Dim productId As String = Request.QueryString("productId")
            If Not String.IsNullOrEmpty(productId) Then

                ' Process Item page link
                plhControl.Controls.Add(GetDivider())
                Dim lnkItemDetails As New HtmlAnchor()

                plhControl.Controls.Add(lnkItemDetails)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Create a breadcrumb nodes divider
    ''' </summary>
    ''' <returns>Literal control containing formatted divider</returns>
    Private Shared Function GetDivider() As Literal
        Return New Literal()
    End Function

End Class