﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a CodeSmith Template.
//
//     DO NOT MODIFY contents of this file. Changes to this
//     file will be lost if the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Data.Linq;

namespace Tester.Data
{
    /// <summary>
    /// The query extension class for UserAccount.
    /// </summary>
    public static partial class UserAccountExtensions
    {

        /// <summary>
        /// Gets an instance by the primary key.
        /// </summary>
        public static Tester.Data.UserAccount GetByKey(this IQueryable<Tester.Data.UserAccount> queryable, System.Guid userAccountID)
        {
            var entity = queryable as System.Data.Linq.Table<Tester.Data.UserAccount>;
            if (entity != null && entity.Context.LoadOptions == null)
                return Query.GetByKey.Invoke((Tester.Data.TesterDataContext)entity.Context, userAccountID);
            
            return queryable.FirstOrDefault(u => u.UserAccountID == userAccountID);
        }

        /// <summary>
        /// Immediately deletes the entity by the primary key from the underlying data source with a single delete command.
        /// </summary>
        /// <param name="table">Represents a table for a particular type in the underlying database containing rows are to be deleted.</param>
        /// <returns>The number of rows deleted from the database.</returns>
        public static int Delete(this System.Data.Linq.Table<Tester.Data.UserAccount> table, System.Guid userAccountID)
        {
            return table.Delete(u => u.UserAccountID == userAccountID);
        }
        
        /// <summary>
        /// Gets a query for <see cref="UserAccount.FirstName"/>.
        /// </summary>
        public static IQueryable<Tester.Data.UserAccount> GetByFirstName(this IQueryable<Tester.Data.UserAccount> queryable, string firstName)
        {
            return queryable.Where(u => u.FirstName == firstName);
        }
        
        /// <summary>
        /// Gets a query for <see cref="UserAccount.LastName"/>.
        /// </summary>
        public static IQueryable<Tester.Data.UserAccount> GetByLastName(this IQueryable<Tester.Data.UserAccount> queryable, string lastName)
        {
            return queryable.Where(u => u.LastName == lastName);
        }
        
        /// <summary>
        /// Gets a query for <see cref="UserAccount.ZipCode"/>.
        /// </summary>
        public static IQueryable<Tester.Data.UserAccount> GetByZipCode(this IQueryable<Tester.Data.UserAccount> queryable, string zipCode)
        {
            return queryable.Where(u => u.ZipCode == zipCode);
        }

        #region Query
        /// <summary>
        /// A private class for lazy loading static compiled queries.
        /// </summary>
        private static partial class Query
        {

            internal static readonly Func<Tester.Data.TesterDataContext, System.Guid, Tester.Data.UserAccount> GetByKey = 
                System.Data.Linq.CompiledQuery.Compile(
                    (Tester.Data.TesterDataContext db, System.Guid userAccountID) => 
                        db.UserAccount.FirstOrDefault(u => u.UserAccountID == userAccountID));

        }
        #endregion
    }
}

