using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface IProfileManager : IManagerBase<Profile, int>
    {
	}
	
	partial class ProfileManager : ManagerBase<Profile, int>, IProfileManager
    {
	}
}