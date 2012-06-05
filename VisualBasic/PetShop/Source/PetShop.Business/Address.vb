Imports PetShop.Business

Public Class Address

#Region "Private Members"

    Dim _firstName As String
    Dim _lastName As String
    Dim _address1 As String
    Dim _address2 As String
    Dim _city As String
    Dim _state As String
    Dim _zip As String
    Dim _country As String
    Dim _phone As String
    Dim _email As String

#End Region

#Region "Constructor(s)"

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    Private Sub New()
    End Sub

    ''' <summary>
    ''' Constructor with specified initial values.
    ''' </summary>
    ''' <param name="firstName">First Name</param>
    ''' <param name="lastName">Last Name</param>
    ''' <param name="address1">Address line 1</param>
    ''' <param name="address2">Address line 2</param>
    ''' <param name="city">City</param>
    ''' <param name="state">State</param>
    ''' <param name="zip">Postal Code</param>
    ''' <param name="country">Country</param>
    ''' <param name="phone">Phone number at this address.</param>
    ''' <param name="email">Email at this address.</param>
    Public Sub New(ByVal firstName As String, ByVal lastName As String, ByVal address1 As String, ByVal address2 As String, ByVal city As String, ByVal state As String, _
     ByVal zip As String, ByVal country As String, ByVal phone As String, ByVal email As String)
        Me.FirstName = firstName
        Me.LastName = lastName
        Me.Address1 = address1
        Me.Address2 = address2
        Me.City = city
        Me.State = state
        Me.Zip = zip
        Me.Country = country
        Me.Phone = phone
        Me.Email = email
    End Sub

    ''' <summary>
    ''' Constructor with specified initial values.
    ''' </summary>
    ''' <param name="profile">User's Profile</param>
    Public Sub New(ByVal profile As Profile)
        If Not String.IsNullOrEmpty(profile.Username) AndAlso profile.Accounts.Count > 0 Then
            'Just grab the first account.
            Dim account As Account = profile.Accounts(0)
            Me.FirstName = account.FirstName
            Me.LastName = account.LastName
            Me.Address1 = account.Address1
            Me.Address2 = account.Address2
            Me.City = account.City
            Me.State = account.State
            Me.Zip = account.Zip
            Me.Country = account.Country
            Me.Phone = account.Phone
            Me.Email = account.Email
        End If
    End Sub
#End Region

#Region "Properties"

    ''' <summary>
    ''' First Name
    ''' </summary>
    Public Property FirstName() As String
        Get
            Return _firstName
        End Get
        Set(ByVal value As String)
            _firstName = value
        End Set
    End Property

    ''' <summary>
    ''' Last Name
    ''' </summary>
    Public Property LastName() As String
        Get
            Return _lastName
        End Get
        Set(ByVal value As String)
            _lastName = value
        End Set
    End Property

    ''' <summary>
    ''' Address line 1
    ''' </summary>
    Public Property Address1() As String
        Get
            Return _address1
        End Get
        Set(ByVal value As String)
            _address1 = value
        End Set
    End Property

    ''' <summary>
    ''' Address line 2
    ''' </summary>
    Public Property Address2() As String
        Get
            Return _address2
        End Get
        Set(ByVal value As String)
            _address2 = value
        End Set
    End Property

    ''' <summary>
    ''' City
    ''' </summary>
    Public Property City() As String
        Get
            Return _city
        End Get
        Set(ByVal value As String)
            _city = value
        End Set
    End Property

    ''' <summary>
    ''' State
    ''' </summary>
    Public Property State() As String
        Get
            Return _state
        End Get
        Set(ByVal value As String)
            _state = value
        End Set
    End Property

    ''' <summary>
    ''' Postal Code
    ''' </summary>
    Public Property Zip() As String
        Get
            Return _zip
        End Get
        Set(ByVal value As String)
            _zip = value
        End Set
    End Property

    ''' <summary>
    ''' Country
    ''' </summary>
    Public Property Country() As String
        Get
            Return _country
        End Get
        Set(ByVal value As String)
            _country = value
        End Set
    End Property

    ''' <summary>
    ''' Phone number at this address.
    ''' </summary>
    Public Property Phone() As String
        Get
            Return _phone
        End Get
        Set(ByVal value As String)
            _phone = value
        End Set
    End Property

    ''' <summary>
    ''' Email at this address.
    ''' </summary>
    Public Property Email() As String
        Get
            Return _email
        End Get
        Set(ByVal value As String)
            _email = value
        End Set
    End Property

#End Region

End Class
