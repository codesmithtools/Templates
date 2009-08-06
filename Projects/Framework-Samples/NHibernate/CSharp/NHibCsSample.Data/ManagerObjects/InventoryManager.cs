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
	}
	
	partial class InventoryManager : ManagerBase<Inventory, string>, IInventoryManager
    {
	}
}