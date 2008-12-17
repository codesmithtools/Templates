Imports System

Namespace Northwind.DAL
	''' <summary>
	''' This object represents the properties and methods of a OrderDetail.
	''' </summary>
	Public Class OrderDetail
		Private _orderID As Int32
		Private _productID As Int32
		Private _unitPrice As Decimal
		Private _quantity As Int16
		Private _discount As Decimal

		Public Sub New()
		End Sub

		#region Custom - Methods
		' Insert custom methods in here so that they are preserved during re-generation.
#End Region
	End Class
End Namespace

