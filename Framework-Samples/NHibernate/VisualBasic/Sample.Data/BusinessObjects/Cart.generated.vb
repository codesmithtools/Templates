Imports System
Imports System.Collections
Imports System.Collections.Generic

Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.BusinessObjects
	Public Partial Class Cart
		Inherits BusinessBase(Of System.Int32)

		#region "Declarations"

		Private _itemId As String = String.Empty
		Private _name As String = String.Empty
		Private _type As String = String.Empty
		Private _price As Decimal = Nothing
		Private _categoryId As String = String.Empty
		Private _productId As String = String.Empty
		Private _isShoppingCart As Boolean = Nothing
		Private _quantity As Integer = Nothing
		
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
			
			sb.Append(_itemId)
			sb.Append(_name)
			sb.Append(_type)
			sb.Append(_price)
			sb.Append(_categoryId)
			sb.Append(_productId)
			sb.Append(_isShoppingCart)
			sb.Append(_quantity)
			
			Return sb.ToString().GetHashCode()
		End Function

        #End Region

        #region "Properties"

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
		Public Overridable Property Type() As String
			Get
				Return _type
			End Get
			Set
				OnTypeChanging()
				_type = value
				OnTypeChanged()
			End Set
		End Property
		Partial Private Sub OnTypeChanging()
		End Sub
		Partial Private Sub OnTypeChanged()
		End Sub
		Public Overridable Property Price() As Decimal
			Get
				Return _price
			End Get
			Set
				OnPriceChanging()
				_price = value
				OnPriceChanged()
			End Set
		End Property
		Partial Private Sub OnPriceChanging()
		End Sub
		Partial Private Sub OnPriceChanged()
		End Sub
		Public Overridable Property CategoryId() As String
			Get
				Return _categoryId
			End Get
			Set
				OnCategoryIdChanging()
				_categoryId = value
				OnCategoryIdChanged()
			End Set
		End Property
		Partial Private Sub OnCategoryIdChanging()
		End Sub
		Partial Private Sub OnCategoryIdChanged()
		End Sub
		Public Overridable Property ProductId() As String
			Get
				Return _productId
			End Get
			Set
				OnProductIdChanging()
				_productId = value
				OnProductIdChanged()
			End Set
		End Property
		Partial Private Sub OnProductIdChanging()
		End Sub
		Partial Private Sub OnProductIdChanged()
		End Sub
		Public Overridable Property IsShoppingCart() As Boolean
			Get
				Return _isShoppingCart
			End Get
			Set
				OnIsShoppingCartChanging()
				_isShoppingCart = value
				OnIsShoppingCartChanged()
			End Set
		End Property
		Partial Private Sub OnIsShoppingCartChanging()
		End Sub
		Partial Private Sub OnIsShoppingCartChanged()
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
