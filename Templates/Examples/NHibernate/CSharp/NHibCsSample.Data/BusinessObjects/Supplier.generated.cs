using System;
using System.Collections;
using System.Collections.Generic;

using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.BusinessObjects
{
    public partial class Supplier : BusinessBase<int>
    {
        #region Declarations

		private string _name = null;
		private string _status = String.Empty;
		private string _addr1 = null;
		private string _addr2 = null;
		private string _city = null;
		private string _state = null;
		private string _zip = null;
		private string _phone = null;
		
		
		private IList<Item> _items = new List<Item>();
		
		#endregion

        #region Constructors

        public Supplier() { }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append(this.GetType().FullName);
			sb.Append(_name);
			sb.Append(_status);
			sb.Append(_addr1);
			sb.Append(_addr2);
			sb.Append(_city);
			sb.Append(_state);
			sb.Append(_zip);
			sb.Append(_phone);

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
		
		public virtual string Addr1
        {
            get { return _addr1; }
			set
			{
				OnAddr1Changing();
				_addr1 = value;
				OnAddr1Changed();
			}
        }
		partial void OnAddr1Changing();
		partial void OnAddr1Changed();
		
		public virtual string Addr2
        {
            get { return _addr2; }
			set
			{
				OnAddr2Changing();
				_addr2 = value;
				OnAddr2Changed();
			}
        }
		partial void OnAddr2Changing();
		partial void OnAddr2Changed();
		
		public virtual string City
        {
            get { return _city; }
			set
			{
				OnCityChanging();
				_city = value;
				OnCityChanged();
			}
        }
		partial void OnCityChanging();
		partial void OnCityChanged();
		
		public virtual string State
        {
            get { return _state; }
			set
			{
				OnStateChanging();
				_state = value;
				OnStateChanged();
			}
        }
		partial void OnStateChanging();
		partial void OnStateChanged();
		
		public virtual string Zip
        {
            get { return _zip; }
			set
			{
				OnZipChanging();
				_zip = value;
				OnZipChanged();
			}
        }
		partial void OnZipChanging();
		partial void OnZipChanged();
		
		public virtual string Phone
        {
            get { return _phone; }
			set
			{
				OnPhoneChanging();
				_phone = value;
				OnPhoneChanged();
			}
        }
		partial void OnPhoneChanging();
		partial void OnPhoneChanged();
		
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
