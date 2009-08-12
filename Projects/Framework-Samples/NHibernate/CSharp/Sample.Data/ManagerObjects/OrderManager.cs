using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface IOrderManager : IManagerBase<Order, int>
    {
	}
	
	partial class OrderManager : ManagerBase<Order, int>, IOrderManager
    {
	}
}