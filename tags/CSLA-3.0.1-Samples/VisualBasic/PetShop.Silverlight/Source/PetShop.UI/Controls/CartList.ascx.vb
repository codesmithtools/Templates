Public Partial Class CartList
    Inherits System.Web.UI.UserControl

    ''' <summary>
    ''' Bind control
    ''' </summary>
    Public Sub Bind(ByVal cart As Business.CartList)
        If Not cart Is Nothing Then
            repOrdered.DataSource = cart
            repOrdered.DataBind()
        End If

    End Sub

End Class