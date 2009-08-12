using System;
using System.Collections;
using System.Collections.Generic;

using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.BusinessObjects
{
    public partial class Product : BusinessBase<string>
    {
        #region Declarations

		private string _name = null;
		private string _descn = null;
		private string _image = null;
		
		private Category _category = null;
		
		private IList<Item> _items = new List<Item>();
		
		#endregion

        #region Constructors

        public Product() { }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append(this.GetType().FullName);
			sb.Append(_name);
			sb.Append(_descn);
			sb.Append(_image);

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
		
		public virtual Category Category
        {
            get { return _category; }
			set
			{
				OnCategoryChanging();
				_category = value;
				OnCategoryChanged();
			}
        }
		partial void OnCategoryChanging();
		partial void OnCategoryChanged();
		
		public virtual IList<Item> Items
        {
            get { return _items; }
            set
			{
				OnItemsChanging();
				_items = value;
				OnItemsChanged();
			}
        }
		partial void OnItemsChanging();
		partial void OnItemsChanged();
		
        #endregion
    }
}
