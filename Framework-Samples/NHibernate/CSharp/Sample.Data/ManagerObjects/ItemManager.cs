using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface IItemManager : IManagerBase<Item, string>
    {
	}
	
	partial class ItemManager : ManagerBase<Item, string>, IItemManager
    {
	}
}