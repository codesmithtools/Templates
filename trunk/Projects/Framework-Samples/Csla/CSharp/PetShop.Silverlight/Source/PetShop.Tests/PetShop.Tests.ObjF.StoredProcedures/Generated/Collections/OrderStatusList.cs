//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v5.2.1, CSLA Templates: v1.5.0.0, CSLA Framework: v3.8.0.
//       Changes to this template will not be lost.
//
//     Template: EditableChildList.cst
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
    public partial class OrderStatusList
    {
        #region Authorization Rules

        protected void AddAuthorizationRules()
        {
            //// More information on these rules can be found here (http://www.devx.com/codemag/Article/40663/1763/page/2).

            //string[] canWrite = { "AdminUser", "RegularUser" };
            //string[] canRead = { "AdminUser", "RegularUser", "ReadOnlyUser" };
            //string[] admin = { "AdminUser" };

            // AuthorizationRules.AllowCreate(typeof(OrderStatusList), admin);
            // AuthorizationRules.AllowDelete(typeof(OrderStatusList), admin);
            // AuthorizationRules.AllowEdit(typeof(OrderStatusList), canWrite);
            // AuthorizationRules.AllowGet(typeof(OrderStatusList), canRead);

            //// OrderId
            // AuthorizationRules.AllowRead(_orderIdProperty, canRead);

            //// LineNum
            // AuthorizationRules.AllowRead(_lineNumProperty, canRead);

            //// Timestamp
            // AuthorizationRules.AllowRead(_timestampProperty, canRead);

            //// Status
            // AuthorizationRules.AllowRead(_statusProperty, canRead);

            //// OrderMember
            // AuthorizationRules.AllowRead(_orderMemberProperty, canRead);

// NOTE: Many-To-Many support coming soon.
        }

        #endregion

    }
}