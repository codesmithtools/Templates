﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v6.0.3, CSLA Templates: v4.0.0.0, CSLA Framework: v4.3.10.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'CategoryList.cs'.
//
//     Template: EditableRootList.DataAccess.ParameterizedSQL.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;

using Csla;
using Csla.Data;

namespace PetShop.Tests.ParameterizedSQL
{
    public partial class CategoryList
    {
        [RunLocal]
        protected override void DataPortal_Create()
        {
        }

        private void DataPortal_Fetch(CategoryCriteria criteria)
        {
            bool cancel = false;
            OnFetching(criteria, ref cancel);
            if (cancel) return;

            RaiseListChangedEvents = false;

            // Fetch Child objects.
            string commandText = String.Format("SELECT [CategoryId], [Name], [Descn] FROM [dbo].[Category] {0}", ADOHelper.BuildWhereStatement(criteria.StateBag));
            using (var connection = new SqlConnection(ADOHelper.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddRange(ADOHelper.SqlParameters(criteria.StateBag));

                    using(var reader = new SafeDataReader(command.ExecuteReader()))
                    {
                        if(reader.Read())
                        {
                            do
                            {
                                this.Add(PetShop.Tests.ParameterizedSQL.Category.GetCategory(reader));
                            } while(reader.Read());
                        }
                    }
                }
            }

            RaiseListChangedEvents = true;

            OnFetched();
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Update()
        {
            bool cancel = false;
            OnUpdating(ref cancel);
            if (cancel) return;

            RaiseListChangedEvents = false;

            for (int index = 0; index < DeletedList.Count; index++)
            {
                DeletedList[index] = DeletedList[index].Save();
            }
           
            DeletedList.Clear();

            for (int index = 0; index < Items.Count; index++)
            {
                Items[index] = Items[index].Save();
            }

            RaiseListChangedEvents = true;

            OnUpdated();
        }
    }
}
