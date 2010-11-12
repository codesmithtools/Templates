using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface ISupplierManager : IManagerBase<Sample.Data.Generated.BusinessObjects.Supplier, int>
    {
		// Get Methods
    }

    partial class SupplierManager : ManagerBase<Sample.Data.Generated.BusinessObjects.Supplier, int>, ISupplierManager
    {
		#region Constructors
		
		public SupplierManager() : base()
        {
        }
        public SupplierManager(INHibernateSession session) : base(session)
        {
        }
		
		#endregion
		
        #region Get Methods

		
		#endregion
    }
}