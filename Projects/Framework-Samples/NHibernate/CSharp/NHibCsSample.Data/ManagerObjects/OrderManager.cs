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
	}
	
	partial class OrderManager : ManagerBase<Order, int>, IOrderManager
    {
	}
}