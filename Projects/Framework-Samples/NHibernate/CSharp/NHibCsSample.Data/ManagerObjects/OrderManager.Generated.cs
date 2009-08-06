using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface IOrderManager : IManagerBase<Order, int>
    {
		// Get Methods
    }

    partial class OrderManager : ManagerBase<Order, int>, IOrderManager
    {
		#region Constructors
		
		public OrderManager() : base()
        {
        }
        public OrderManager(INHibernateSession session) : base(session)
        {
        }
		
		#endregion
		
        #region Get Methods

		
		#endregion
    }
}