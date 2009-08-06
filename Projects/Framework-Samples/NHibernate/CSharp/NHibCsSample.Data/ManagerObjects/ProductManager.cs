using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface IProductManager : IManagerBase<Product, string>
    {
	}
	
	partial class ProductManager : ManagerBase<Product, string>, IProductManager
    {
	}
}