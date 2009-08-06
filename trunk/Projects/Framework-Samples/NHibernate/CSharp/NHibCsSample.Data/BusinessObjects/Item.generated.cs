using System;
using System.Collections;
using System.Collections.Generic;

using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.BusinessObjects
{
    public partial class Item : BusinessBase<string>
    {
        #region Declarations

		private decimal? _listPrice = null;
		private decimal? _unitCost = null;
		private string _status = null;
		private string _name = null;
		private string _image = null;
		
		private Product _product = null;
		private Supplier _supplier = null;
		
		
		#endregion

        #region Constructors

        public Item() { }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append(this.GetType().FullName);
			sb.Append(_listPrice);
			sb.Append(_unitCost);
			sb.Append(_status);
			sb.Append(_name);
			sb.Append(_image);

            return sb.ToString().GetHashCode();
        }

        #endregion

        #region Properties

		public virtual decimal? ListPrice
        {
            get { return _listPrice; }
			set
			{
				OnListPriceChanging();
				_listPrice = value;
				OnListPriceChanged();
			}
        }
		partial void OnListPriceChanging();
		partial void OnListPriceChanged();
		
		public virtual decimal? UnitCost
        {
            get { return _unitCost; }
			set
			{
				OnUnitCostChanging();
				_unitCost = value;
				OnUnitCostChanged();
			}
        }
		partial void OnUnitCostChanging();
		partial void OnUnitCostChanged();
		
		public virtual string Status
        {
            get { return _status; }
			set
			{
				OnStatusChanging();
				_status = value;
				OnStatusChanged();
			}
        }
		partial void OnStatusChanging();
		partial void OnStatusChanged();
		
		public virtual string Name
        {
            get { return _name; }
			set
			{
				OnNameChanging();
				_name = value;
				OnNameChanged();
			}
        }
		partial void OnNameChanging();
		partial void OnNameChanged();
		
		public virtual string Image
        {
            get { return _image; }
			set
			{
				OnImageChanging();
				_image = value;
				OnImageChanged();
			}
        }
		partial void OnImageChanging();
		partial void OnImageChanged();
		
		public virtual Product Product
        {
            get { return _product; }
			set
			{
				OnProductChanging();
				_product = value;
				OnProductChanged();
			}
        }
		partial void OnProductChanging();
		partial void OnProductChanged();
		
		public virtual Supplier Supplier
        {
            get { return _supplier; }
			set
			{
				OnSupplierChanging();
				_supplier = value;
				OnSupplierChanged();
			}
        }
		partial void OnSupplierChanging();
		partial void OnSupplierChanged();
		
        #endregion
    }
}
