using System;
using System.Collections;
using System.Collections.Generic;

using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.BusinessObjects
{
    public partial class Inventory : BusinessBase<string>
    {
        #region Declarations

		private int _qty = default(Int32);
		
		
		
		#endregion

        #region Constructors

        public Inventory() { }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append(this.GetType().FullName);
			sb.Append(_qty);

            return sb.ToString().GetHashCode();
        }

        #endregion

        #region Properties

		public virtual int Qty
        {
            get { return _qty; }
			set
			{
				OnQtyChanging();
				_qty = value;
				OnQtyChanged();
			}
        }
		partial void OnQtyChanging();
		partial void OnQtyChanged();
		
        #endregion
    }
}
