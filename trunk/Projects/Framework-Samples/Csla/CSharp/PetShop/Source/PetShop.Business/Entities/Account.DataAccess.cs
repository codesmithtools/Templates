﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v5.2.3, CSLA Templates: v3.0.1.1934, CSLA Framework: v3.8.4.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'Account.cs'.
//
//     Template: EditableChild.DataAccess.ParameterizedSQL.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
#region Using declarations

using System;
using System.Data;
using System.Data.SqlClient;

using Csla;
using Csla.Data;

#endregion

namespace PetShop.Business
{
    public partial class Account
    {
        protected override void Child_Create()
        {
            bool cancel = false;
            OnChildCreating(ref cancel);
            if (cancel) return;

            ValidationRules.CheckRules();

            OnChildCreated();
        }

        private void Child_Fetch(AccountCriteria criteria)
        {
            bool cancel = false;
            OnChildFetching(criteria, ref cancel);
            if (cancel) return;

            string commandText = string.Format("SELECT [AccountId], [UniqueID], [Email], [FirstName], [LastName], [Address1], [Address2], [City], [State], [Zip], [Country], [Phone] FROM [dbo].[Account] {0}", ADOHelper.BuildWhereStatement(criteria.StateBag));
            using (SqlConnection connection = new SqlConnection(ADOHelper.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddRange(ADOHelper.SqlParameters(criteria.StateBag));
                    using(var reader = new SafeDataReader(command.ExecuteReader()))
                    {
                        if(reader.Read())
                            Map(reader);
                        else
                            throw new Exception(string.Format("The record was not found in 'Account' using the following criteria: {0}.", criteria));
                    }
                }
            }

            OnChildFetched();
        }

        #region Child_Insert

        private void Child_Insert(SqlConnection connection)
        {
            bool cancel = false;
            OnChildInserting(connection, ref cancel);
            if (cancel) return;

            if(connection.State != ConnectionState.Open) connection.Open();
            const string commandText = "INSERT INTO [dbo].[Account] ([UniqueID], [Email], [FirstName], [LastName], [Address1], [Address2], [City], [State], [Zip], [Country], [Phone]) VALUES (@p_UniqueID, @p_Email, @p_FirstName, @p_LastName, @p_Address1, @p_Address2, @p_City, @p_State, @p_Zip, @p_Country, @p_Phone); SELECT [AccountId] FROM [dbo].[Account] WHERE AccountId = SCOPE_IDENTITY()";
            using(SqlCommand command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@p_UniqueID", this.UniqueID);
					command.Parameters.AddWithValue("@p_Email", this.Email);
					command.Parameters.AddWithValue("@p_FirstName", this.FirstName);
					command.Parameters.AddWithValue("@p_LastName", this.LastName);
					command.Parameters.AddWithValue("@p_Address1", this.Address1);
					command.Parameters.AddWithValue("@p_Address2", ADOHelper.NullCheck(this.Address2));
					command.Parameters.AddWithValue("@p_City", this.City);
					command.Parameters.AddWithValue("@p_State", this.State);
					command.Parameters.AddWithValue("@p_Zip", this.Zip);
					command.Parameters.AddWithValue("@p_Country", this.Country);
					command.Parameters.AddWithValue("@p_Phone", ADOHelper.NullCheck(this.Phone));

                using(var reader = new SafeDataReader(command.ExecuteReader()))
                {
                    if(reader.Read())
                    {

                        // Update identity primary key value.
                        LoadProperty(_accountIdProperty, reader.GetInt32("AccountId"));
                    }
                }
            }

            FieldManager.UpdateChildren(this, connection);
            OnChildInserted();
        }

        private void Child_Insert(Profile profile, SqlConnection connection)
        {
            bool cancel = false;
            OnChildInserting(connection, ref cancel);
            if (cancel) return;

            if(connection.State != ConnectionState.Open) connection.Open();
            const string commandText = "INSERT INTO [dbo].[Account] ([UniqueID], [Email], [FirstName], [LastName], [Address1], [Address2], [City], [State], [Zip], [Country], [Phone]) VALUES (@p_UniqueID, @p_Email, @p_FirstName, @p_LastName, @p_Address1, @p_Address2, @p_City, @p_State, @p_Zip, @p_Country, @p_Phone); SELECT [AccountId] FROM [dbo].[Account] WHERE AccountId = SCOPE_IDENTITY()";
            using(SqlCommand command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@p_UniqueID", profile != null ? profile.UniqueID : this.UniqueID);
					command.Parameters.AddWithValue("@p_Email", this.Email);
					command.Parameters.AddWithValue("@p_FirstName", this.FirstName);
					command.Parameters.AddWithValue("@p_LastName", this.LastName);
					command.Parameters.AddWithValue("@p_Address1", this.Address1);
					command.Parameters.AddWithValue("@p_Address2", ADOHelper.NullCheck(this.Address2));
					command.Parameters.AddWithValue("@p_City", this.City);
					command.Parameters.AddWithValue("@p_State", this.State);
					command.Parameters.AddWithValue("@p_Zip", this.Zip);
					command.Parameters.AddWithValue("@p_Country", this.Country);
					command.Parameters.AddWithValue("@p_Phone", ADOHelper.NullCheck(this.Phone));

                using(var reader = new SafeDataReader(command.ExecuteReader()))
                {
                    if(reader.Read())
                    {

                        // Update identity primary key value.
                        LoadProperty(_accountIdProperty, reader.GetInt32("AccountId"));
                    }
                }

                // Update foreign keys values. This code will update the values passed in from the parent only if no errors occurred after executing the query.
                if(profile != null && profile.UniqueID != this.UniqueID)
                    LoadProperty(_uniqueIDProperty, profile.UniqueID);
            }
            
            // A child relationship exists on this Business Object but its type is not a child type (E.G. EditableChild). 
            // TODO: Please override OnChildInserted() and insert this child manually.
            // FieldManager.UpdateChildren(this, connection);
            OnChildInserted();
        }

        #endregion

        #region Child_Update

        private void Child_Update(SqlConnection connection)
        {
            bool cancel = false;
            OnChildUpdating(connection, ref cancel);
            if (cancel) return;

            if(connection.State != ConnectionState.Open) connection.Open();
            const string commandText = "UPDATE [dbo].[Account]  SET [UniqueID] = @p_UniqueID, [Email] = @p_Email, [FirstName] = @p_FirstName, [LastName] = @p_LastName, [Address1] = @p_Address1, [Address2] = @p_Address2, [City] = @p_City, [State] = @p_State, [Zip] = @p_Zip, [Country] = @p_Country, [Phone] = @p_Phone WHERE [AccountId] = @p_AccountId";
            using(SqlCommand command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@p_AccountId", this.AccountId);
					command.Parameters.AddWithValue("@p_UniqueID", this.UniqueID);
					command.Parameters.AddWithValue("@p_Email", this.Email);
					command.Parameters.AddWithValue("@p_FirstName", this.FirstName);
					command.Parameters.AddWithValue("@p_LastName", this.LastName);
					command.Parameters.AddWithValue("@p_Address1", this.Address1);
					command.Parameters.AddWithValue("@p_Address2", ADOHelper.NullCheck(this.Address2));
					command.Parameters.AddWithValue("@p_City", this.City);
					command.Parameters.AddWithValue("@p_State", this.State);
					command.Parameters.AddWithValue("@p_Zip", this.Zip);
					command.Parameters.AddWithValue("@p_Country", this.Country);
					command.Parameters.AddWithValue("@p_Phone", ADOHelper.NullCheck(this.Phone));

                using(var reader = new SafeDataReader(command.ExecuteReader()))
                {
                    if(reader.Read())
                    {
                    }
                }
            }

            FieldManager.UpdateChildren(this, connection);

            OnChildUpdated();
        }
 
        private void Child_Update(Profile profile, SqlConnection connection)
        {
            bool cancel = false;
            OnChildUpdating(connection, ref cancel);
            if (cancel) return;

            if(connection.State != ConnectionState.Open) connection.Open();
            const string commandText = "UPDATE [dbo].[Account]  SET [UniqueID] = @p_UniqueID, [Email] = @p_Email, [FirstName] = @p_FirstName, [LastName] = @p_LastName, [Address1] = @p_Address1, [Address2] = @p_Address2, [City] = @p_City, [State] = @p_State, [Zip] = @p_Zip, [Country] = @p_Country, [Phone] = @p_Phone WHERE [AccountId] = @p_AccountId";
            using(SqlCommand command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@p_AccountId", this.AccountId);
					command.Parameters.AddWithValue("@p_UniqueID", profile != null ? profile.UniqueID : this.UniqueID);
					command.Parameters.AddWithValue("@p_Email", this.Email);
					command.Parameters.AddWithValue("@p_FirstName", this.FirstName);
					command.Parameters.AddWithValue("@p_LastName", this.LastName);
					command.Parameters.AddWithValue("@p_Address1", this.Address1);
					command.Parameters.AddWithValue("@p_Address2", ADOHelper.NullCheck(this.Address2));
					command.Parameters.AddWithValue("@p_City", this.City);
					command.Parameters.AddWithValue("@p_State", this.State);
					command.Parameters.AddWithValue("@p_Zip", this.Zip);
					command.Parameters.AddWithValue("@p_Country", this.Country);
					command.Parameters.AddWithValue("@p_Phone", ADOHelper.NullCheck(this.Phone));

                using(var reader = new SafeDataReader(command.ExecuteReader()))
                {
                    if(reader.Read())
                    {
                    }
                }

                // Update foreign keys values. This code will update the values passed in from the parent only if no errors occurred after executing the query.
                if(profile != null && profile.UniqueID != this.UniqueID)
                    LoadProperty(_uniqueIDProperty, profile.UniqueID);
            }
            
            // A child relationship exists on this Business Object but its type is not a child type (E.G. EditableChild). 
            // TODO: Please override OnChildUpdated() and update this child manually.
            // FieldManager.UpdateChildren(this, connection);

            OnChildUpdated();
        }
        #endregion

        private void Child_DeleteSelf(SqlConnection connection)
        {
            bool cancel = false;
            OnChildSelfDeleting(connection, ref cancel);
            if (cancel) return;
            
            DataPortal_Delete(new AccountCriteria (AccountId), connection);
        
            OnChildSelfDeleted();
        }

        protected void DataPortal_Delete(AccountCriteria criteria, SqlConnection connection)
        {
            bool cancel = false;
            OnDeleting(criteria, connection, ref cancel);
            if (cancel) return;

            string commandText = string.Format("DELETE FROM [dbo].[Account] {0}", ADOHelper.BuildWhereStatement(criteria.StateBag));
            using (SqlCommand command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddRange(ADOHelper.SqlParameters(criteria.StateBag));

                //result: The number of rows changed, inserted, or deleted. -1 for select statements; 0 if no rows were affected, or the statement failed. 
                int result = command.ExecuteNonQuery();
                if (result == 0)
                    throw new DBConcurrencyException("The entity is out of date on the client. Please update the entity and try again. This could also be thrown if the sql statement failed to execute.");
            }

            OnDeleted();
        }

        private void Map(SafeDataReader reader)
        {
            bool cancel = false;
            OnMapping(reader, ref cancel);
            if (cancel) return;

            using(BypassPropertyChecks)
            {
                LoadProperty(_accountIdProperty, reader["AccountId"]);
                LoadProperty(_uniqueIDProperty, reader["UniqueID"]);
                LoadProperty(_emailProperty, reader["Email"]);
                LoadProperty(_firstNameProperty, reader["FirstName"]);
                LoadProperty(_lastNameProperty, reader["LastName"]);
                LoadProperty(_address1Property, reader["Address1"]);
                LoadProperty(_address2Property, reader["Address2"]);
                LoadProperty(_cityProperty, reader["City"]);
                LoadProperty(_stateProperty, reader["State"]);
                LoadProperty(_zipProperty, reader["Zip"]);
                LoadProperty(_countryProperty, reader["Country"]);
                LoadProperty(_phoneProperty, reader["Phone"]);
            }

            OnMapped();

            MarkAsChild();
            MarkOld();
        }
    }
}
