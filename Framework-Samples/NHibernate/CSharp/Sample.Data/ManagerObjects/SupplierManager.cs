using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface ISupplierManager : IManagerBase<Supplier, int>
    {
	}
	
	partial class SupplierManager : ManagerBase<Supplier, int>, ISupplierManager
    {
	}
}