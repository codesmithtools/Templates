using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface ISupplierManager : IManagerBase<Supplier, int>
    {
		// Get Methods
    }

    partial class SupplierManager : ManagerBase<Supplier, int>, ISupplierManager
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