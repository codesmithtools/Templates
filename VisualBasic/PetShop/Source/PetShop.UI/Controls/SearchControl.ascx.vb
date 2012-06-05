Imports PetShop.Business

Partial Public Class SearchControl
    Inherits System.Web.UI.UserControl

    ''' <summary>
    ''' Rebind control
    ''' </summary>
    Protected Sub PageChanged(ByVal sender As Object, ByVal e As DataGridPageChangedEventArgs)
        'reset index
        searchList.CurrentPageIndex = e.NewPageIndex

        'get category id
        Dim keywordKey As String = Request.QueryString("keywords")

        'ProductList list = ProductList.NewList();
        Dim list As New List(Of Product)
        For Each product As Product In ProductList.GetAll()
            Dim isResult As Boolean = product.Name.ToLowerInvariant().Contains(keywordKey.ToLowerInvariant()) OrElse product.Description.ToLowerInvariant().Contains(keywordKey.ToLowerInvariant())

            If isResult Then
                list.Add(product)
            End If
        Next

        'bind data
        searchList.DataSource = list
        searchList.DataBind()
    End Sub

    ''' <summary>
    ''' Add cache dependency
    ''' </summary>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
    End Sub


End Class