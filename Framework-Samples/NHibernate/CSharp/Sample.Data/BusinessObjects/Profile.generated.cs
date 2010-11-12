using System;
using System.Collections;
using System.Collections.Generic;

using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.BusinessObjects
{
    public partial class Profile : BusinessBase<int>
    {
        #region Declarations

		private string _username = String.Empty;
		private string _applicationName = String.Empty;
		private bool? _isAnonymous = null;
		private System.DateTime? _lastActivityDate = null;
		private System.DateTime? _lastUpdatedDate = null;
		
		
		private IList<Account> _accounts = new List<Account>();
		private IList<Cart> _carts = new List<Cart>();
		
		#endregion

        #region Constructors

        public Profile() { }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.Append(this.GetType().FullName);
			sb.Append(_username);
			sb.Append(_applicationName);
			sb.Append(_isAnonymous);
			sb.Append(_lastActivityDate);
			sb.Append(_lastUpdatedDate);

            return sb.ToString().GetHashCode();
        }

        #endregion

        #region Properties

		public virtual string Username
        {
            get { return _username; }
			set
			{
				OnUsernameChanging();
				_username = value;
				OnUsernameChanged();
			}
        }
		partial void OnUsernameChanging();
		partial void OnUsernameChanged();
		
		public virtual string ApplicationName
        {
            get { return _applicationName; }
			set
			{
				OnApplicationNameChanging();
				_applicationName = value;
				OnApplicationNameChanged();
			}
        }
		partial void OnApplicationNameChanging();
		partial void OnApplicationNameChanged();
		
		public virtual bool? IsAnonymous
        {
            get { return _isAnonymous; }
			set
			{
				OnIsAnonymousChanging();
				_isAnonymous = value;
				OnIsAnonymousChanged();
			}
        }
		partial void OnIsAnonymousChanging();
		partial void OnIsAnonymousChanged();
		
		public virtual System.DateTime? LastActivityDate
        {
            get { return _lastActivityDate; }
			set
			{
				OnLastActivityDateChanging();
				_lastActivityDate = value;
				OnLastActivityDateChanged();
			}
        }
		partial void OnLastActivityDateChanging();
		partial void OnLastActivityDateChanged();
		
		public virtual System.DateTime? LastUpdatedDate
        {
            get { return _lastUpdatedDate; }
			set
			{
				OnLastUpdatedDateChanging();
				_lastUpdatedDate = value;
				OnLastUpdatedDateChanged();
			}
        }
		partial void OnLastUpdatedDateChanging();
		partial void OnLastUpdatedDateChanged();
		
		public virtual IList<Account> Accounts
        {
            get { return _accounts; }
            set
			{
				OnAccountsChanging();
				_accounts = value;
				OnAccountsChanged();
			}
        }
		partial void OnAccountsChanging();
		partial void OnAccountsChanged();
		
		public virtual IList<Cart> Carts
        {
            get { return _carts; }
            set
			{
				OnCartsChanging();
				_carts = value;
				OnCartsChanged();
			}
        }
		partial void OnCartsChanging();
		partial void OnCartsChanged();
		
        #endregion
    }
}
