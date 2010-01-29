using System;
using System.Collections;
using System.Collections.Generic;

using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.BusinessObjects
{
    public partial class Account : BusinessBase<int>
    {
        #region Declarations

		private string _email = String.Empty;
		private string _firstName = String.Empty;
		private string _lastName = String.Empty;
		private string _address1 = String.Empty;
		private string _address2 = null;
		private string _city = String.Empty;
		private string _state = String.Empty;
		private string _zip = String.Empty;
		private string _country = String.Empty;
		private string _phone = null;
		
		private Profile _profile = null;
		
		
		#endregion

        #region Constructors

        public Account() { }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append(this.GetType().FullName);
			sb.Append(_email);
			sb.Append(_firstName);
			sb.Append(_lastName);
			sb.Append(_address1);
			sb.Append(_address2);
			sb.Append(_city);
			sb.Append(_state);
			sb.Append(_zip);
			sb.Append(_country);
			sb.Append(_phone);

            return sb.ToString().GetHashCode();
        }

        #endregion

        #region Properties

		public virtual string Email
        {
            get { return _email; }
			set
			{
				OnEmailChanging();
				_email = value;
				OnEmailChanged();
			}
        }
		partial void OnEmailChanging();
		partial void OnEmailChanged();
		
		public virtual string FirstName
        {
            get { return _firstName; }
			set
			{
				OnFirstNameChanging();
				_firstName = value;
				OnFirstNameChanged();
			}
        }
		partial void OnFirstNameChanging();
		partial void OnFirstNameChanged();
		
		public virtual string LastName
        {
            get { return _lastName; }
			set
			{
				OnLastNameChanging();
				_lastName = value;
				OnLastNameChanged();
			}
        }
		partial void OnLastNameChanging();
		partial void OnLastNameChanged();
		
		public virtual string Address1
        {
            get { return _address1; }
			set
			{
				OnAddress1Changing();
				_address1 = value;
				OnAddress1Changed();
			}
        }
		partial void OnAddress1Changing();
		partial void OnAddress1Changed();
		
		public virtual string Address2
        {
            get { return _address2; }
			set
			{
				OnAddress2Changing();
				_address2 = value;
				OnAddress2Changed();
			}
        }
		partial void OnAddress2Changing();
		partial void OnAddress2Changed();
		
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
		
		public virtual string Country
        {
            get { return _country; }
			set
			{
				OnCountryChanging();
				_country = value;
				OnCountryChanged();
			}
        }
		partial void OnCountryChanging();
		partial void OnCountryChanged();
		
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
