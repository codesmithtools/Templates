using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface ICategoryManager : IManagerBase<Category, string>
    {
	}
	
	partial class CategoryManager : ManagerBase<Category, string>, ICategoryManager
    {
	}
}