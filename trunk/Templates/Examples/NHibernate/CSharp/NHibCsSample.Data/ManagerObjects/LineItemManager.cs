using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface ILineItemManager : IManagerBase<LineItem, string>
    {
	}
	
	partial class LineItemManager : ManagerBase<LineItem, string>, ILineItemManager
    {
	}
}