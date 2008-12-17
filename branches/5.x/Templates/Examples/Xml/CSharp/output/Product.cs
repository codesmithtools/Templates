using System;

namespace Northwind.DAL
{
	/// <summary>
	/// This object represents the properties and methods of a Product.
	/// </summary>
	public class Product
	{
		private Int32 _productID;
		private String _productName;
		private Int32 _supplierID;
		private Int32 _categoryID;
		private String _quantityPerUnit;
		private Decimal _unitPrice;
		private Int16 _unitsInStock;
		private Int16 _unitsOnOrder;
		private Int16 _reorderLevel;
		private Boolean _discontinued;
		
		public Product()
		{
		}
		
		#region Custom - Methods
		// Insert custom methods in here so that they are preserved during re-generation.
		#endregion
	}
}
