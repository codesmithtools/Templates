Imports PetShop.Controls
Imports PetShop.Business

Partial Public Class AddressForm
    Inherits System.Web.UI.UserControl

    Public ReadOnly Property IsValid() As Boolean
        Get
            If String.IsNullOrEmpty(txtFirstName.Text) AndAlso String.IsNullOrEmpty(txtLastName.Text) AndAlso String.IsNullOrEmpty(txtAddress1.Text) AndAlso String.IsNullOrEmpty(txtAddress2.Text) AndAlso String.IsNullOrEmpty(txtCity.Text) AndAlso String.IsNullOrEmpty(txtZip.Text) AndAlso String.IsNullOrEmpty(txtEmail.Text) AndAlso String.IsNullOrEmpty(txtPhone.Text) Then
                Return False
            End If

            Return True
        End Get
    End Property


    ''' <summary>
    ''' Control property to set or get the address
    ''' </summary>
    Public Property Address() As Address
        Get
            If IsValid = False Then
                Return Nothing
            End If

            ' Make sure we clean the input
            Dim firstName As String = WebUtility.InputText(txtFirstName.Text, 50)
            Dim lastName As String = WebUtility.InputText(txtLastName.Text, 50)
            Dim address1 As String = WebUtility.InputText(txtAddress1.Text, 50)
            Dim address2 As String = WebUtility.InputText(txtAddress2.Text, 50)
            Dim city As String = WebUtility.InputText(txtCity.Text, 50)
            Dim zip As String = WebUtility.InputText(txtZip.Text, 10)
            Dim phone As String = WebUtility.InputText(WebUtility.CleanNonWord(txtPhone.Text), 10)
            Dim email As String = WebUtility.InputText(txtEmail.Text, 80)
            Dim state As String = WebUtility.InputText(listState.SelectedItem.Value, 2)
            Dim country As String = WebUtility.InputText(listCountry.SelectedItem.Value, 50)

            Return New Address(firstName, lastName, address1, address2, city, state, _
                zip, country, phone, email)
        End Get
        Set(ByVal value As Address)
            If Not value Is Nothing Then
                If Not String.IsNullOrEmpty(value.FirstName) Then
                    txtFirstName.Text = value.FirstName
                End If
                If Not String.IsNullOrEmpty(value.LastName) Then
                    txtLastName.Text = value.LastName
                End If
                If Not String.IsNullOrEmpty(value.Address1) Then
                    txtAddress1.Text = value.Address1
                End If
                If Not String.IsNullOrEmpty(value.Address2) Then
                    txtAddress2.Text = value.Address2
                End If
                If Not String.IsNullOrEmpty(value.City) Then
                    txtCity.Text = value.City
                End If
                If Not String.IsNullOrEmpty(value.Zip) Then
                    txtZip.Text = value.Zip
                End If
                If Not String.IsNullOrEmpty(value.Phone) Then
                    txtPhone.Text = value.Phone
                End If
                If Not String.IsNullOrEmpty(value.Email) Then
                    txtEmail.Text = value.Email
                End If
                If Not String.IsNullOrEmpty(value.State) Then
                    listState.ClearSelection()
                    listState.SelectedValue = value.State
                End If
                If Not String.IsNullOrEmpty(value.Country) Then
                    listCountry.ClearSelection()
                    listCountry.SelectedValue = value.Country
                End If
            End If
        End Set
    End Property

End Class