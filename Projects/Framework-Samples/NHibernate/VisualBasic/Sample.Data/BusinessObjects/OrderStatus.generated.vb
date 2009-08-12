Imports System
Imports System.Collections
Imports System.Collections.Generic

Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.BusinessObjects
	Public Partial Class OrderStatus
		Inherits BusinessBase(Of System.String)

		#region "Declarations"

		Private _orderId As Integer = Nothing
		Private _lineNum As Integer = Nothing
		Private _timestamp As Date = new DateTime()
		Private _status As String = String.Empty
		
		
				
        #End Region

        #region "Constructors"

        Public Sub New()
		End Sub

        #End Region

        #region "Methods"

		Public Overloads Overrides Function GetHashCode() As Integer
			Dim sb As New System.Text.StringBuilder()

			sb.Append(Me.[GetType]().FullName)
			
			sb.Append(_orderId)
			sb.Append(_lineNum)
			sb.Append(_timestamp)
			sb.Append(_status)
			
			Return sb.ToString().GetHashCode()
		End Function

        #End Region

        #region "Properties"

		Public Overloads Overrides Property Id() As String
			Get
				Dim uniqueId As New System.Text.StringBuilder()
				uniqueId.Append(_orderId)
				uniqueId.Append("^")
				uniqueId.Append(_lineNum)
				Return uniqueId.ToString()
			End Get
			Set(ByVal value As String)
            End Set
		End Property
		
		Public Overridable Property OrderId() As Integer
			Get
				Return _orderId
			End Get
			Set
				OnOrderIdChanging()
				_orderId = value
				OnOrderIdChanged()
			End Set
		End Property
		Partial Private Sub OnOrderIdChanging()
		End Sub
		Partial Private Sub OnOrderIdChanged()
		End Sub
		Public Overridable Property LineNum() As Integer
			Get
				Return _lineNum
			End Get
			Set
				OnLineNumChanging()
				_lineNum = value
				OnLineNumChanged()
			End Set
		End Property
		Partial Private Sub OnLineNumChanging()
		End Sub
		Partial Private Sub OnLineNumChanged()
		End Sub
		Public Overridable Property Timestamp() As Date
			Get
				Return _timestamp
			End Get
			Set
				OnTimestampChanging()
				_timestamp = value
				OnTimestampChanged()
			End Set
		End Property
		Partial Private Sub OnTimestampChanging()
		End Sub
		Partial Private Sub OnTimestampChanged()
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
		
        #End Region

	End Class
End Namespace
