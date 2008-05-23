using System;

namespace Northwind.DAL
{
	/// <summary>
	/// This object represents the properties and methods of a Order.
	/// </summary>
	public class Order
	{
		private Int32 _orderID;
		private String _customerID;
		private Int32 _employeeID;
		private DateTime _orderDate;
		private DateTime _requiredDate;
		private DateTime _shippedDate;
		private Int32 _shipVia;
		private Decimal _freight;
		private String _shipName;
		private String _shipAddress;
		private String _shipCity;
		private String _shipRegion;
		private String _shipPostalCode;
		private String _shipCountry;
		
		public Order()
		{
		}
		
		#region Custom - Methods
		// Insert custom methods in here so that they are preserved during re-generation.
		#endregion
	}
}
