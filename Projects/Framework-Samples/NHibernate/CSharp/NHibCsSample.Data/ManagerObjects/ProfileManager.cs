using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface IProfileManager : IManagerBase<Profile, int>
    {
	}
	
	partial class ProfileManager : ManagerBase<Profile, int>, IProfileManager
    {
	}
}