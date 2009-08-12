using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface ICartManager : IManagerBase<Cart, int>
    {
	}
	
	partial class CartManager : ManagerBase<Cart, int>, ICartManager
    {
	}
}