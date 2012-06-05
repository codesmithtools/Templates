Imports PetShop.Controls
Imports PetShop.Business

Partial Public Class CreditCardForm
    Inherits System.Web.UI.UserControl

    ''' <summary>
    ''' Property to set/get credit card info
    ''' </summary>
    Public Property CreditCard() As CreditCard
        Get
            ' Make sure we clean the input
            Dim type As String = WebUtility.InputText(listCctype.SelectedValue, 40)
            Dim exp As String = WebUtility.InputText(txtExpdate.Text, 7)
            Dim number As String = WebUtility.InputText(txtCcnumber.Text, 20)
            Return New CreditCard(type, number, exp)
        End Get
        Set(ByVal value As CreditCard)
            If Not value Is Nothing Then
                If Not String.IsNullOrEmpty(value.CardNumber) Then
                    txtCcnumber.Text = value.CardNumber
                End If
                If Not String.IsNullOrEmpty(value.CardExpiration) Then
                    txtExpdate.Text = value.CardExpiration
                End If
                If Not String.IsNullOrEmpty(value.CardType) Then
                    listCctype.Items.FindByValue(value.CardType).Selected = True
                End If
            End If
        End Set
    End Property

    ''' <summary>
    ''' Custom validator to check the expiration date
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