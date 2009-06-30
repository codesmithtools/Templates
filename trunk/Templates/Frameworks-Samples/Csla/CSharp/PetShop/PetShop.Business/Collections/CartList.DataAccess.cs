
//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CSLA 3.6.x CodeSmith Templates.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'CartList.cs'.
//
//     Template: EditableChildList.DataAccess.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
#region using declarations

using System;

using Csla;
using Csla.Data;

using PetShop.Data;
#endregion

namespace PetShop.Business
{
	public partial class CartList
	{
		#region Data Access
		
        [RunLocal]
        protected override void DataPortal_Create()
        {
        }

		private void Child_Fetch(CartCriteria criteria)
		{
			RaiseListChangedEvents = false;
			
			using(SafeDataReader reader = DataAccessLayer.Instance.CartFetch(criteria.StateBag)) 
			{
                while(reader.Read())
				{	
                    this.Add(new PetShop.Business.Cart(reader));
				}
			}
			
			RaiseListChangedEvents = true;
		}
		
		#endregion
	}
}