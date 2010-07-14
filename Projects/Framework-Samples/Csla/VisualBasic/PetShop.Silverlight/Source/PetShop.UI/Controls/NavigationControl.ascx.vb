Imports PetShop.Business

Partial Public Class NavigationControl
    Inherits System.Web.UI.UserControl

    Private _controlStyle As String

    ' control layout property
    Protected ReadOnly Property ControlStyle() As String
        Get
            Return _controlStyle
        End Get
    End Property

    ' Get properties based on control consumer
    Protected Sub GetControlStyle()
        _controlStyle = If(Request.ServerVariables("SCRIPT_NAME").ToLower().IndexOf("default.aspx") > 0, "navigationLinks", "mainNavigation")
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        GetControlStyle()
        BindCategories()

        ' Select current category
        Dim categoryId As String = Request.QueryString("categoryId")
        If Not String.IsNullOrEmpty(categoryId) Then
            SelectCategory(categoryId)
        End If
    End Sub

    ' Select current category.
    Private Sub SelectCategory(ByVal categoryId As String)
        For Each item As RepeaterItem In rePCategories.Items
            Dim hidCategoryId As HiddenField = DirectCast(item.FindControl("hidCategoryId"), HiddenField)
            If hidCategoryId.Value.ToLower() = categoryId.ToLower() Then
                Dim lnkCategory As HyperLink = DirectCast(item.FindControl("lnkCategory"), HyperLink)
                lnkCategory.ForeColor = System.Drawing.Color.FromArgb(199, 116, 3)
                Exit For
            End If
        Next
    End Sub

    ' Bind categories
    Private Sub BindCategories()
        rePCategories.DataSource = CategoryList.GetAll()
        rePCategories.DataBind()
    End Sub

End Class