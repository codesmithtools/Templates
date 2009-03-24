using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface ICartManager : IManagerBase<Cart, int>
    {
	}
	
	partial class CartManager : ManagerBase<Cart, int>, ICartManager
    {
	}
}