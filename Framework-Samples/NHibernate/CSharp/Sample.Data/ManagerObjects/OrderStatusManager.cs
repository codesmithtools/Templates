using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface IOrderStatusManager : IManagerBase<OrderStatus, string>
    {
	}
	
	partial class OrderStatusManager : ManagerBase<OrderStatus, string>, IOrderStatusManager
    {
	}
}