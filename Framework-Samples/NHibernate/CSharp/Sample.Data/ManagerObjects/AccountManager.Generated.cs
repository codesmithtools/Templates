using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface IAccountManager : IManagerBase<Sample.Data.Generated.BusinessObjects.Account, int>
    {
		// Get Methods
		IList<Account> GetByUniqueID(System.Int32 profile);
    }

    partial class AccountManager : ManagerBase<Sample.Data.Generated.BusinessObjects.Account, int>, IAccountManager
    {
		#region Constructors
		
		public AccountManager() : base()
        {
        }
        public AccountManager(INHibernateSession session) : base(session)
        {
        }
		
		#endregion
		
        #region Get Methods

		
		public IList<Account> GetByUniqueID(System.Int32 profile)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(Account));
			
			
			ICriteria profileCriteria = criteria.CreateCriteria("Profile");
            profileCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", profile));
			
			return criteria.List<Account>();
        }
		
		#endregion
    }
}