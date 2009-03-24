Imports System
Imports System.Collections
Imports System.Collections.Generic

Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.BusinessObjects
	Public Partial Class Account
		Inherits BusinessBase(Of System.Int32)

		#region "Declarations"

		Private _email As String = String.Empty
		Private _firstName As String = String.Empty
		Private _lastName As String = String.Empty
		Private _address1 As String = String.Empty
		Private _address2 As String = Nothing
		Private _city As String = String.Empty
		Private _state As String = String.Empty
		Private _zip As String = String.Empty
		Private _country As String = String.Empty
		Private _phone As String = Nothing
		
		Private _profile As Profile = Nothing
		
				
        #End Region

        #region "Constructors"

        Public Sub New()
		End Sub

        #End Region

        #region "Methods"

		Public Overloads Overrides Function GetHashCode() As Integer
			Dim sb As New System.Text.StringBuilder()

			sb.Append(Me.[GetType]().FullName)
			
			sb.Append(_email)
			sb.Append(_firstName)
			sb.Append(_lastName)
			sb.Append(_address1)
			sb.Append(_address2)
			sb.Append(_city)
			sb.Append(_state)
			sb.Append(_zip)
			sb.Append(_country)
			sb.Append(_phone)
			
			Return sb.ToString().GetHashCode()
		End Function

        #End Region

        #region "Properties"

		Public Overridable Property Email() As String
			Get
				Return _email
			End Get
			Set
				OnEmailChanging()
				_email = value
				OnEmailChanged()
			End Set
		End Property
		Partial Private Sub OnEmailChanging()
		End Sub
		Partial Private Sub OnEmailChanged()
		End Sub
		Public Overridable Property FirstName() As String
			Get
				Return _firstName
			End Get
			Set
				OnFirstNameChanging()
				_firstName = value
				OnFirstNameChanged()
			End Set
		End Property
		Partial Private Sub OnFirstNameChanging()
		End Sub
		Partial Private Sub OnFirstNameChanged()
		End Sub
		Public Overridable Property LastName() As String
			Get
				Return _lastName
			End Get
			Set
				OnLastNameChanging()
				_lastName = value
				OnLastNameChanged()
			End Set
		End Property
		Partial Private Sub OnLastNameChanging()
		End Sub
		Partial Private Sub OnLastNameChanged()
		End Sub
		Public Overridable Property Address1() As String
			Get
				Return _address1
			End Get
			Set
				OnAddress1Changing()
				_address1 = value
				OnAddress1Changed()
			End Set
		End Property
		Partial Private Sub OnAddress1Changing()
		End Sub
		Partial Private Sub OnAddress1Changed()
		End Sub
		Public Overridable Property Address2() As String
			Get
				Return _address2
			End Get
			Set
				OnAddress2Changing()
				_address2 = value
				OnAddress2Changed()
			End Set
		End Property
		Partial Private Sub OnAddress2Changing()
		End Sub
		Partial Private Sub OnAddress2Changed()
		End Sub
		Public Overridable Property City() As String
			Get
				Return _city
			End Get
			Set
				OnCityChanging()
				_city = value
				OnCityChanged()
			End Set
		End Property
		Partial Private Sub OnCityChanging()
		End Sub
		Partial Private Sub OnCityChanged()
		End Sub
		Public Overridable Property State() As String
			Get
				Return _state
			End Get
			Set
				OnStateChanging()
				_state = value
				OnStateChanged()
			End Set
		End Property
		Partial Private Sub OnStateChanging()
		End Sub
		Partial Private Sub OnStateChanged()
		End Sub
		Public Overridable Property Zip() As String
			Get
				Return _zip
			End Get
			Set
				OnZipChanging()
				_zip = value
				OnZipChanged()
			End Set
		End Property
		Partial Private Sub OnZipChanging()
		End Sub
		Partial Private Sub OnZipChanged()
		End Sub
		Public Overridable Property Country() As String
			Get
				Return _country
			End Get
			Set
				OnCountryChanging()
				_country = value
				OnCountryChanged()
			End Set
		End Property
		Partial Private Sub OnCountryChanging()
		End Sub
		Partial Private Sub OnCountryChanged()
		End Sub
		Public Overridable Property Phone() As String
			Get
				Return _phone
			End Get
			Set
				OnPhoneChanging()
				_phone = value
				OnPhoneChanged()
			End Set
		End Property
		Partial Private Sub OnPhoneChanging()
		End Sub
		Partial Private Sub OnPhoneChanged()
		End Sub
		
		Public Overridable Property Profile() As Profile
			Get
				Return _profile
			End Get
			Set
				OnProfileChanging()
				_profile = value
				OnProfileChanged()
			End Set
		End Property
		Partial Private Sub OnProfileChanging()
		End Sub
		Partial Private Sub OnProfileChanged()
		End Sub
		
        #End Region

	End Class
End Namespace
