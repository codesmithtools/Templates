//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v5.2.1, CSLA Templates: v2.0.0.1440, CSLA Framework: v3.8.2.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'Category.cs'.
//
//     Template: ObjectFactory.DataAccess.ParameterizedSQL.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
#region Using declarations

using System;
using System.Data;
using System.Data.SqlClient;

using Csla;
using Csla.Server;
using Csla.Data;

using PetShop.Tests.ObjF.ParameterizedSQL;

#endregion

namespace PetShop.Tests.ObjF.ParameterizedSQL.DAL
{
    public partial class CategoryFactory : ObjectFactory
    {
        #region Create

        /// <summary>
        /// Creates new Category with default values.
        /// </summary>
        /// <returns>new Category.</returns>
        [RunLocal]
        public Category Create()
        {
            var item = (Category)Activator.CreateInstance(typeof(Category), true);

            bool cancel = false;
            OnCreating(ref cancel);
            if (cancel) return item;

            using (BypassPropertyChecks(item))
            {
                // Default values.
            }

            CheckRules(item);
            MarkNew(item);
            OnCreated();

            return item;
        }

        /// <summary>
        /// Creates new Category with default values.
        /// </summary>
        /// <returns>new Category.</returns>
        [RunLocal]
        private Category Create(CategoryCriteria criteria)
        {
            var item = (Category)Activator.CreateInstance(typeof(Category), true);

            bool cancel = false;
            OnCreating(ref cancel);
            if (cancel) return item;

            var resource = Fetch(criteria);
            using (BypassPropertyChecks(item))
            {
                item.Name = resource.Name;
                item.Description = resource.Description;
            }

            CheckRules(item);
            MarkNew(resource);

            OnCreated();

            return item;
        }

        #endregion

        #region Fetch

        /// <summary>
        /// Fetch Category.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public Category Fetch(CategoryCriteria criteria)
        {
            bool cancel = false;
            OnFetching(criteria, ref cancel);
            if (cancel) return null;

            Category item;
            string commandText = string.Format("SELECT [CategoryId], [Name], [Descn] FROM [dbo].[Category] {0}", ADOHelper.BuildWhereStatement(criteria.StateBag));
            using (SqlConnection connection = new SqlConnection(ADOHelper.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddRange(ADOHelper.SqlParameters(criteria.StateBag));
                    using(var reader = new SafeDataReader(command.ExecuteReader()))
                    {
                        if (reader.Read())
                            item = Map(reader);
                        else
                            throw new Exception(String.Format("The record was not found in 'Category' using the following criteria: {0}.", criteria));
                    }
                }
            }

            MarkOld(item);
            OnFetched();
            return item;
        }

        #endregion

        #region Insert

        private void DoInsert(ref Category item, bool stopProccessingChildren)
        {
            // Don't update if the item isn't dirty.
            if (!item.IsDirty) return;

            bool cancel = false;
            OnInserting(ref cancel);
            if (cancel) return;

            const string commandText = "INSERT INTO [dbo].[Category] ([CategoryId], [Name], [Descn]) VALUES (@p_CategoryId, @p_Name, @p_Descn)";
            using (SqlConnection connection = new SqlConnection(ADOHelper.ConnectionString))
            {
                connection.Open();
                using(SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@p_CategoryId", item.CategoryId);
					command.Parameters.AddWithValue("@p_Name", item.Name);
					command.Parameters.AddWithValue("@p_Descn", item.Description);

                    using(var reader = new SafeDataReader(command.ExecuteReader()))
                    {
                        if(reader.Read())
                        {
                        }
                    }
                }
            }

            item.OriginalCategoryId = item.CategoryId;

            MarkOld(item);
            CheckRules(item);
            
            if(!stopProccessingChildren)
            {
            // Update Child Items.
                Update_Product_Products_CategoryId(ref item);
            }

            OnInserted();
        }

        #endregion

        #region Update

        [Transactional(TransactionalTypes.TransactionScope)]
        public Category Update(Category item)
        {
            return Update(item, false);
        }

        public Category Update(Category item, bool stopProccessingChildren)
        {
            if(item.IsDeleted)
            {
                DoDelete(ref item);
                MarkNew(item);
            }
            else if(item.IsNew)
            {
                DoInsert(ref item, stopProccessingChildren);
            }
            else
            {
                DoUpdate(ref item, stopProccessingChildren);
            }

            return item;
        }

        private void DoUpdate(ref Category item, bool stopProccessingChildren)
        {
            bool cancel = false;
            OnUpdating(ref cancel);
            if (cancel) return;

            // Don't update if the item isn't dirty.
            if (item.IsDirty)
            {
                if(item.OriginalCategoryId != item.CategoryId)
                {
                    // Insert new child.
                    var temp = (Category)Activator.CreateInstance(typeof(Category), true);
                    temp.CategoryId = item.CategoryId;
                    temp.Name = item.Name;
                    temp.Description = item.Description;
                    temp = temp.Save();
    
                    // Mark child lists as dirty. This code may need to be updated to one-to-one relationships.
                    foreach(Product itemToUpdate in item.Products)
                    {
				itemToUpdate.CategoryId = item.CategoryId;
                    }

                    // Update Children
                    Update_Product_Products_CategoryId(ref item);
    
                    // Delete the old.
                    Delete(new CategoryCriteria {CategoryId = item.OriginalCategoryId});
    
                    // Mark the original as the new one.
                    item.OriginalCategoryId = item.CategoryId;

                    MarkOld(item);
                    CheckRules(item);
                    OnUpdated();

                    return;
                }

                const string commandText = "UPDATE [dbo].[Category]  SET [CategoryId] = @p_CategoryId, [Name] = @p_Name, [Descn] = @p_Descn WHERE [CategoryId] = @p_CategoryId";
                using (SqlConnection connection = new SqlConnection(ADOHelper.ConnectionString))
                {
                    connection.Open();
                    using(SqlCommand command = new SqlCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@p_OriginalCategoryId", item.OriginalCategoryId);
					command.Parameters.AddWithValue("@p_CategoryId", item.CategoryId);
					command.Parameters.AddWithValue("@p_Name", item.Name);
					command.Parameters.AddWithValue("@p_Descn", item.Description);

                        //result: The number of rows changed, inserted, or deleted. -1 for select statements; 0 if no rows were affected, or the statement failed. 
                        int result = command.ExecuteNonQuery();
                        if (result == 0)
                            throw new DBConcurrencyException("The entity is out of date on the client. Please update the entity and try again. This could also be thrown if the sql statement failed to execute.");
                    }
                }
            }

            item.OriginalCategoryId = item.CategoryId;

            MarkOld(item);
            CheckRules(item);

            if(!stopProccessingChildren)
            {
                // Update Child Items.
                Update_Product_Products_CategoryId(ref item);
            }

            OnUpdated();
        }
        #endregion

        #region Delete

        [Transactional(TransactionalTypes.TransactionScope)]
        public void Delete(CategoryCriteria criteria)
        {
            // Note: this call to delete is for immediate deletion and doesn't keep track of any entity state.
            DoDelete(criteria);
        }

        protected void DoDelete(ref Category item)
        {
            // If we're not dirty then don't update the database.
            if (!item.IsDirty) return;

            // If we're new then don't call delete.
            if (item.IsNew) return;

            DoDelete(new CategoryCriteria{CategoryId = item.CategoryId});

            MarkNew(item);
        }

        /// <summary>
        /// This call to delete is for immediate deletion and doesn't keep track of any entity state.
        /// </summary>
        /// <param name="criteria">The Criteria.</param>
        private void DoDelete(CategoryCriteria criteria)
        {
            bool cancel = false;
            OnDeleting(criteria, ref cancel);
            if (cancel) return;

            string commandText = string.Format("DELETE FROM [dbo].[Category] {0}", ADOHelper.BuildWhereStatement(criteria.StateBag));
            using (SqlConnection connection = new SqlConnection(ADOHelper.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddRange(ADOHelper.SqlParameters(criteria.StateBag));

                    //result: The number of rows changed, inserted, or deleted. -1 for select statements; 0 if no rows were affected, or the statement failed. 
                    int result = command.ExecuteNonQuery();
                    if (result == 0)
                        throw new DBConcurrencyException("The entity is out of date on the client. Please update the entity and try again. This could also be thrown if the sql statement failed to execute.");
                }
            }

            OnDeleted();
        }

        #endregion

        #region Helper Methods

        public Category Map(SafeDataReader reader)
        {
            var item = (Category)Activator.CreateInstance(typeof(Category), true);
            using (BypassPropertyChecks(item))
            {
                item.CategoryId = reader.GetString("CategoryId");
                item.OriginalCategoryId = reader.GetString("CategoryId");
                item.Name = reader.GetString("Name");
                item.Description = reader.GetString("Descn");
            }
            
            return item;
        }

        //AssociatedOneToMany
        private static void Update_Product_Products_CategoryId(ref Category item)
        {
            foreach(Product itemToUpdate in item.Products)
            {
				itemToUpdate.CategoryId = item.CategoryId;

                new ProductFactory().Update(itemToUpdate, true);
            }
        }

        #endregion

        #region Data access partial methods

        partial void OnCreating(ref bool cancel);
        partial void OnCreated();
        partial void OnFetching(CategoryCriteria criteria, ref bool cancel);
        partial void OnFetched();
        partial void OnMapping(ref bool cancel);
        partial void OnMapped();
        partial void OnInserting(ref bool cancel);
        partial void OnInserted();
        partial void OnUpdating(ref bool cancel);
        partial void OnUpdated();
        partial void OnSelfDeleting(ref bool cancel);
        partial void OnSelfDeleted();
        partial void OnDeleting(CategoryCriteria criteria, ref bool cancel);
        partial void OnDeleted();

        #endregion
    }
}