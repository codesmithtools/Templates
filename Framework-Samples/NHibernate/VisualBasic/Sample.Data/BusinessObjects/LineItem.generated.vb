Imports System
Imports System.Collections
Imports System.Collections.Generic

Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.BusinessObjects
	Public Partial Class LineItem
		Inherits BusinessBase(Of System.String)

		#region "Declarations"

		Private _orderId As Integer = Nothing
		Private _lineNum As Integer = Nothing
		Private _itemId As String = String.Empty
		Private _quantity As Integer = Nothing
		Private _unitPrice As Decimal = Nothing
		
		
				
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
			sb.Append(_itemId)
			sb.Append(_quantity)
			sb.Append(_unitPrice)
			
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
		Public Overridable Property ItemId() As String
			Get
				Return _itemId
			End Get
			Set
				OnItemIdChanging()
				_itemId = value
				OnItemIdChanged()
			End Set
		End Property
		Partial Private Sub OnItemIdChanging()
		End Sub
		Partial Private Sub OnItemIdChanged()
		End Sub
		Public Overridable Property Quantity() As Integer
			Get
				Return _quantity
			End Get
			Set
				OnQuantityChanging()
				_quantity = value
				OnQuantityChanged()
			End Set
		End Property
		Partial Private Sub OnQuantityChanging()
		End Sub
		Partial Private Sub OnQuantityChanged()
		End Sub
		Public Overridable Property UnitPrice() As Decimal
			Get
				Return _unitPrice
			End Get
			Set
				OnUnitPriceChanging()
				_unitPrice = value
				OnUnitPriceChanged()
			End Set
		End Property
		Partial Private Sub OnUnitPriceChanging()
		End Sub
		Partial Private Sub OnUnitPriceChanged()
		End Sub
		
        #End Region

	End Class
End Namespace
