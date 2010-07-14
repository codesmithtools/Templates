//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CSLA 3.7.X CodeSmith Templates.
//	   Changes to this template will not be lost.
//
//     Template: EditableRoot.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
#region using declarations

using System;

using Csla;
using Csla.Security;
using Csla.Validation;

#endregion

namespace PetShop.Business
{
	public partial class Category
	{
		#region Validation Rules
		
		/// <summary>
        /// All custom rules need to be placed in this method.
        /// </summary>
        /// <returns>Return true to override the generated rules; If false generated rules will be run.</returns>
		protected bool AddBusinessValidationRules()
		{
			// TODO: add validation rules
			//ValidationRules.AddRule(RuleMethod, "");

		    return false;
		}
		
		#endregion
		
		#region Authorization Rules
		
		protected override void AddAuthorizationRules()
		{
            //// More information on these rules can be found here (http://www.devx.com/codemag/Article/40663/1763/page/2).
            
            //string[] canWrite = { "AdminUser", "RegularUser" };
            //string[] canRead = { "AdminUser", "RegularUser", "ReadOnlyUser" };
            //string[] admin = { "AdminUser" };

            // AuthorizationRules.AllowCreate(typeof(Category), admin);
            // AuthorizationRules.AllowDelete(typeof(Category), admin);
            // AuthorizationRules.AllowEdit(typeof(Category), canWrite);
            // AuthorizationRules.AllowGet(typeof(Category), canRead);

            //// CategoryId
            // AuthorizationRules.AllowRead(_categoryIdProperty, canRead);
        
            //// Name
            // AuthorizationRules.AllowRead(_nameProperty, canRead);
        
            //// Description
            // AuthorizationRules.AllowRead(_descnProperty, canRead);
        
            //// Products
            // AuthorizationRules.AllowRead(_productsProperty, canRead);
        
		}
		
		private static void AddObjectAuthorizationRules()
		{
			// TODO: add authorization rules
			//AuthorizationRules.AllowEdit(typeof(Category), "Role");
		}
		
		#endregion
	}
}