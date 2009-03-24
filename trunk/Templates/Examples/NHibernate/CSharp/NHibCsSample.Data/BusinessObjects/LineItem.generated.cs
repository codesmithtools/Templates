using System;
using System.Collections;
using System.Collections.Generic;

using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.BusinessObjects
{
    public partial class LineItem : BusinessBase<string>
    {
        #region Declarations

		private int _orderId = default(Int32);
		private int _lineNum = default(Int32);
		private string _itemId = String.Empty;
		private int _quantity = default(Int32);
		private decimal _unitPrice = default(Decimal);
		
		
		
		#endregion

        #region Constructors

        public LineItem() { }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append(this.GetType().FullName);
			sb.Append(_orderId);
			sb.Append(_lineNum);
			sb.Append(_itemId);
			sb.Append(_quantity);
			sb.Append(_unitPrice);

            return sb.ToString().GetHashCode();
        }

        #endregion

        #region Properties

		public override string Id
		{
			get
			{
				System.Text.StringBuilder uniqueId = new System.Text.StringBuilder();
				uniqueId.Append(_orderId.ToString());
				uniqueId.Append("^");
				uniqueId.Append(_lineNum.ToString());
				return uniqueId.ToString();
			}
		}
		
		public virtual int OrderId
        {
            get { return _orderId; }
			set
			{
				OnOrderIdChanging();
				_orderId = value;
				OnOrderIdChanged();
			}
        }
		partial void OnOrderIdChanging();
		partial void OnOrderIdChanged();
		
		public virtual int LineNum
        {
            get { return _lineNum; }
			set
			{
				OnLineNumChanging();
				_lineNum = value;
				OnLineNumChanged();
			}
        }
		partial void OnLineNumChanging();
		partial void OnLineNumChanged();
		
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
		
		public virtual decimal UnitPrice
        {
            get { return _unitPrice; }
			set
			{
				OnUnitPriceChanging();
				_unitPrice = value;
				OnUnitPriceChanged();
			}
        }
		partial void OnUnitPriceChanging();
		partial void OnUnitPriceChanged();
		
        #endregion
    }
}
