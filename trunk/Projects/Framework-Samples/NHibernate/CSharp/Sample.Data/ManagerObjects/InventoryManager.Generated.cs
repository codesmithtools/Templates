using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface IInventoryManager : IManagerBase<Sample.Data.Generated.BusinessObjects.Inventory, string>
    {
		// Get Methods
    }

    partial class InventoryManager : ManagerBase<Sample.Data.Generated.BusinessObjects.Inventory, string>, IInventoryManager
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