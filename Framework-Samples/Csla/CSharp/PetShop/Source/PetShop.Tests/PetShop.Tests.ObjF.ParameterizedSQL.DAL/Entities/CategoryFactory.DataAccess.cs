﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v6.0.3, CSLA Templates: v4.0.0.0, CSLA Framework: v4.3.10.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'Category.cs'.
//
//     Template: ObjectFactory.DataAccess.ParameterizedSQL.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;

using Csla;
using Csla.Data;
using Csla.Server;

using PetShop.Tests.ObjF.ParameterizedSQL;

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
            MarkNew(item);

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
            string commandText = String.Format("SELECT [CategoryId], [Name], [Descn] FROM [dbo].[Category] {0}", ADOHelper.BuildWhereStatement(criteria.StateBag));
            using (var connection = new SqlConnection(ADOHelper.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddRange(ADOHelper.SqlParameters(criteria.StateBag));
                    using(var reader = new SafeDataReader(command.ExecuteReader()))
                    {
                        if (reader.Read())
                            item = Map(reader);
                        else
                            throw new Exception(String.Format("The record was not found in 'dbo.Category' using the following criteria: {0}.", criteria));
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
            using (var connection = new SqlConnection(ADOHelper.ConnectionString))
            {
                connection.Open();
                using(var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@p_CategoryId", item.CategoryId);
                command.Parameters.AddWithValue("@p_Name", ADOHelper.NullCheck(item.Name));
                command.Parameters.AddWithValue("@p_Descn", ADOHelper.NullCheck(item.Description));

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



                Update_Products_Products_FK__Product__Categor__0CBAE877(ref item);
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



                    Update_Products_Products_FK__Product__Categor__0CBAE877(ref item);
    
                    // Delete the old.
                    var criteria = new CategoryCriteria {CategoryId = item.OriginalCategoryId};
                    
                    Delete(criteria);
    
                    // Mark the original as the new one.
                    item.OriginalCategoryId = item.CategoryId;

                    MarkOld(item);
                    CheckRules(item);
                    OnUpdated();

                    return;
                }

                const string commandText = "UPDATE [dbo].[Category] SET [CategoryId] = @p_CategoryId, [Name] = @p_Name, [Descn] = @p_Descn WHERE [CategoryId] = @p_CategoryId; SELECT [CategoryId] FROM [dbo].[Category] WHERE [CategoryId] = @p_OriginalCategoryId";
                using (var connection = new SqlConnection(ADOHelper.ConnectionString))
                {
                    connection.Open();
                    using(var command = new SqlCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@p_OriginalCategoryId", item.OriginalCategoryId);
                command.Parameters.AddWithValue("@p_CategoryId", item.CategoryId);
                command.Parameters.AddWithValue("@p_Name", ADOHelper.NullCheck(item.Name));
                command.Parameters.AddWithValue("@p_Descn", ADOHelper.NullCheck(item.Description));

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



                Update_Products_Products_FK__Product__Categor__0CBAE877(ref item);
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

            var criteria = new CategoryCriteria{CategoryId = item.CategoryId};
            
            DoDelete(criteria);

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

            string commandText = String.Format("DELETE FROM [dbo].[Category] {0}", ADOHelper.BuildWhereStatement(criteria.StateBag));
            using (var connection = new SqlConnection(ADOHelper.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(commandText, connection))
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
            
            MarkOld(item);

            return item;
        }




        //Where(a => a.AssociationType == AssociationType.OneToMany  || a.AssociationType == AssociationType.ZeroOrOneToMany  || a.AssociationType == AssociationType.ManyToMany)
        private static void Update_Products_Products_FK__Product__Categor__0CBAE877(ref Category item)
        {
            foreach(Product itemToUpdate in item.Products)
            {
                itemToUpdate.CategoryId = item.CategoryId;

                new ProductFactory().Update(itemToUpdate, true);
            }
        }

        #endregion

        #region DataPortal partial methods

        /// <summary>
        /// Codesmith generated stub method that is called when creating the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object creation should proceed.</param>
        partial void OnCreating(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Category"/> object has been created. 
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// Codesmith generated stub method that is called when fetching the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="criteria"><see cref="CategoryCriteria"/> object containg the criteria of the object to fetch.</param>
        /// <param name="cancel">Value returned from the method indicating whether the object fetching should proceed.</param>
        partial void OnFetching(CategoryCriteria criteria, ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Category"/> object has been fetched. 
        /// </summary>    
        partial void OnFetched();

        /// <summary>
        /// Codesmith generated stub method that is called when mapping the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object mapping should proceed.</param>
        partial void OnMapping(ref bool cancel);
 
        /// <summary>
        /// Codesmith generated stub method that is called when mapping the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancel">Value returned from the method indicating whether the object mapping should proceed.</param>
        partial void OnMapping(SafeDataReader reader, ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Category"/> object has been mapped. 
        /// </summary>
        partial void OnMapped();

        /// <summary>
        /// Codesmith generated stub method that is called when inserting the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object insertion should proceed.</param>
        partial void OnInserting(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Category"/> object has been inserted. 
        /// </summary>
        partial void OnInserted();

        /// <summary>
        /// Codesmith generated stub method that is called when updating the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object creation should proceed.</param>
        partial void OnUpdating(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Category"/> object has been updated. 
        /// </summary>
        partial void OnUpdated();

        /// <summary>
        /// Codesmith generated stub method that is called when self deleting the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object self deletion should proceed.</param>
        partial void OnSelfDeleting(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Category"/> object has been deleted. 
        /// </summary>
        partial void OnSelfDeleted();

        /// <summary>
        /// Codesmith generated stub method that is called when deleting the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="criteria"><see cref="CategoryCriteria"/> object containg the criteria of the object to delete.</param>
        /// <param name="cancel">Value returned from the method indicating whether the object deletion should proceed.</param>
        partial void OnDeleting(CategoryCriteria criteria, ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Category"/> object with the specified criteria has been deleted. 
        /// </summary>
        partial void OnDeleted();
        partial void OnChildLoading(Csla.Core.IPropertyInfo childProperty, ref bool cancel);

        #endregion
    }
}