Imports PetShop.Business

Partial Public Class AddressConfirm
    Inherits UserControl
    ''' <summary>
    '''	Control property to set the address
    ''' </summary>
    Public WriteOnly Property Address() As Address
        Set(ByVal value As Address)
            If Not value Is Nothing Then
                If Not String.IsNullOrEmpty(value.FirstName) Then
                    ltlFirstName.Text = value.FirstName
                End If
                If Not String.IsNullOrEmpty(value.LastName) Then
                    ltlLastName.Text = value.LastName
                End If
                If Not String.IsNullOrEmpty(value.Address1) Then
                    ltlAddress1.Text = value.Address1
                End If
                If Not String.IsNullOrEmpty(value.Address2) Then
                    ltlAddress2.Text = value.Address2
                End If
                If Not String.IsNullOrEmpty(value.City) Then
                    ltlCity.Text = value.City
                End If
                If Not String.IsNullOrEmpty(value.Zip) Then
                    ltlZip.Text = value.Zip
                End If
                If Not String.IsNullOrEmpty(value.State) Then
                    ltlState.Text = value.State
                End If
                If Not String.IsNullOrEmpty(value.Country) Then
                    ltlCountry.Text = value.Country
                End If
                If Not String.IsNullOrEmpty(value.Phone) Then
                    ltlPhone.Text = value.Phone
                End If
                If Not String.IsNullOrEmpty(value.Email) Then
                    ltlEmail.Text = value.Email
                End If
            End If
        End Set
    End Property
End Class