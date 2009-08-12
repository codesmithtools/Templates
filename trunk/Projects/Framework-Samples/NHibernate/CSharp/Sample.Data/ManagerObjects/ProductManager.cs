using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface IProductManager : IManagerBase<Product, string>
    {
	}
	
	partial class ProductManager : ManagerBase<Product, string>, IProductManager
    {
	}
}