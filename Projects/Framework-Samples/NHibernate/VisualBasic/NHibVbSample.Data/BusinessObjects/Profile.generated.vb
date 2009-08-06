Imports System
Imports System.Collections
Imports System.Collections.Generic

Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.BusinessObjects
	Public Partial Class Profile
		Inherits BusinessBase(Of System.Int32)

		#region "Declarations"

		Private _username As String = String.Empty
		Private _applicationName As String = String.Empty
		Private _isAnonymous As Boolean? = Nothing
		Private _lastActivityDate As Date? = Nothing
		Private _lastUpdatedDate As Date? = Nothing
		
		
		Private _accounts As IList(Of Account) = New List(Of Account)()
		Private _carts As IList(Of Cart) = New List(Of Cart)()
				
        #End Region

        #region "Constructors"

        Public Sub New()
		End Sub

        #End Region

        #region "Methods"

		Public Overloads Overrides Function GetHashCode() As Integer
			Dim sb As New System.Text.StringBuilder()

			sb.Append(Me.[GetType]().FullName)
			
			sb.Append(_username)
			sb.Append(_applicationName)
			sb.Append(_isAnonymous)
			sb.Append(_lastActivityDate)
			sb.Append(_lastUpdatedDate)
			
			Return sb.ToString().GetHashCode()
		End Function

        #End Region

        #region "Properties"

		Public Overridable Property Username() As String
			Get
				Return _username
			End Get
			Set
				OnUsernameChanging()
				_username = value
				OnUsernameChanged()
			End Set
		End Property
		Partial Private Sub OnUsernameChanging()
		End Sub
		Partial Private Sub OnUsernameChanged()
		End Sub
		Public Overridable Property ApplicationName() As String
			Get
				Return _applicationName
			End Get
			Set
				OnApplicationNameChanging()
				_applicationName = value
				OnApplicationNameChanged()
			End Set
		End Property
		Partial Private Sub OnApplicationNameChanging()
		End Sub
		Partial Private Sub OnApplicationNameChanged()
		End Sub
		Public Overridable Property IsAnonymous() As Boolean?
			Get
				Return _isAnonymous
			End Get
			Set
				OnIsAnonymousChanging()
				_isAnonymous = value
				OnIsAnonymousChanged()
			End Set
		End Property
		Partial Private Sub OnIsAnonymousChanging()
		End Sub
		Partial Private Sub OnIsAnonymousChanged()
		End Sub
		Public Overridable Property LastActivityDate() As Date?
			Get
				Return _lastActivityDate
			End Get
			Set
				OnLastActivityDateChanging()
				_lastActivityDate = value
				OnLastActivityDateChanged()
			End Set
		End Property
		Partial Private Sub OnLastActivityDateChanging()
		End Sub
		Partial Private Sub OnLastActivityDateChanged()
		End Sub
		Public Overridable Property LastUpdatedDate() As Date?
			Get
				Return _lastUpdatedDate
			End Get
			Set
				OnLastUpdatedDateChanging()
				_lastUpdatedDate = value
				OnLastUpdatedDateChanged()
			End Set
		End Property
		Partial Private Sub OnLastUpdatedDateChanging()
		End Sub
		Partial Private Sub OnLastUpdatedDateChanged()
		End Sub
		
		Public Overridable Property Accounts() As IList(Of Account)
			Get
				Return _accounts
			End Get
			Set
				OnAccountsChanging()
				_accounts = value
				OnAccountsChanged()
			End Set
		End Property
		Partial Private Sub OnAccountsChanging()
		End Sub
		Partial Private Sub OnAccountsChanged()
		End Sub
		
		Public Overridable Property Carts() As IList(Of Cart)
			Get
				Return _carts
			End Get
			Set
				OnCartsChanging()
				_carts = value
				OnCartsChanged()
			End Set
		End Property
		Partial Private Sub OnCartsChanging()
		End Sub
		Partial Private Sub OnCartsChanged()
		End Sub
		
        #End Region

	End Class
End Namespace
