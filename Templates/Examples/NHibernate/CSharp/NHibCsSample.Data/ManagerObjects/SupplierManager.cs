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
	}
	
	partial class SupplierManager : ManagerBase<Supplier, int>, ISupplierManager
    {
	}
}