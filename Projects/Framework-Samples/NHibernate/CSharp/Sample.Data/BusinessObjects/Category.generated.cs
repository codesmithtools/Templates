using System;
using System.Collections;
using System.Collections.Generic;

using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.BusinessObjects
{
    public partial class Category : BusinessBase<string>
    {
        #region Declarations

		private string _name = null;
		private string _descn = null;
		
		
		private IList<Product> _products = new List<Product>();
		
		#endregion

        #region Constructors

        public Category() { }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append(this.GetType().FullName);
			sb.Append(_name);
			sb.Append(_descn);

            return sb.ToString().GetHashCode();
        }

        #endregion

        #region Properties

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
		
		public virtual string Descn
        {
            get { return _descn; }
			set
			{
				OnDescnChanging();
				_descn = value;
				OnDescnChanged();
			}
        }
		partial void OnDescnChanging();
		partial void OnDescnChanged();
		
		public virtual IList<Product> Products
        {
            get { return _products; }
            set
			{
				OnProductsChanging();
				_products = value;
				OnProductsChanged();
			}
        }
		partial void OnProductsChanging();
		partial void OnProductsChanged();
		
        #endregion
    }
}
