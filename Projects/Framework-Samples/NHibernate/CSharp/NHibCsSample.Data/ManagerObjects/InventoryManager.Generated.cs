using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface IInventoryManager : IManagerBase<Inventory, string>
    {
		// Get Methods
    }

    partial class InventoryManager : ManagerBase<Inventory, string>, IInventoryManager
    {
		#region Constructors
		
		public InventoryManager() : base()
        {
        }
        public InventoryManager(INHibernateSession session) : base(session)
        {
        }
		
		#endregion
		
        #region Get Methods

		
		#endregion
    }
}