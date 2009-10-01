''' <summary>
''' Business entity used to model credit card information.
''' </summary>
<Serializable()> _
Public Class CreditCard

#Region "Private Members"

    Dim _cardType As String
    Dim _cardNumber As String
    Dim _cardExpiration As String

#End Region

#Region "Constructor(s)"

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Constructor with specified initial values.
    ''' </summary>
    ''' <param name="cardType">Card type, e.g. Visa, Master Card, American Express.</param>
    ''' <param name="cardNumber">Number on the card.</param>
    ''' <param name="cardExpiration">Expiry Date, form  MM/YY.</param>
    Public Sub New(ByVal cardType As String, ByVal cardNumber As String, ByVal cardExpiration As String)
        Me.CardType = cardType
        Me.CardNumber = cardNumber
        Me.CardExpiration = cardExpiration
    End Sub

#End Region

#Region "Public Properties"

    ''' <summary>
    ''' Card type, e.g. Visa, Master Card, American Express.
    ''' </summary>
    Public Property CardType() As String
        Get
            Return _cardType
        End Get
        Set(ByVal value As String)
            _cardType = value
        End Set
    End Property

    ''' <summary>
    ''' Number on the card.
    ''' </summary>
    Public Property CardNumber() As String
        Get
            Return _cardNumber
        End Get
        Set(ByVal value As String)
            _cardNumber = value
        End Set
    End Property

    ''' <summary>
    ''' Expiry Date, form  MM/YY.
    ''' </summary>
    Public Property CardExpiration() As String
        Get
            Return _cardExpiration
        End Get
        Set(ByVal value As String)
            _cardExpiration = value
        End Set
    End Property

#End Region

End Class