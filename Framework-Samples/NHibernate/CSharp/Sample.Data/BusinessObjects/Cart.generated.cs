using System;
using System.Collections;
using System.Collections.Generic;

using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.BusinessObjects
{
    public partial class Cart : BusinessBase<int>
    {
        #region Declarations

		private string _itemId = String.Empty;
		private string _name = String.Empty;
		private string _type = String.Empty;
		private decimal _price = default(Decimal);
		private string _categoryId = String.Empty;
		private string _productId = String.Empty;
		private bool _isShoppingCart = default(Boolean);
		private int _quantity = default(Int32);
		
		private Profile _profile = null;
		
		
		#endregion

        #region Constructors

        public Cart() { }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append(this.GetType().FullName);
			sb.Append(_itemId);
			sb.Append(_name);
			sb.Append(_type);
			sb.Append(_price);
			sb.Append(_categoryId);
			sb.Append(_productId);
			sb.Append(_isShoppingCart);
			sb.Append(_quantity);

            return sb.ToString().GetHashCode();
        }

        #endregion

        #region Properties

		public virtual string ItemId
        {
            get { return _itemId; }
			set
			{
				OnItemIdChanging();
				_itemId = value;
				OnItemIdChanged();
			}
        }
		partial void OnItemIdChanging();
		partial void OnItemIdChanged();
		
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
		
		public virtual string Type
        {
            get { return _type; }
			set
			{
				OnTypeChanging();
				_type = value;
				OnTypeChanged();
			}
        }
		partial void OnTypeChanging();
		partial void OnTypeChanged();
		
		public virtual decimal Price
        {
            get { return _price; }
			set
			{
				OnPriceChanging();
				_price = value;
				OnPriceChanged();
			}
        }
		partial void OnPriceChanging();
		partial void OnPriceChanged();
		
		public virtual string CategoryId
        {
            get { return _categoryId; }
			set
			{
				OnCategoryIdChanging();
				_categoryId = value;
				OnCategoryIdChanged();
			}
        }
		partial void OnCategoryIdChanging();
		partial void OnCategoryIdChanged();
		
		public virtual string ProductId
        {
            get { return _productId; }
			set
			{
				OnProductIdChanging();
				_productId = value;
				OnProductIdChanged();
			}
        }
		partial void OnProductIdChanging();
		partial void OnProductIdChanged();
		
		public virtual bool IsShoppingCart
        {
            get { return _isShoppingCart; }
			set
			{
				OnIsShoppingCartChanging();
				_isShoppingCart = value;
				OnIsShoppingCartChanged();
			}
        }
		partial void OnIsShoppingCartChanging();
		partial void OnIsShoppingCartChanged();
		
		public virtual int Quantity
        {
            get { return _quantity; }
			set
			{
				OnQuantityChanging();
				_quantity = value;
				OnQuantityChanged();
			}
        }
		partial void OnQuantityChanging();
		partial void OnQuantityChanged();
		
		public virtual Profile Profile
        {
            get { return _profile; }
			set
			{
				OnProfileChanging();
				_profile = value;
				OnProfileChanged();
			}
        }
		partial void OnProfileChanging();
		partial void OnProfileChanged();
		
        #endregion
    }
}
