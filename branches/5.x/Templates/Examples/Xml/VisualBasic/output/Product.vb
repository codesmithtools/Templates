Imports System

Namespace Northwind.DAL
	''' <summary>
	''' This object represents the properties and methods of a Product.
	''' </summary>
	Public Class Product
		Private _productID As Int32
		Private _productName As String
		Private _supplierID As Int32
		Private _categoryID As Int32
		Private _quantityPerUnit As String
		Private _unitPrice As Decimal
		Private _unitsInStock As Int16
		Private _unitsOnOrder As Int16
		Private _reorderLevel As Int16
		Private _discontinued As Boolean

		Public Sub New()
		End Sub

		#region Custom - Methods
		' Insert custom methods in here so that they are preserved during re-generation.
#End Region
	End Class
End Namespace

