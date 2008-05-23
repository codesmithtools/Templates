using System;

namespace Northwind.DAL
{
	/// <summary>
	/// This object represents the properties and methods of a OrderDetail.
	/// </summary>
	public class OrderDetail
	{
		private Int32 _orderID;
		private Int32 _productID;
		private Decimal _unitPrice;
		private Int16 _quantity;
		private Decimal _discount;
		
		public OrderDetail()
		{
		}
		
		#region Custom - Methods
		// Insert custom methods in here so that they are preserved during re-generation.
		#endregion
	}
}
