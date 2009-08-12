using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface IProfileManager : IManagerBase<Profile, int>
    {
		// Get Methods
		IList<Profile> GetByUsernameApplicationName(System.String username, System.String applicationName);
    }

    partial class ProfileManager : ManagerBase<Profile, int>, IProfileManager
    {
		#region Constructors
		
		public ProfileManager() : base()
        {
        }
        public ProfileManager(INHibernateSession session) : base(session)
        {
        }
		
		#endregion
		
        #region Get Methods

		
		public IList<Profile> GetByUsernameApplicationName(System.String username, System.String applicationName)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(Profile));
			
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("Username", username));
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("ApplicationName", applicationName));
			
			return criteria.List<Profile>();
        }
		
		#endregion
    }
}