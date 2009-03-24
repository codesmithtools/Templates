using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface IAccountManager : IManagerBase<Account, int>
    {
	}
	
	partial class AccountManager : ManagerBase<Account, int>, IAccountManager
    {
	}
}