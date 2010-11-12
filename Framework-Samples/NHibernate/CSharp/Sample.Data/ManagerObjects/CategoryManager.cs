using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface ICategoryManager : IManagerBase<Category, string>
    {
	}
	
	partial class CategoryManager : ManagerBase<Category, string>, ICategoryManager
    {
	}
}