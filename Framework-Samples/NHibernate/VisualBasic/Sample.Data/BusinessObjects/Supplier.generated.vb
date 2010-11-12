Imports System
Imports System.Collections
Imports System.Collections.Generic

Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.BusinessObjects
	Public Partial Class Supplier
		Inherits BusinessBase(Of System.Int32)

		#region "Declarations"

		Private _name As String = Nothing
		Private _status As String = String.Empty
		Private _addr1 As String = Nothing
		Private _addr2 As String = Nothing
		Private _city As String = Nothing
		Private _state As String = Nothing
		Private _zip As String = Nothing
		Private _phone As String = Nothing
		
		
		Private _items As IList(Of Item) = New List(Of Item)()
				
        #End Region

        #region "Constructors"

        Public Sub New()
		End Sub

        #End Region

        #region "Methods"

		Public Overloads Overrides Function GetHashCode() As Integer
			Dim sb As New System.Text.StringBuilder()

			sb.Append(Me.[GetType]().FullName)
			
			sb.Append(_name)
			sb.Append(_status)
			sb.Append(_addr1)
			sb.Append(_addr2)
			sb.Append(_city)
			sb.Append(_state)
			sb.Append(_zip)
			sb.Append(_phone)
			
			Return sb.ToString().GetHashCode()
		End Function

        #End Region

        #region "Properties"

		Public Overridable Property Name() As String
			Get
				Return _name
			End Get
			Set
				OnNameChanging()
				_name = value
				OnNameChanged()
			End Set
		End Property
		Partial Private Sub OnNameChanging()
		End Sub
		Partial Private Sub OnNameChanged()
		End Sub
		Public Overridable Property Status() As String
			Get
				Return _status
			End Get
			Set
				OnStatusChanging()
				_status = value
				OnStatusChanged()
			End Set
		End Property
		Partial Private Sub OnStatusChanging()
		End Sub
		Partial Private Sub OnStatusChanged()
		End Sub
		Public Overridable Property Addr1() As String
			Get
				Return _addr1
			End Get
			Set
				OnAddr1Changing()
				_addr1 = value
				OnAddr1Changed()
			End Set
		End Property
		Partial Private Sub OnAddr1Changing()
		End Sub
		Partial Private Sub OnAddr1Changed()
		End Sub
		Public Overridable Property Addr2() As String
			Get
				Return _addr2
			End Get
			Set
				OnAddr2Changing()
				_addr2 = value
				OnAddr2Changed()
			End Set
		End Property
		Partial Private Sub OnAddr2Changing()
		End Sub
		Partial Private Sub OnAddr2Changed()
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
		
		Public Overridable Property Items() As IList(Of Item)
			Get
				Return _items
			End Get
			Set
				OnItemsChanging()
				_items = value
				OnItemsChanged()
			End Set
		End Property
		Partial Private Sub OnItemsChanging()
		End Sub
		Partial Private Sub OnItemsChanged()
		End Sub
		
        #End Region

	End Class
End Namespace
