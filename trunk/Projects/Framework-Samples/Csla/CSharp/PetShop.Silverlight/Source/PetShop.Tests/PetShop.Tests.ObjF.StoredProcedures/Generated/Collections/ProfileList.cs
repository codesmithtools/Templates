//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v5.2.1, CSLA Templates: v1.5.0.0, CSLA Framework: v3.8.0.
//       Changes to this template will not be lost.
//
//     Template: EditableRootList.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
#region Using declarations

using System;
using System.Collections.Generic;
using Csla;

#endregion

namespace PetShop.Tests.ObjF.StoredProcedures
{
    public partial class ProfileList
    {
        #region Authorization Rules

        protected void AddAuthorizationRules()
        {
            //// More information on these rules can be found here (http://www.devx.com/codemag/Article/40663/1763/page/2).

            //string[] canWrite = { "AdminUser", "RegularUser" };
            //string[] canRead = { "AdminUser", "RegularUser", "ReadOnlyUser" };
            //string[] admin = { "AdminUser" };

            // AuthorizationRules.AllowCreate(typeof(ProfileList), admin);
            // AuthorizationRules.AllowDelete(typeof(ProfileList), admin);
            // AuthorizationRules.AllowEdit(typeof(ProfileList), canWrite);
            // AuthorizationRules.AllowGet(typeof(ProfileList), canRead);

            //// UniqueID
            // AuthorizationRules.AllowWrite(_uniqueIDProperty, canWrite);
            // AuthorizationRules.AllowRead(_uniqueIDProperty, canRead);

            //// Username
            // AuthorizationRules.AllowRead(_usernameProperty, canRead);

            //// ApplicationName
            // AuthorizationRules.AllowRead(_applicationNameProperty, canRead);

            //// IsAnonymous
            // AuthorizationRules.AllowRead(_isAnonymousProperty, canRead);

            //// LastActivityDate
            // AuthorizationRules.AllowRead(_lastActivityDateProperty, canRead);

            //// LastUpdatedDate
            // AuthorizationRules.AllowRead(_lastUpdatedDateProperty, canRead);

// NOTE: Many-To-Many support coming soon.
            //// Accounts
            // AuthorizationRules.AllowRead(_accountsProperty, canRead);

            //// Carts
            // AuthorizationRules.AllowRead(_cartsProperty, canRead);

        }

        #endregion

    }
}