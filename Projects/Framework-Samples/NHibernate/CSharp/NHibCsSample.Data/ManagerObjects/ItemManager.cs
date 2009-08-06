using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface IItemManager : IManagerBase<Item, string>
    {
	}
	
	partial class ItemManager : ManagerBase<Item, string>, IItemManager
    {
	}
}