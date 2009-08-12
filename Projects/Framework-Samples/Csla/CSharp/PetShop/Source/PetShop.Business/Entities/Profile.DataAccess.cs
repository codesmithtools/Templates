//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CSLA 3.7.X CodeSmith Templates.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'Profile.cs'.
//
//     Template: SwitchableObject.DataAccess.cst
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
	public partial class Profile
	{
		#region Root Data Access
		
		[RunLocal]
		protected override void DataPortal_Create()
		{
			//base.DataPortal_Create();

			ValidationRules.CheckRules();
		}
		
		private void DataPortal_Fetch(ProfileCriteria criteria)
		{
            using(SafeDataReader reader = DataAccessLayer.Instance.ProfileFetch(criteria.StateBag)) 
			{
				if(reader.Read())
				{	
					Fetch(reader);
				}
			}
        }
		
		[Transactional(TransactionalTypes.TransactionScope)]
		protected override void DataPortal_Insert()
		{
			using(SafeDataReader reader = DataAccessLayer.Instance.ProfileInsert(ReadProperty(_usernameProperty), ReadProperty(_applicationNameProperty), ReadProperty(_isAnonymousProperty), ReadProperty(_lastActivityDateProperty), ReadProperty(_lastUpdatedDateProperty)))
			{
				if(reader.Read())
				{

					LoadProperty(_uniqueIDProperty, reader.GetInt32("UniqueID"));
				}
			}
            
            FieldManager.UpdateChildren(this);
		}
		
		[Transactional(TransactionalTypes.TransactionScope)]
		protected override void DataPortal_Update()
		{
            using(SafeDataReader reader = DataAccessLayer.Instance.ProfileUpdate(ReadProperty(_uniqueIDProperty), ReadProperty(_usernameProperty), ReadProperty(_applicationNameProperty), ReadProperty(_isAnonymousProperty), ReadProperty(_lastActivityDateProperty), ReadProperty(_lastUpdatedDateProperty)))
            {
			}
            
            FieldManager.UpdateChildren(this);
		}
		
		[Transactional(TransactionalTypes.TransactionScope)]
		protected override void DataPortal_DeleteSelf()
		{
            DataPortal_Delete(new ProfileCriteria(UniqueID));
        }
		
		[Transactional(TransactionalTypes.TransactionScope)]
		protected override void DataPortal_Delete(object criteria)
		{
			ProfileCriteria theCriteria = criteria as ProfileCriteria;
            if (theCriteria != null)
            {
				using(SafeDataReader reader = DataAccessLayer.Instance.ProfileDelete(theCriteria.StateBag)) 
				{
				}
			}
        }

		#endregion
		
		#region Child Data Access

		protected override void Child_Create()
		{
			// TODO: load default values
			// omit this override if you have no defaults to set
		    //base.Child_Create();
		}
	
		private void Child_Fetch(SafeDataReader reader)
		{
			Fetch(reader);
            
            MarkAsChild();
		}
		
		private void Child_Insert()
		{
			using(SafeDataReader reader = DataAccessLayer.Instance.ProfileInsert(ReadProperty(_usernameProperty), ReadProperty(_applicationNameProperty), ReadProperty(_isAnonymousProperty), ReadProperty(_lastActivityDateProperty), ReadProperty(_lastUpdatedDateProperty)))
			{
				if(reader.Read())
				{

					LoadProperty(_uniqueIDProperty, reader.GetInt32("UniqueID"));
				}
			}
		}
		
		private void Child_Update()
		{
            using(SafeDataReader reader = DataAccessLayer.Instance.ProfileUpdate(ReadProperty(_uniqueIDProperty), ReadProperty(_usernameProperty), ReadProperty(_applicationNameProperty), ReadProperty(_isAnonymousProperty), ReadProperty(_lastActivityDateProperty), ReadProperty(_lastUpdatedDateProperty)))
            {
			}
		}
		
		private void Child_DeleteSelf()
		{
			DataPortal_Delete(new ProfileCriteria(UniqueID));
		}

		#endregion

        private void Fetch(SafeDataReader reader)
		{
			LoadProperty(_uniqueIDProperty, reader.GetInt32("UniqueID"));
			LoadProperty(_usernameProperty, reader.GetString("Username"));
			LoadProperty(_applicationNameProperty, reader.GetString("ApplicationName"));
			LoadProperty(_isAnonymousProperty, reader.GetBoolean("IsAnonymous"));
			LoadProperty(_lastActivityDateProperty, reader.GetDateTime("LastActivityDate"));
			LoadProperty(_lastUpdatedDateProperty, reader.GetDateTime("LastUpdatedDate"));


            MarkOld();
        }

	}
}