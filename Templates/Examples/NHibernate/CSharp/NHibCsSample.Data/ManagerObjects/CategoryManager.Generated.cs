using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface ICategoryManager : IManagerBase<Category, string>
    {
		// Get Methods
    }

    partial class CategoryManager : ManagerBase<Category, string>, ICategoryManager
    {
		#region Constructors
		
		public CategoryManager() : base()
        {
        }
        public CategoryManager(INHibernateSession session) : base(session)
        {
        }
		
		#endregion
		
        #region Get Methods

		
		#endregion
    }
}