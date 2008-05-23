Imports System

Namespace Northwind.DAL
	''' <summary>
	''' This object represents the properties and methods of a Order.
	''' </summary>
	Public Class Order
		Private _orderID As Int32
		Private _customerID As String
		Private _employeeID As Int32
		Private _orderDate As DateTime
		Private _requiredDate As DateTime
		Private _shippedDate As DateTime
		Private _shipVia As Int32
		Private _freight As Decimal
		Private _shipName As String
		Private _shipAddress As String
		Private _shipCity As String
		Private _shipRegion As String
		Private _shipPostalCode As String
		Private _shipCountry As String

		Public Sub New()
		End Sub

		#region Custom - Methods
		' Insert custom methods in here so that they are preserved during re-generation.
#End Region
	End Class
End Namespace

