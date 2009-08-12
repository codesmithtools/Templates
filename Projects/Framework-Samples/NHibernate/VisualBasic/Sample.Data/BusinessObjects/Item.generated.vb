Imports System
Imports System.Collections
Imports System.Collections.Generic

Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.BusinessObjects
	Public Partial Class Item
		Inherits BusinessBase(Of System.String)

		#region "Declarations"

		Private _listPrice As Decimal? = Nothing
		Private _unitCost As Decimal? = Nothing
		Private _status As String = Nothing
		Private _name As String = Nothing
		Private _image As String = Nothing
		
		Private _product As Product = Nothing
		Private _supplier As Supplier = Nothing
		
				
        #End Region

        #region "Constructors"

        Public Sub New()
		End Sub

        #End Region

        #region "Methods"

		Public Overloads Overrides Function GetHashCode() As Integer
			Dim sb As New System.Text.StringBuilder()

			sb.Append(Me.[GetType]().FullName)
			
			sb.Append(_listPrice)
			sb.Append(_unitCost)
			sb.Append(_status)
			sb.Append(_name)
			sb.Append(_image)
			
			Return sb.ToString().GetHashCode()
		End Function

        #End Region

        #region "Properties"

		Public Overridable Property ListPrice() As Decimal?
			Get
				Return _listPrice
			End Get
			Set
				OnListPriceChanging()
				_listPrice = value
				OnListPriceChanged()
			End Set
		End Property
		Partial Private Sub OnListPriceChanging()
		End Sub
		Partial Private Sub OnListPriceChanged()
		End Sub
		Public Overridable Property UnitCost() As Decimal?
			Get
				Return _unitCost
			End Get
			Set
				OnUnitCostChanging()
				_unitCost = value
				OnUnitCostChanged()
			End Set
		End Property
		Partial Private Sub OnUnitCostChanging()
		End Sub
		Partial Private Sub OnUnitCostChanged()
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
		Public Overridable Property Image() As String
			Get
				Return _image
			End Get
			Set
				OnImageChanging()
				_image = value
				OnImageChanged()
			End Set
		End Property
		Partial Private Sub OnImageChanging()
		End Sub
		Partial Private Sub OnImageChanged()
		End Sub
		
		Public Overridable Property Product() As Product
			Get
				Return _product
			End Get
			Set
				OnProductChanging()
				_product = value
				OnProductChanged()
			End Set
		End Property
		Partial Private Sub OnProductChanging()
		End Sub
		Partial Private Sub OnProductChanged()
		End Sub
		
		Public Overridable Property Supplier() As Supplier
			Get
				Return _supplier
			End Get
			Set
				OnSupplierChanging()
				_supplier = value
				OnSupplierChanged()
			End Set
		End Property
		Partial Private Sub OnSupplierChanging()
		End Sub
		Partial Private Sub OnSupplierChanged()
		End Sub
		
        #End Region

	End Class
End Namespace
