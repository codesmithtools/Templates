using System;
using System.Linq;
using Petshop.Data;
using Petshop.Data.Entities;

namespace Petshop.Data.Entities
{
    public static partial class ProductExtensions
    {
        public static IQueryable<Product> Search(this IQueryable<Product> queryable, string keywords)
        {
            return queryable.Where(p => p.Name.ToLower().Contains(keywords.ToLower()) || p.Description.ToLower().Contains(keywords.ToLower()));
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