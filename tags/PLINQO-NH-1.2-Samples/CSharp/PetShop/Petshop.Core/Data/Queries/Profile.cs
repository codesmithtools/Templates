using System;
using System.Linq;
using CodeSmith.Data.Linq;
using NHibernate.Linq;
using Petshop.Data;
using Petshop.Data.Entities;

namespace Petshop.Data.Entities
{
    public static partial class ProfileExtensions
    {
        public static Profile GetProfile(this IQueryable<Profile> query, string name)
        {
            return query
                .ByUsername(name)
                .Fetch(p => p.AccountList)
                .FirstOrDefault();
        }

        #region Query

        // A private class for lazy loading static compiled queries.
        private static partial class Query
        {
            // Place custom compiled queries here. 
        } 

        #endregion
    }
}