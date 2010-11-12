Imports System
Imports System.Collections
Imports System.Collections.Generic

Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.BusinessObjects
	Public Partial Class Order
		Inherits BusinessBase(Of System.Int32)

		#region "Declarations"

		Private _userId As String = String.Empty
		Private _orderDate As Date = new DateTime()
		Private _shipAddr1 As String = String.Empty
		Private _shipAddr2 As String = Nothing
		Private _shipCity As String = String.Empty
		Private _shipState As String = String.Empty
		Private _shipZip As String = String.Empty
		Private _shipCountry As String = String.Empty
		Private _billAddr1 As String = String.Empty
		Private _billAddr2 As String = Nothing
		Private _billCity As String = String.Empty
		Private _billState As String = String.Empty
		Private _billZip As String = String.Empty
		Private _billCountry As String = String.Empty
		Private _courier As String = String.Empty
		Private _totalPrice As Decimal = Nothing
		Private _billToFirstName As String = String.Empty
		Private _billToLastName As String = String.Empty
		Private _shipToFirstName As String = String.Empty
		Private _shipToLastName As String = String.Empty
		Private _authorizationNumber As Integer = Nothing
		Private _locale As String = String.Empty
		
		
				
        #End Region

        #region "Constructors"

        Public Sub New()
		End Sub

        #End Region

        #region "Methods"

		Public Overloads Overrides Function GetHashCode() As Integer
			Dim sb As New System.Text.StringBuilder()

			sb.Append(Me.[GetType]().FullName)
			
			sb.Append(_userId)
			sb.Append(_orderDate)
			sb.Append(_shipAddr1)
			sb.Append(_shipAddr2)
			sb.Append(_shipCity)
			sb.Append(_shipState)
			sb.Append(_shipZip)
			sb.Append(_shipCountry)
			sb.Append(_billAddr1)
			sb.Append(_billAddr2)
			sb.Append(_billCity)
			sb.Append(_billState)
			sb.Append(_billZip)
			sb.Append(_billCountry)
			sb.Append(_courier)
			sb.Append(_totalPrice)
			sb.Append(_billToFirstName)
			sb.Append(_billToLastName)
			sb.Append(_shipToFirstName)
			sb.Append(_shipToLastName)
			sb.Append(_authorizationNumber)
			sb.Append(_locale)
			
			Return sb.ToString().GetHashCode()
		End Function

        #End Region

        #region "Properties"

		Public Overridable Property UserId() As String
			Get
				Return _userId
			End Get
			Set
				OnUserIdChanging()
				_userId = value
				OnUserIdChanged()
			End Set
		End Property
		Partial Private Sub OnUserIdChanging()
		End Sub
		Partial Private Sub OnUserIdChanged()
		End Sub
		Public Overridable Property OrderDate() As Date
			Get
				Return _orderDate
			End Get
			Set
				OnOrderDateChanging()
				_orderDate = value
				OnOrderDateChanged()
			End Set
		End Property
		Partial Private Sub OnOrderDateChanging()
		End Sub
		Partial Private Sub OnOrderDateChanged()
		End Sub
		Public Overridable Property ShipAddr1() As String
			Get
				Return _shipAddr1
			End Get
			Set
				OnShipAddr1Changing()
				_shipAddr1 = value
				OnShipAddr1Changed()
			End Set
		End Property
		Partial Private Sub OnShipAddr1Changing()
		End Sub
		Partial Private Sub OnShipAddr1Changed()
		End Sub
		Public Overridable Property ShipAddr2() As String
			Get
				Return _shipAddr2
			End Get
			Set
				OnShipAddr2Changing()
				_shipAddr2 = value
				OnShipAddr2Changed()
			End Set
		End Property
		Partial Private Sub OnShipAddr2Changing()
		End Sub
		Partial Private Sub OnShipAddr2Changed()
		End Sub
		Public Overridable Property ShipCity() As String
			Get
				Return _shipCity
			End Get
			Set
				OnShipCityChanging()
				_shipCity = value
				OnShipCityChanged()
			End Set
		End Property
		Partial Private Sub OnShipCityChanging()
		End Sub
		Partial Private Sub OnShipCityChanged()
		End Sub
		Public Overridable Property ShipState() As String
			Get
				Return _shipState
			End Get
			Set
				OnShipStateChanging()
				_shipState = value
				OnShipStateChanged()
			End Set
		End Property
		Partial Private Sub OnShipStateChanging()
		End Sub
		Partial Private Sub OnShipStateChanged()
		End Sub
		Public Overridable Property ShipZip() As String
			Get
				Return _shipZip
			End Get
			Set
				OnShipZipChanging()
				_shipZip = value
				OnShipZipChanged()
			End Set
		End Property
		Partial Private Sub OnShipZipChanging()
		End Sub
		Partial Private Sub OnShipZipChanged()
		End Sub
		Public Overridable Property ShipCountry() As String
			Get
				Return _shipCountry
			End Get
			Set
				OnShipCountryChanging()
				_shipCountry = value
				OnShipCountryChanged()
			End Set
		End Property
		Partial Private Sub OnShipCountryChanging()
		End Sub
		Partial Private Sub OnShipCountryChanged()
		End Sub
		Public Overridable Property BillAddr1() As String
			Get
				Return _billAddr1
			End Get
			Set
				OnBillAddr1Changing()
				_billAddr1 = value
				OnBillAddr1Changed()
			End Set
		End Property
		Partial Private Sub OnBillAddr1Changing()
		End Sub
		Partial Private Sub OnBillAddr1Changed()
		End Sub
		Public Overridable Property BillAddr2() As String
			Get
				Return _billAddr2
			End Get
			Set
				OnBillAddr2Changing()
				_billAddr2 = value
				OnBillAddr2Changed()
			End Set
		End Property
		Partial Private Sub OnBillAddr2Changing()
		End Sub
		Partial Private Sub OnBillAddr2Changed()
		End Sub
		Public Overridable Property BillCity() As String
			Get
				Return _billCity
			End Get
			Set
				OnBillCityChanging()
				_billCity = value
				OnBillCityChanged()
			End Set
		End Property
		Partial Private Sub OnBillCityChanging()
		End Sub
		Partial Private Sub OnBillCityChanged()
		End Sub
		Public Overridable Property BillState() As String
			Get
				Return _billState
			End Get
			Set
				OnBillStateChanging()
				_billState = value
				OnBillStateChanged()
			End Set
		End Property
		Partial Private Sub OnBillStateChanging()
		End Sub
		Partial Private Sub OnBillStateChanged()
		End Sub
		Public Overridable Property BillZip() As String
			Get
				Return _billZip
			End Get
			Set
				OnBillZipChanging()
				_billZip = value
				OnBillZipChanged()
			End Set
		End Property
		Partial Private Sub OnBillZipChanging()
		End Sub
		Partial Private Sub OnBillZipChanged()
		End Sub
		Public Overridable Property BillCountry() As String
			Get
				Return _billCountry
			End Get
			Set
				OnBillCountryChanging()
				_billCountry = value
				OnBillCountryChanged()
			End Set
		End Property
		Partial Private Sub OnBillCountryChanging()
		End Sub
		Partial Private Sub OnBillCountryChanged()
		End Sub
		Public Overridable Property Courier() As String
			Get
				Return _courier
			End Get
			Set
				OnCourierChanging()
				_courier = value
				OnCourierChanged()
			End Set
		End Property
		Partial Private Sub OnCourierChanging()
		End Sub
		Partial Private Sub OnCourierChanged()
		End Sub
		Public Overridable Property TotalPrice() As Decimal
			Get
				Return _totalPrice
			End Get
			Set
				OnTotalPriceChanging()
				_totalPrice = value
				OnTotalPriceChanged()
			End Set
		End Property
		Partial Private Sub OnTotalPriceChanging()
		End Sub
		Partial Private Sub OnTotalPriceChanged()
		End Sub
		Public Overridable Property BillToFirstName() As String
			Get
				Return _billToFirstName
			End Get
			Set
				OnBillToFirstNameChanging()
				_billToFirstName = value
				OnBillToFirstNameChanged()
			End Set
		End Property
		Partial Private Sub OnBillToFirstNameChanging()
		End Sub
		Partial Private Sub OnBillToFirstNameChanged()
		End Sub
		Public Overridable Property BillToLastName() As String
			Get
				Return _billToLastName
			End Get
			Set
				OnBillToLastNameChanging()
				_billToLastName = value
				OnBillToLastNameChanged()
			End Set
		End Property
		Partial Private Sub OnBillToLastNameChanging()
		End Sub
		Partial Private Sub OnBillToLastNameChanged()
		End Sub
		Public Overridable Property ShipToFirstName() As String
			Get
				Return _shipToFirstName
			End Get
			Set
				OnShipToFirstNameChanging()
				_shipToFirstName = value
				OnShipToFirstNameChanged()
			End Set
		End Property
		Partial Private Sub OnShipToFirstNameChanging()
		End Sub
		Partial Private Sub OnShipToFirstNameChanged()
		End Sub
		Public Overridable Property ShipToLastName() As String
			Get
				Return _shipToLastName
			End Get
			Set
				OnShipToLastNameChanging()
				_shipToLastName = value
				OnShipToLastNameChanged()
			End Set
		End Property
		Partial Private Sub OnShipToLastNameChanging()
		End Sub
		Partial Private Sub OnShipToLastNameChanged()
		End Sub
		Public Overridable Property AuthorizationNumber() As Integer
			Get
				Return _authorizationNumber
			End Get
			Set
				OnAuthorizationNumberChanging()
				_authorizationNumber = value
				OnAuthorizationNumberChanged()
			End Set
		End Property
		Partial Private Sub OnAuthorizationNumberChanging()
		End Sub
		Partial Private Sub OnAuthorizationNumberChanged()
		End Sub
		Public Overridable Property Locale() As String
			Get
				Return _locale
			End Get
			Set
				OnLocaleChanging()
				_locale = value
				OnLocaleChanged()
			End Set
		End Property
		Partial Private Sub OnLocaleChanging()
		End Sub
		Partial Private Sub OnLocaleChanged()
		End Sub
		
        #End Region

	End Class
End Namespace
